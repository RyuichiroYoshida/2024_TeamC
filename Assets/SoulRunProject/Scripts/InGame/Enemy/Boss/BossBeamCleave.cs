using System;
using System.Collections.Generic;
using SoulRunProject.Audio;
using SoulRunProject.Common;
using UnityEngine;
using Random = UnityEngine.Random;
using UniRx;
using UnityEngine.SceneManagement;

namespace SoulRunProject.InGame
{
    [Name("ビーム薙ぎ払い")]
    public class BossBeamCleave : BossBehaviorBase
    {
        [SerializeField, CustomLabel("ビームプレハブ")]
        private BossBeamController _beamPrefab;

        [SerializeField, CustomLabel("チャージエフェクト")]
        private ParticleSystem _chargeEffect;

        [SerializeField, CustomLabel("ビーム原点")] private Transform _beamOrigin;

        [SerializeField, CustomLabel("開始着弾地点Z座標(プレイヤーからの相対)")]
        private float _beamStarPosZ;
        
        [SerializeField, CustomLabel("着弾距離")]
        private float _beamHitLength;

        [SerializeField, CustomLabel("ビームのランダム振れ幅")]
        private float _randomRange;

        [SerializeField, CustomLabel("当たるレイヤー")]
        private LayerMask _collisionLayer;

        [Header("性能")] [SerializeField, CustomLabel("薙ぎ払い時間")]
        private float _beamTime;

        [SerializeField, CustomLabel("チャージ時間")]
        private float _chargeDuration;

        [SerializeField, CustomLabel("ダメージ")] 
        private float _damage;

        [Header("強化後性能")] 
        [SerializeField, CustomLabel("ビームの間隔")]
        private float _beamDistance;

        [SerializeField, CustomLabel("強化後チャージ時間")]
        private float _chargeDurationBuffed;

        private BossController _boss;
        private Transform _playerTf;
        private Vector3 _startImpactPosition;
        private Vector3 _finishImpactPosition;
        private float _cleaveTimer;
        private float _chargeTimer;
        private float _currentDistance;
        private Guid _beamSound;
        private readonly List<BossBeamController> _beamControllers = new();

        public override void Initialize(BossController bossController, Transform playerTf)
        {
            // エフェクトインスタンスの初期化
            _beamControllers.Add(GameObject.Instantiate(_beamPrefab, _beamOrigin));
            _chargeEffect = GameObject.Instantiate(_chargeEffect, _beamOrigin);
            _chargeEffect.Stop();
            _boss = bossController;
            _playerTf = playerTf;

            // sound
            PauseManager.IsPause.Subscribe(isPause =>
            {
                if (isPause) CriAudioManager.Instance.Pause(CriAudioType.CueSheet_SE, _beamSound);
                else CriAudioManager.Instance.Resume(CriAudioType.CueSheet_SE, _beamSound);
            });

            SceneManager.sceneLoaded += (arg0, mode) =>
            {
                CriAudioManager.Instance.Stop(CriAudioType.CueSheet_SE, _beamSound);
            };
        }

        public override void BeginAction()
        {
            _cleaveTimer = 0;
            _chargeTimer = 0;

            foreach (var bossBeam in _beamControllers)
            {
                bossBeam.Initialize(_damage, _collisionLayer);
            }
            
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
                    // ビームに間隔を事前に決めておく
                    _currentDistance = (_beamDistance + Random.Range(-_randomRange, _randomRange)) * (_boss.transform.position.x < _startImpactPosition.x ? -1f : 1f);

                    for (int i = 0; i < _beamControllers.Count; i++)
                    {
                        _beamControllers[i].IsActiveBeam = true;
                        _beamControllers[i].SetImpactPoint(_startImpactPosition + _currentDistance * i * Vector3.right);
                    }
                    
                    // sound
                    _beamSound = CriAudioManager.Instance.Play(CriAudioType.CueSheet_SE, "SE_Laser");
                }
            }
            else if (_cleaveTimer < _beamTime)
            {
                for (int i = 0; i < _beamControllers.Count; i++)
                {
                    _beamControllers[i].SetImpactPoint(Vector3.Lerp(_startImpactPosition + _currentDistance * i * Vector3.right, 
                        _finishImpactPosition + _currentDistance * i * Vector3.right, _cleaveTimer / _beamTime));
                }
                
                _cleaveTimer += deltaTime;
            }
            else
            {
                // 終了処理
                foreach (var beam in _beamControllers)
                {
                    beam.IsActiveBeam = false;
                }
                
                CriAudioManager.Instance.Stop(CriAudioType.CueSheet_SE, _beamSound);
                OnFinishAction?.Invoke();
            }

            // 当たり判定
            // if (Physics.Raycast(_beamOrigin.position, _beamOrigin.forward, out RaycastHit hit, float.MaxValue,
            //         _collisionLayer))
            // {
            //     if (hit.collider.gameObject.TryGetComponent(out PlayerManager playerManager))
            //     {
            //         if (!_afterHitting)
            //         {
            //             _afterHitting = true;
            //             playerManager.Damage(_damage);
            //         }
            //     }
            // }
        }

        public override void BuffBehavior(BossController bossController)
        {
            _beamControllers.Add(GameObject.Instantiate(_beamPrefab, _beamOrigin));

            foreach (var particle in _chargeEffect.GetComponentsInChildren<ParticleSystem>())
            {
                var main = particle.main;
                main.simulationSpeed = _chargeDuration / _chargeDurationBuffed;
            }
            
            _chargeDuration = _chargeDurationBuffed;
        }
    }
}