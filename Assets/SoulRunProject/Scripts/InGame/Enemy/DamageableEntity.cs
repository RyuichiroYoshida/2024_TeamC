using System;
using UnityEngine;
using SoulRunProject.Common;
using UniRx;

namespace SoulRunProject.InGame
{
    /// <summary>
    /// 敵や障害物を管理するクラス
    /// </summary>
    public class DamageableEntity : MonoBehaviour
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
        readonly Subject<Unit> _finishedSubject = new();
        public IObservable<Unit> OnFinishedAsync => _finishedSubject.Take(1);
        public Action OnDead;
        public float MaxHp => _maxHp;
        public float CollisionDamage => _collisionDamage;
        public FloatReactiveProperty CurrentHp => _currentHp;
        public bool IsPooled { get; set; }

        void Start()
        {
            Initialize();
        }

        public void Initialize()
        {
            _currentHp.Value = _maxHp;
        }

        /// <summary>
        /// ダメージ処理 + ノックバック処理
        /// </summary>
        public void Damage(float damage, in GiveKnockBack knockBack = null)
        {
            if (!gameObject.activeSelf) return;
            
            _currentHp.Value -= damage;
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

        public void Finish()
        {
            if (IsPooled)
            {
                OnDead = null;
                _finishedSubject.OnNext(Unit.Default);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        void Death()
        {
            if (_lootTable)
            {
                ItemDropManager.Instance.Drop(_lootTable, transform.position);
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