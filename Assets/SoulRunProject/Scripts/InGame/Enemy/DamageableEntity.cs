using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using SoulRunProject.Common;
using SoulRunProject.SoulMixScene;
using UniRx;

namespace SoulRunProject.InGame
{
    /// <summary>
    /// 敵や障害物を管理するクラス
    /// </summary>
    public class DamageableEntity : MonoBehaviour
    {
        [SerializeField , Header("HP")] private float _maxHp = 30;
        [SerializeField, Header("衝突ダメージ")] private float _collisionDamage;
        [SerializeField, Header("ノックバック方向")] private Vector3 _direction = Vector3.one;
        [SerializeField ,Header("ノックバック処理")] TakeKnockBack _takeKnockBack;
        [SerializeField, Header("ドロップデータ")] LootTable _lootTable;
        [SerializeField, Header("ダメージエフェクト")] HitDamageEffectManager _hitDamageEffectManager;

        public float MaxHp => _maxHp;
        public float CollisionDamage => _collisionDamage;
        public FloatReactiveProperty CurrentHp => _currentHp;

        private FloatReactiveProperty _currentHp = new();
        private float _knockBackResistance;
        public Action OnDead;

        private void Awake()
        {
            _currentHp.Value = _maxHp;
        }

        /// <summary>
        /// ダメージ処理 + ノックバック処理
        /// </summary>
        public void Damage(float damage , in GiveKnockBack knockBack = null)
        {
            _currentHp.Value -= damage;
            CriAudioManager.Instance.PlaySE(CriAudioManager.CueSheet.Se, "SE_Hit");
            if (_currentHp.Value <= 0)
            {
                Death();
            }
            if (knockBack != null)
            {
                _takeKnockBack.KnockBack(transform , knockBack.Power , _direction);
            }
            if (_hitDamageEffectManager)
            {
                _hitDamageEffectManager.HitFadeBlinkWhite();
            }
        }

        void Death()
        {
            if (_lootTable)
            {
                ItemDropManager.Instance.Drop(_lootTable, transform.position);
            }
            OnDead?.Invoke();
            Destroy(gameObject);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.TryGetComponent(out PlayerManager playerManager))
            {
                playerManager.Damage(_collisionDamage);
            }
        }
    }
}