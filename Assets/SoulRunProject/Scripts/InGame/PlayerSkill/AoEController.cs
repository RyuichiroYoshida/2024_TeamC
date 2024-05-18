using System.Collections.Generic;
using System.Linq;
using SoulRunProject.InGame;
using UniRx;
using UnityEngine;

namespace SoulRunProject.Common
{
    /// <summary>
    /// 範囲攻撃クラス
    /// </summary>
    public class AoEController : MonoBehaviour, IPausable
    {
        HashSet<DamageableEntity> _entities = new();
        private AoESkillParameter _param;
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

        public void Initialize(in AoESkillParameter param)
        {
            _param = param;
            param.ObserveEveryValueChanged(x => x.Size).Subscribe(x => transform.localScale = Vector3.one * x).AddTo(this);
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
            PauseManager.UnRegisterPausableObject(this);
        }

        public void Pause(bool isPause)
        {
            _isPause = isPause;
        }

        public void Register()
        {
            PauseManager.RegisterPausableObject(this);
        }
    }
}
