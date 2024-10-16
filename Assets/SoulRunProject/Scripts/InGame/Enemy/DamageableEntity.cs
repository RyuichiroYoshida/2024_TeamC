using System;
using Cysharp.Threading.Tasks;
using SoulRunProject.Audio;
using SoulRunProject.Common;
using SoulRunProject.Runtime;
using UniRx;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace SoulRunProject.InGame
{
    /// <summary>
    /// 敵や障害物を管理するクラス
    /// </summary>
    public class DamageableEntity : PooledObject
    {
        [SerializeField, CustomLabel("HP")] private float _maxHp = 30;

        [SerializeField, CustomLabel("衝突ダメージ")]
        private float _collisionDamage;

        [SerializeField, CustomLabel("ノックバックするかどうか")]
        private bool _canKnockback = true;

        [SerializeField, CustomLabel("ノックバック方向"), ShowWhenBoolean(nameof(_canKnockback))]
        private Vector3 _direction = Vector3.one;

        [SerializeField, CustomLabel("ノックバック処理"), ShowWhenBoolean(nameof(_canKnockback))]
        private TakeKnockBack _takeKnockBack;

        [SerializeField, CustomLabel("ドロップデータ")]
        private LootTable _lootTable;

        [SerializeField, CustomLabel("ダメージエフェクト")]
        private HitDamageEffectManager _hitDamageEffectManager;

        [SerializeField] private bool _useHitSe = true;

        private FloatReactiveProperty _currentHp = new();
        private EnemyController _enemyController;
        private Collider _hitCollider;

        private float _knockBackResistance;

        private PlayerManager _player;
        private DecalProjector _decalProjector;

        /// <typeparam name="ダメージ"></typeparam>
        /// <typeparam name="クリティカルかどうか"></typeparam>
        public Action<float, bool> OnDamaged;

        /// <summary> 衝突ダメージを与えたとき </summary>
        public event Action<PlayerManager> OnCollisionDamage; 

        public Action OnDead;
        public float MaxHp => _maxHp;
        public float CollisionDamage => _collisionDamage;
        public FloatReactiveProperty CurrentHp => _currentHp;
        public bool IsEnemy => _enemyController;

        private void Start()
        {
            _player = FindObjectOfType<PlayerManager>();
            _enemyController = GetComponent<EnemyController>();
            _hitCollider = GetComponent<Collider>();
            _decalProjector = GetComponentInChildren<DecalProjector>();
            Initialize();
        }

        public override void Initialize()
        {
            CurrentHp.Value = _maxHp;
            if(_enemyController) _enemyController.Initialize().Forget();
        }

        /// <summary>
        /// ダメージ処理 + ノックバック処理
        /// </summary>
        public void Damage(float damage, in GiveKnockBack knockBack = null, bool useSE = true)
        {
            if (!gameObject.activeSelf) return;
            if (!_player) return;
            
            bool isCritical = false;
            damage = damage * UnityEngine.Random.Range(0.8f , 1.2f) * (1.0f + _player.PlayerCurrentLevel / 10f);
            var calculatedDamage = Calculator.CalcDamage(damage, 0, _player.CurrentPlayerStatus.CriticalRate,
                _player.CurrentPlayerStatus.CriticalDamageRate, ref isCritical);
            CurrentHp.Value -= calculatedDamage;
            OnDamaged?.Invoke(calculatedDamage, isCritical);

            if (useSE && _useHitSe) CriAudioManager.Instance.Play(CriAudioType.CueSheet_SE, "SE_Hit");

            if (CurrentHp.Value <= 0) _ = Death();

            if (knockBack != null && _canKnockback) _takeKnockBack.KnockBack(transform, knockBack.Power, _direction);

            if (_hitDamageEffectManager) _hitDamageEffectManager.HitFadeBlinkWhite();
        }

        public override void OnFinish()
        {
            OnDead = null;
        }

        public async UniTask Death()
        {
            if (_lootTable) DropManager.Instance.RequestDrop(_lootTable, transform.position);
            
            OnDead?.Invoke();
            // 死んでからの演出の時間当たり判定を無くす
            _hitCollider.enabled = false;
            if(_decalProjector) _decalProjector.enabled = false;
            if (_hitDamageEffectManager) await _hitDamageEffectManager.DissolveFade();
            _hitCollider.enabled = true;
            if(_decalProjector) _decalProjector.enabled = true;
            Finish();
        }

        public void Despawn()
        {
            Finish();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!gameObject.activeSelf) return;

            if (other.gameObject.TryGetComponent(out PlayerManager playerManager))
            {
                playerManager.Damage(_collisionDamage);
                OnCollisionDamage?.Invoke(playerManager);
            }
        }
    }
}