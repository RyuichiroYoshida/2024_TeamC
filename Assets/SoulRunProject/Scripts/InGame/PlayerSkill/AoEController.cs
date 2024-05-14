using System.Collections.Generic;
using System.Linq;
using SoulRunProject.InGame;
using UnityEngine;

namespace SoulRunProject.Common
{
    /// <summary>
    /// 範囲攻撃クラス
    /// </summary>
    public class AoEController : MonoBehaviour, IPausable
    {
        HashSet<DamageableEntity> _entities = new();
        float _attackDamage;
        private bool _isPause;

        private void Awake()
        {
            Register();
        }

        private void OnDestroy()
        {
            UnRegister();
        }

        public void Initialize(float attackDamage, float range)
        {
            _attackDamage = attackDamage;
            transform.localScale = new Vector3(range, range, range);
        }

        void FixedUpdate()
        {
            if (_isPause) return;
            // OnTriggerExitする前にDestroyすることがあるので、
            // Whereでnullチェックしてからダメージ処理
            foreach (var entity in _entities.Where(entity => entity))
            {
                entity.Damage(_attackDamage * Time.fixedDeltaTime, useSE: false);
            }
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out DamageableEntity entity))
            {
                _entities.Add(entity);
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out DamageableEntity entity))
            {
                _entities.Remove(entity);
            }
        }

        public void UnRegister()
        {
            PauseManager.Instance.UnRegisterPausableObject(this);
        }

        public void Pause(bool isPause)
        {
            _isPause = isPause;
        }

        public void Register()
        {
            PauseManager.Instance.RegisterPausableObject(this);
        }
    }
}
