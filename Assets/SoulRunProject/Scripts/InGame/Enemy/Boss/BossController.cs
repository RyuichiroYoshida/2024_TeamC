using System;
using System.Collections.Generic;
using SoulRunProject.Common;
using SoulRunProject.SoulMixScene;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SoulRunProject.InGame
{
    /// <summary>
    /// ボスの行動を管理するクラス
    /// 行動様式
    /// 最初に入場ムーブ
    /// 一行動を起こすたびに次の行動からランダムで実行する、
    /// </summary>
    public class BossController : MonoBehaviour
    {
        [SerializeField, Header("スコア")]
        private int _score = 100;
        
        [SerializeField, Tooltip("敵のパラメータを設定する")]
        protected Status _status;
        
        [SerializeField] LootTable _lootTable;
        [Header("ボスの行動"), SerializeReference, SubclassSelector] List<IBossBehavior> _bossBehaviors;

        [SerializeField, CustomLabel("行動待機時間")]
        private float _behaviorIntervalTime;

        private BossState _currentState = BossState.Animation;
        private float _intervalTimer;
        private int _inActionIndex;

        private void Start()
        {
            _status = _status.Copy();

            foreach (var behavior in _bossBehaviors)
            {
                behavior.Initialize(this);
                ((BossBehaviorBase)behavior).OnFinishAction += () =>
                {
                    _currentState = BossState.Standby;
                    _intervalTimer = 0;
                };
            }

            _currentState = BossState.Standby; // とりあえず
        }
        
        private void Update()
        {
            if (_currentState == BossState.Animation)
            {
            }
            else if (_currentState == BossState.Standby)
            {
                _intervalTimer += Time.deltaTime;
                
                if (_intervalTimer >= _behaviorIntervalTime)
                {
                    _currentState = BossState.InAction;
                    _inActionIndex = Random.Range(0, _bossBehaviors.Count);
                    _bossBehaviors[_inActionIndex].BeginAction();
                    _bossBehaviors[_inActionIndex].UpdateAction(Time.deltaTime); // ???
                }
            }
            else
            {
                _bossBehaviors[_inActionIndex].UpdateAction(Time.deltaTime);
            }
        }

        private enum BossState
        {
            Animation, // 登場時などのAnimation中
            Standby, // 行動待機中
            InAction // IBossBehaviorのAction中
        }
    }
    
    /// <summary>
    /// ボスの行動が持つインターフェイス
    /// </summary>
    public interface IBossBehavior
    {
        /// <summary> Script初期化処理 </summary>
        public void Initialize(BossController bossController);
        /// <summary> Action開始 </summary>
        public void BeginAction();
        /// <summary> Action中Update </summary>
        public void UpdateAction(float deltaTime);
        /// <summary> Action強化 </summary> ActionなのかBehaviorなのか
        public void PowerUpBehavior();
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

        public abstract void Initialize(BossController bossController);
        public abstract void BeginAction();
        public abstract void UpdateAction(float deltaTime);
        public abstract void PowerUpBehavior();
    }
}
