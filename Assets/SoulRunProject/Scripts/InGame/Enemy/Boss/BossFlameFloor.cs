using SoulRunProject.Common;
using UnityEngine;
using SoulRunProject.Audio;

namespace SoulRunProject.InGame
{
    [Name("炎の床生成")]
    public class BossFlameFloor : BossBehaviorBase
    {
        [SerializeField] private Animator _animator;

        [SerializeField, CustomLabel("X軸移動範囲")]
        private float _moveRangeX;

        [SerializeField, CustomLabel("Y軸移動範囲")]
        private float _moveRangeY;

        [SerializeField, CustomLabel("発射位置")] private Transform _firingPosition;

        [SerializeField, CustomLabel("火炎弾プレハブ")]
        private BossFlameBallController _flameBallPrefab;

        [Header("性能")] [SerializeField, CustomLabel("行動時間 ∞字一周の時間")]
        private float _actionTime;

        [SerializeField, CustomLabel("火炎弾スピード")]
        private float _flameBallSpeed;

        [SerializeField, CustomLabel("一回の行動の火炎弾の数")]
        private int _flameBallCount;

        [SerializeField, CustomLabel("延焼ダメージ(秒)")]
        private float _flameDamage;

        [SerializeField, CustomLabel("着弾地点のプレイヤーとの距離"), Tooltip("ブレス発射時のプレイヤーの位置からの着弾地点の距離")]
        private float _deviationDistance;

        [SerializeField, CustomLabel("着弾地点の最大のずれ幅")]
        private float _flameBallAccuracy;

        [Header("強化後性能")] 
        [SerializeField, CustomLabel("行動時間")]
        private float _actionTimeBuffed;

        [SerializeField, CustomLabel("火炎弾の数")] 
        private int _flameBallCountBuffed;

        // 移動
        private float _actionTimer;
        private Transform _bossTransform;
        private Vector3 _defaultBossPos;

        private Transform _playerTransform;

        // flame
        private float _flameBallTimer;

        public override void Initialize(BossController bossController, Transform playerTf)
        {
            _bossTransform = bossController.transform;
            _defaultBossPos = _bossTransform.position;
            _playerTransform = playerTf;
        }

        public override void BeginAction()
        {
            _actionTimer = 0;
            _animator.SetTrigger("FrameFloor");
            _animator.SetFloat("Speed", 6 / _actionTime); // 6 => animationの長さ
        }

        public override void UpdateAction(float deltaTime)
        {
            // flame
            _flameBallTimer += deltaTime;

            if (_flameBallTimer >= _actionTime / _flameBallCount)
            {
                _flameBallTimer = 0;

                // flame ball 生成と向きの調整
                BossFlameBallController flameBall =
                    GameObject.Instantiate(_flameBallPrefab, _firingPosition.position, Quaternion.identity);
                Vector3 impactPos = _playerTransform.position;
                impactPos.x += Random.Range(-_flameBallAccuracy, _flameBallAccuracy);
                impactPos.y = 0; // fieldの高さ
                impactPos.z += _deviationDistance;
                flameBall.transform.LookAt(impactPos);
                flameBall.Initialize(_flameBallSpeed, _flameDamage);
                CriAudioManager.Instance.Play(CriAudioType.CueSheet_SE, "SE_Fire_Shot");
            }

            // 時間と移動
            _actionTimer += deltaTime;

            if (_actionTimer >= _actionTime)
            {
                // 終了処理
                OnFinishAction?.Invoke();
            }
        }

        public override void BuffBehavior(BossController bossController)
        {
            _flameBallCount = _flameBallCountBuffed;
            _actionTime = _actionTimeBuffed;
        }
    }
}