using System;
using UnityEngine;
using SoulRunProject.Common;
using UniRx;

namespace SoulRunProject.InGame
{
    /// <summary>
    /// 敵や障害物を管理するクラス
    /// </summary>
    public class DamageableEntity : PooledObject
    {
        [SerializeField, CustomLabel("HP")] float _maxHp = 30;
        [SerializeField, CustomLabel("衝突ダメージ")]
        float _collisionDamage;
        [SerializeField, CustomLabel("ノックバック方向")]
        Vector3 _direction = Vector3.one;
        [SerializeField, CustomLabel("ノックバック処理")]
        TakeKnockBack _takeKnockBack;
        [SerializeField, CustomLabel("ドロップデータ")]
        LootTable _lootTable;
        [SerializeField, CustomLabel("ダメージエフェクト")]
        HitDamageEffectManager _hitDamageEffectManager;
        FloatReactiveProperty _currentHp = new();
        float _knockBackResistance;
        public Action OnDead;
        public float MaxHp => _maxHp;
        public float CollisionDamage => _collisionDamage;
        public FloatReactiveProperty CurrentHp => _currentHp;
        private PlayerManager _player;

        void Start()
        {
            Initialize();
            _player = FindObjectOfType<PlayerManager>();
        }

        public override void Initialize()
        {
            _currentHp.Value = _maxHp;
        }

        /// <summary>
        /// ダメージ処理 + ノックバック処理
        /// </summary>
        public void Damage(float damage, in GiveKnockBack knockBack = null)
        {
            if (!gameObject.activeSelf) return;

            float calcedDamage = Calculator.CalcDamage(damage,
                0, _player.CurrentStatus.CriticalRate, _player.CurrentStatus.CriticalDamageRate);
            _currentHp.Value -= calcedDamage;
            CriAudioManager.Instance.PlaySE(CriAudioManager.CueSheet.Se, "SE_Hit");
            
            if (_currentHp.Value <= 0)
            {
                Death();
            }

            if (knockBack != null)
            {
                _takeKnockBack.KnockBack(transform, knockBack.Power, _direction);
            }

            if (_hitDamageEffectManager)
            {
                _hitDamageEffectManager.HitFadeBlinkWhite();
            }
        }

        public override void OnFinish()
        {
            OnDead = null;
        }

        void Death()
        {
            if (_lootTable)
            {
                ItemDropManager.Instance.RequestDrop(_lootTable, transform.position);
            }

            OnDead?.Invoke();
            Finish();
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.TryGetComponent(out PlayerManager playerManager))
            {
                playerManager.Damage(_collisionDamage);
            }
        }
    }
}