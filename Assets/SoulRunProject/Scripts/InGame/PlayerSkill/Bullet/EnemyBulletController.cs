using SoulRunProject.InGame;
using UnityEngine;

namespace SoulRunProject.Common
{
    public class EnemyBulletController : BulletController
    {
        public override void Move()
        {
            transform.position += Vector3.back * (_speed * Time.fixedDeltaTime);
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