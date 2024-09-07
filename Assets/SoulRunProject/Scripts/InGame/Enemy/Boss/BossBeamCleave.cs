using System;
using DG.Tweening;
using SoulRunProject.Audio;
using SoulRunProject.Common;
using Unity.Mathematics;
using UnityEngine;

namespace SoulRunProject.InGame
{
    [Name("ビーム薙ぎ払い")]
    public class BossBeamCleave : BossBehaviorBase
    {
        [SerializeField, CustomLabel("エフェクトプレハブ")]
        private GameObject _beamEffect;

        [SerializeField, CustomLabel("チャージエフェクト")]
        private ParticleSystem _chargeEffect;

        [SerializeField, CustomLabel("ビーム原点")] private Transform _beamOrigin;

        [SerializeField, CustomLabel("開始着弾地点Z座標(プレイヤーからの相対)")]
        private float _beamStarPosZ;
        
        [SerializeField, CustomLabel("着弾距離")]
        private float _beamHitLength;

        [SerializeField, CustomLabel("当たるレイヤー")]
        private LayerMask _collisionLayer;

        [Header("性能")] [SerializeField, CustomLabel("薙ぎ払い時間")]
        private float _beamTime;

        [SerializeField, CustomLabel("チャージ時間")]
        private float _chargeDuration;

        [SerializeField, CustomLabel("ダメージ")] private float _damage;
        [SerializeField] private int _hitCount; // 多段ヒットなのか1回だけなのか

        [Header("強化内容"), EnumDrawer(typeof(PowerUpName)), SerializeReference, SubclassSelector]
        PowerUpBehaviorBase[] _powerUpBehaviors;

        private GameObject _laserInstance;
        private Transform _playerTf;
        private Vector3 _startImpactPosition;
        private Vector3 _finishImpactPosition;
        private float _cleaveTimer;
        private float _chargeTimer;
        private int _hitCounter;
        private Guid _beamSound;

        public override void Initialize(BossController bossController, Transform playerTf)
        {
            // エフェクトインスタンスの初期化
            _laserInstance = GameObject.Instantiate(_beamEffect, _beamOrigin);
            _laserInstance.GetComponent<LaserEffectController>().HitLayer = _collisionLayer;
            _laserInstance.SetActive(false);
            _chargeEffect = GameObject.Instantiate(_chargeEffect, _beamOrigin);
            _chargeEffect.Stop();
            _playerTf = playerTf;

            // 行動パワーアップの代入
            foreach (var powerUpBehavior in _powerUpBehaviors)
            {
                powerUpBehavior.Initialize(this);
                PowerUpBejaviors.Add(powerUpBehavior.PowerUpBehavior);
            }

            // sound

            _beamSound = CriAudioManager.Instance.Play(CriAudioType.CueSheet_SE, "SE_Laser");
            CriAudioManager.Instance.Pause(CriAudioType.CueSheet_SE, _beamSound);
        }

        public override void BeginAction()
        {
            _hitCounter = 0;
            _cleaveTimer = 0;
            _chargeTimer = 0;

            // ビーム角度のリセット
            // Vector3 playerPos = _playerTf.position;
            // _startImpactPosition = new Vector3(playerPos.x, 0, _playerTf.position.z + _beamStarPosZ);
            // _finishImpactPosition = new Vector3(playerPos.x, 0, _playerTf.position.z + _beamStarPosZ - _beamHitLength);
            // _beamOrigin.rotation = Quaternion.LookRotation(_startImpactPosition - _beamOrigin.position);
            //_laserInstance.SetActive(true);
            _chargeEffect.Play();
        }

        public override void UpdateAction(float deltaTime)
        {
            if (_chargeTimer < _chargeDuration)
            {
                _chargeTimer += deltaTime;

                if (_chargeTimer >= _chargeDuration)
                {
                    // ビーム角度のリセット
                    Vector3 playerPos = _playerTf.position;
                    _startImpactPosition = new Vector3(playerPos.x, 0, _playerTf.position.z + _beamStarPosZ);
                    _finishImpactPosition = new Vector3(playerPos.x, 0, _playerTf.position.z + _beamStarPosZ - _beamHitLength);
                    _beamOrigin.rotation = Quaternion.LookRotation(_startImpactPosition - _beamOrigin.position);
                    // sound
                    CriAudioManager.Instance.Resume(CriAudioType.CueSheet_SE, _beamSound);
                }
            }
            else if (_cleaveTimer < _beamTime)
            {
                _laserInstance.SetActive(true);
                _beamOrigin.rotation = Quaternion.LookRotation(Vector3.Lerp(_startImpactPosition - _beamOrigin.position,
                    _finishImpactPosition - _beamOrigin.position, _cleaveTimer));
                _cleaveTimer += deltaTime;
            }
            else
            {
                // 終了処理
                _laserInstance.SetActive(false);
                CriAudioManager.Instance.Pause(CriAudioType.CueSheet_SE, _beamSound);
                OnFinishAction?.Invoke();
            }

            // 当たり判定
            if (Physics.Raycast(_beamOrigin.position, _beamOrigin.forward, out RaycastHit hit, float.MaxValue,
                    _collisionLayer))
            {
                if (hit.collider.gameObject.TryGetComponent(out PlayerManager playerManager))
                {
                    if (_hitCounter < _hitCount)
                    {
                        _hitCounter++;
                        playerManager.Damage(_damage);
                    }
                }
            }
        }

        [Serializable, Name("抽象クラス")]
        abstract class PowerUpBehaviorBase
        {
            protected BossBeamCleave _instance;

            public void Initialize(BossBeamCleave bossBeamCleave)
            {
                _instance = bossBeamCleave;
            }

            public abstract void PowerUpBehavior(BossController controller);
        }

        [Serializable, Name("スピードアップ")]
        class SpeedPowerUp : PowerUpBehaviorBase
        {
            [SerializeField, CustomLabel("時間短縮割合")]
            private float _timeReductionRatio;

            public override void PowerUpBehavior(BossController controller)
            {
                _instance._beamTime *= _timeReductionRatio;
            }
        }
    }
}