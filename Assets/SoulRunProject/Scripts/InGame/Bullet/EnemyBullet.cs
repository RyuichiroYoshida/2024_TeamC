using SoulRunProject.InGame;
using UnityEngine;

namespace SoulRunProject.Common
{
    public class EnemyBullet : BulletBase
    {
        [SerializeField, CustomLabel("弾速")] private float _speed = 5f;
        [SerializeField, CustomLabel("攻撃力")] private float _attackDamage = 5f;
        public override void Move()
        {
            if (_isPause) return;
            transform.position +=  Vector3.back * (_speed * Time.fixedDeltaTime);
        }
        void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.TryGetComponent(out PlayerManager player))
            {
                player.Damage(_attackDamage);
                OnHit(other);
            }
        }

    }
}