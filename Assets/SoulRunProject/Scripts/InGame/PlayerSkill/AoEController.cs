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
    public class AoEController : MonoBehaviour
    {
        HashSet<DamageableEntity> _entities = new();
        private AoESkillParameter _param;
        private PlayerManager _playerManager;

        public void ApplyParameter(in AoESkillParameter param, PlayerManager playerManager)
        {
            _param = param;
            _playerManager = playerManager;
            param.ObserveEveryValueChanged(x => x.Size).Subscribe(x => transform.localScale = Vector3.one * x).AddTo(this);
        }

        void FixedUpdate()
        {
            // OnTriggerExitする前にDestroyすることがあるので、
            // Whereでnullチェックしてからダメージ処理
            foreach (var entity in _entities.Where(entity => entity))
            {
                entity.Damage(_param.BaseAttackDamage * Time.fixedDeltaTime, useSE: false);

                if (entity.TryGetComponent(out EnemyController _)) // 敵に対するヒット数によってもらえるソウルが増える
                {
                    _playerManager.AddSoul(_param.GetSoulPerSec * Time.fixedDeltaTime);
                }
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
