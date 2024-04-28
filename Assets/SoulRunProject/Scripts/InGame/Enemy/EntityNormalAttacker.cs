using System;
using SoulRunProject.Common;
using UnityEngine;

namespace SoulRunProject.InGame
{
    /// <summary>
    /// Enemyの通常攻撃処理の実装クラス
    /// </summary>
    [Serializable, Name("通常攻撃"), RequireComponent(typeof(EnemyBulletShot))]
    public class EntityNormalAttacker : EntityAttacker
    {
        [SerializeField] float _speed;
        [SerializeField] EnemyBulletShot _enemyBulletShot;
        ProjectileSkillParameter _parameter;
        float _timer;
        bool _isPause;

        public override void OnStart()
        {
            _parameter = new ProjectileSkillParameter();
        }

        public override void OnUpdateAttack(Transform myTransform, Transform playerTransform)
        {
            if (_isPause)
            {
                return;
            }

            var param = _parameter;
            param.AttackDamage = _attack;
            param.CoolTime = _coolTime;
            param.Range = _range;
            param.Speed = _speed;
            param.LifeTime = 1000;
            if (param.CoolTime > _timer)
            {
                _timer += Time.deltaTime;
            }
            else
            {
                _timer = 0;
                var bullet = _enemyBulletShot.GenerateBullet(myTransform);
                bullet.Initialize(param);
            }
        }

        // TODO ポーズ処理怪しい
        public override void Pause()
        {
            _isPause = true;
        }

        public override void Resume()
        {
            _isPause = false;
        }
    }
}