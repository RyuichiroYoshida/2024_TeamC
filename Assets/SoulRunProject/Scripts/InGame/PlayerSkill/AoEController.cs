using System;
using System.Collections.Generic;
using System.Linq;
using SoulRunProject.InGame;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace SoulRunProject.Common
{
    /// <summary>
    /// 範囲攻撃クラス
    /// </summary>
    public class AoEController : MonoBehaviour
    {
        HashSet<DamageableEntity> _entities = new();
        private AoESkillParameter _param;

        public void Initialize(in AoESkillParameter param)
        {
            _param = param;
            param.ObserveEveryValueChanged(x => x.Size).Subscribe(x => transform.localScale = Vector3.one * x).AddTo(this);
        }

        void FixedUpdate()
        {
            // OnTriggerExitする前にDestroyすることがあるので、
            // Whereでnullチェックしてからダメージ処理
            foreach (var entity in _entities.Where(entity => entity))
            {
                entity.Damage(_param.AttackDamage * Time.fixedDeltaTime);
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
    }
}
