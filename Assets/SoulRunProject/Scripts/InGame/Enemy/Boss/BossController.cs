using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using SoulRunProject.Audio;
using SoulRunProject.Common;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace SoulRunProject.InGame
{
    /// <summary>
    /// ボスの行動を管理するクラス
    /// 行動様式
    /// 最初に入場ムーブ
    /// 一行動を起こすたびに次の行動からランダムで実行する、
    /// </summary>
    public class BossController : MonoBehaviour, IPausable
    {
        [SerializeField, CustomLabel("デフォルト位置")] private Vector3 _defaultPos;
        [SerializeField] private Image _hpImage;
        [SerializeField] private Animator _bossAnimator;
        [SerializeField, Tooltip("パワーアップする閾値(%)")] private float _powerUpThreshold; 
        [Header("ボスの行動"), CustomLabel("行動の種類"), SerializeReference, SubclassSelector] IBossBehavior[] _bossBehaviors;
        [SerializeField, CustomLabel("行動待機時間")] private float _behaviorIntervalTime;
        [SerializeField, CustomLabel("強化後行動待機時間")] private float _buffedIntervalTime;
        [SerializeField, CustomLabel("死亡アニメーション時間")] private float _deadAnimationTime;

        private BossState _currentState = BossState.Animation;
        private int _thresholdIndex;
        private float _interval;
        private float _intervalTimer;
        /// <summary> 動いている行動 </summary>
        private IBossBehavior _inActionBehavior;
        private bool _isPause;

        public float DeadAnimationTime => _deadAnimationTime;

        private void Awake()
        {
            Register();
        }

        private void OnDestroy()
        {
            UnRegister();
        }

        public void InitializePosition(float positionX)
        {
            Vector3 pos = new Vector3(positionX, _defaultPos.y, _defaultPos.z);
            transform.position = pos;
        }

        private void Start()
        {
            Transform playerTf = FindObjectOfType<PlayerManager>().transform;
            _interval = _behaviorIntervalTime;
            
            foreach (var behavior in _bossBehaviors)
            {
                behavior.Initialize(this, playerTf);
                ((BossBehaviorBase)behavior).OnFinishAction += () =>
                {
                    _currentState = BossState.Standby;
                    _intervalTimer = 0;
                };
            }
            
            DamageableEntity bossDamageable = GetComponent<DamageableEntity>();
            bossDamageable.CurrentHp.Subscribe(hp => _hpImage.fillAmount = hp / bossDamageable.MaxHp).AddTo(this);
            // hp減少により強化される
            bossDamageable.CurrentHp
                .SkipLatestValueOnSubscribe()
                .Where(hp => _powerUpThreshold > hp / bossDamageable.MaxHp * 100)
                .Take(1) // 現時点強化は1回だけ
                .Subscribe(hp =>
                {
                    foreach (var behavior in _bossBehaviors) behavior.BuffBehavior(this);
                    _interval = _buffedIntervalTime;
                })
                .AddTo(this);

            bossDamageable.OnDead += () =>
            {
                CriAudioManager.Instance.StopAll();
                CriAudioManager.Instance.Play(CriAudioType.CueSheet_ME, "ME_Boss_Defeat");
            };

            // 入場アニメーション
            _ = PlayEntryAnimation();
        }
        
        private void Update()
        {
            if (_isPause) return;
            
            switch (_currentState)
            {
                case BossState.Animation:
                    break;
                
                case BossState.Standby:
                    _intervalTimer += Time.deltaTime;
                
                    if (_intervalTimer >= _interval)
                    {
                        _currentState = BossState.InAction;
                        IBossBehavior[] selectable = _bossBehaviors.Where(behavior => behavior != _inActionBehavior).ToArray();
                        _inActionBehavior = selectable[Random.Range(0, selectable.Length)];// 直前をのぞいた一つをランダムに選択
                        _inActionBehavior.BeginAction();
                        _inActionBehavior.UpdateAction(Time.deltaTime);
                    }
                    break;
                
                case BossState.InAction:
                    _inActionBehavior.UpdateAction(Time.deltaTime);
                    break;
            }
        }

        async UniTask PlayEntryAnimation()
        {
            _currentState = BossState.Animation;
            _bossAnimator.SetTrigger("Entry");
            await UniTask.WaitUntil(() => _bossAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f, 
                cancellationToken: this.GetCancellationTokenOnDestroy());
            _currentState = BossState.Standby;
            CriAudioManager.Instance.Play(CriAudioType.CueSheet_VOICE, "VOICE_BossLaughing");
        }

        private enum BossState
        {
            Animation, // 登場時などのAnimation中
            Standby, // 行動待機中
            InAction // IBossBehaviorのAction中
        }

        public void Register()
        {
            PauseManager.RegisterPausableObject(this);
        }

        public void UnRegister()
        {
            PauseManager.UnRegisterPausableObject(this);
        }

        public void Pause(bool isPause)
        {
            _isPause = isPause;
        }
    }
    
    /// <summary>
    /// ボスの行動が持つインターフェイス
    /// </summary>
    public interface IBossBehavior
    {
        /// <summary> Script初期化処理 </summary>
        public void Initialize(BossController bossController, Transform playerTf);
        /// <summary> Action開始 </summary>
        public void BeginAction();
        /// <summary> Action中Update </summary>
        public void UpdateAction(float deltaTime);
        /// <summary> 攻撃強化処理 </summary>
        public void BuffBehavior(BossController bossController);
    }

    /// <summary>
    /// Boss行動の基底クラス
    /// </summary>
    /// SubclassSelectorを使いたいけどフィールドも欲しい
    [Name("抽象クラス")]
    public abstract class BossBehaviorBase : IBossBehavior
    {
        /// <summary> Action終了時に呼ばれる </summary>
        public Action OnFinishAction;

        public abstract void Initialize(BossController bossController, Transform playerTf);
        public abstract void BeginAction();
        public abstract void UpdateAction(float deltaTime);
        public abstract void BuffBehavior(BossController bossController);
    }
}
