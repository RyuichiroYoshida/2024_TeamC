using System;
using SoulRunProject.Common;
using UniRx;
using UnityEngine;

namespace SoulRunProject.InGame
{
    /// <summary>
    /// Enemyの遠距離攻撃処理の実装クラス
    /// </summary>
    [Serializable, Name("遠距離攻撃")]
    public class EntityLongRangeAttacker : EntityAttacker
    {
        [SerializeField, CustomLabel("弾の生成間隔")]
        private float _interval = 3f;
        [SerializeField, CustomLabel("生成する弾丸")] private EnemyBullet _enemyBullet;
        private CommonObjectPool _bulletPool;
        float _timer;
        bool _isPause;

        public override void OnStart()
        {
            _bulletPool = ObjectPoolManager.Instance.RequestPool(_enemyBullet);
            _timer = _interval;
        }

        public override void OnUpdateAttack(Transform myTransform, Transform playerTransform)
        {
            if (_isPause)
            {
                return;
            }
            if (_interval > _timer)
            {
                _timer += Time.deltaTime;
            }
            else
            {
                _timer = 0;
                var bullet = (EnemyBullet)_bulletPool.Rent();
                bullet.transform.position = myTransform.position + Vector3.back;
                bullet.Initialize();
                bullet.OnFinishedAsync.Take(1).Subscribe(_ => _bulletPool.Return(bullet));
            }
        }

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