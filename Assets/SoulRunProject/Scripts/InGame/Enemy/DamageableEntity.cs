using System;
using Cysharp.Threading.Tasks;
using SoulRunProject.Audio;
using SoulRunProject.Common;
using SoulRunProject.Runtime;
using UniRx;
using UnityEngine;

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

        [SerializeField, CustomLabel("Dead演出")]
        private DissolveController _dissolveController;

        private FloatReactiveProperty _currentHp = new();
        private EnemyController _enemyController;
        private Collider _hitCollider;

        private float _knockBackResistance;

        private PlayerManager _player;

        /// <typeparam name="ダメージ"></typeparam>
        /// <typeparam name="クリティカルかどうか"></typeparam>
        public Action<float, bool> OnDamaged;

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
            Initialize();
        }

        public override void Initialize()
        {
            CurrentHp.Value = _maxHp;
            if (_enemyController) _enemyController.Initialize();
        }

        /// <summary>
        /// ダメージ処理 + ノックバック処理
        /// </summary>
        public void Damage(float damage, in GiveKnockBack knockBack = null, bool useSE = true)
        {
            if (!gameObject.activeSelf) return;
            if (!_player) return;
            bool isCritical = false;
            var calculatedDamage = Calculator.CalcDamage(damage, 0, _player.CurrentPlayerStatus.CriticalRate,
                _player.CurrentPlayerStatus.CriticalDamageRate, ref isCritical);
            CurrentHp.Value -= calculatedDamage;
            OnDamaged?.Invoke(calculatedDamage, isCritical);

            if (useSE) CriAudioManager.Instance.Play(CriAudioType.CueSheet_SE, "SE_Hit");

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
            if (_dissolveController) await _dissolveController.DissolveFade();
            _hitCollider.enabled = true;
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
                playerManager.Damage(_collisionDamage);
        }
    }
}