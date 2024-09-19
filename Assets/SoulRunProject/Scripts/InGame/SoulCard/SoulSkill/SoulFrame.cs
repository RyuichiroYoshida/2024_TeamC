using System;
using System.Collections.Generic;
using SoulRunProject.Audio;
using SoulRunProject.Common;
using UniRx;
using UniRx.Triggers;
using UnityEngine;


namespace SoulRunProject.InGame
{
    public class SoulFrame : SoulSkillBase
    {
        [SerializeField , CustomLabel("ソウル技プレハブ")] private GameObject _meteorPrefab;
        [SerializeField , CustomLabel("ヒットエフェクトプレハブ")] private GameObject _hitMeteorPrefab;
        [SerializeField , CustomLabel("ヒットエフェクト表示時間")] private float _hitMeteorLifeTime;
        [SerializeField , CustomLabel("地面判定のY座標")] private float _groundPos;
        [SerializeField, CustomLabel("攻撃判定遅延")] private float _attackDelay;
        [SerializeField, CustomLabel("秒間ダメージ")] private int _attackDamage;
        [SerializeField, CustomLabel("ノックバック")] private GiveKnockBack _giveKnockBack;
        [SerializeField, CustomLabel("エフェクト生成箇所")] private Vector3 _relativePosition;
        
        [SerializeField, CustomLabel("攻撃オフセット")] private Vector3 _attackPositionOffset;
        [SerializeField, CustomLabel("攻撃範囲")] private Vector3 _attackArea;
        [SerializeField, CustomLabel("Gizmoの色")] private Color _gizmoColor;
        private ParticleSystem _meteorParticle;
        private float _attackTimer;
        private float _delayTimer;
        private bool _isPause;
        private bool _isAttack;
        readonly Collider[] _hitTargetsBuffer = new Collider[50];
        readonly List<ParticleCollisionEvent> _particleCollisionEvents = new();

        void Start()
        {
            _meteorParticle =  Instantiate(_meteorPrefab , transform).GetComponent<ParticleSystem>();
            _meteorParticle.transform.position += _relativePosition; 
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = _gizmoColor;
            Gizmos.DrawCube(transform.position + _attackPositionOffset ,  _attackArea);
        }

        public override void StartSoulSkill()
        {
            _isAttack = true;
            _meteorParticle.Play();
            _delayTimer = _attackDelay;
            _attackTimer = _duration;
            // 音の再生 パーティクルが火の玉を出しているからこうなる
            Observable.Interval(TimeSpan.FromSeconds(0.1f))
                .Take(25)
                .Subscribe(_ => CriAudioManager.Instance.Play(CriAudioType.CueSheet_SE, "SE_Soulflame"))
                .AddTo(_meteorParticle);
            
        }
        public override void UpdateSoulSkill(float deltaTime)
        {
            
        }

        private void Update()
        {
            if (!_isAttack) return;
            if (_delayTimer >= 0f)
            {
                _delayTimer -= Time.deltaTime;
            }
            else
            {
                if (_attackTimer >= 0f && !_isPause)
                {
                    if (_attackTimer < _attackDelay && _meteorParticle.isPlaying)
                    {
                        _meteorParticle.Stop();
                    }
                    _attackTimer -= Time.deltaTime;
                    Physics.OverlapBoxNonAlloc(transform.position + _attackPositionOffset, _attackArea / 2 , _hitTargetsBuffer);
                    foreach (var hit in _hitTargetsBuffer)
                    {
                        if (hit && hit.TryGetComponent(out DamageableEntity entity))
                        {
                            entity.Damage(_attackDamage * Time.deltaTime , _giveKnockBack);
                        }
                    }
                    ParticleSystem.Particle[] particles = new ParticleSystem.Particle[_meteorParticle.particleCount];
                    _meteorParticle.GetParticles(particles);
                    
                    // 新しいリストに条件を満たすパーティクルを保存
                    int index = 0;
                    for (int i = 0; i < _meteorParticle.particleCount; i++)
                    {
                        // パーティクルの位置が一定の距離を超えているか確認
                        if (particles[i].position.y <= _groundPos)
                        {
                            var hitEffect = Instantiate( _hitMeteorPrefab , particles[i].position , Quaternion.identity , null );
                            Destroy(hitEffect , _hitMeteorLifeTime);
                            particles[i].position = new Vector3(0, 1000, 0);
                        }
                    }
                    _meteorParticle.SetParticles(particles);
                    
                }
                else
                {
                    _isAttack = false;
                }
            }
        }
        public override void PauseSoulSkill(bool isPause)
        {
            _isPause = isPause;
            if (!_meteorParticle) return;
            if (isPause)
            {
                _meteorParticle.Pause();
            }
            else
            {
                _meteorParticle.Play();
            }
        }
    }
}
