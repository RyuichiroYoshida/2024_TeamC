using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using SoulRunProject.Audio;
using SoulRunProject.Common;
using SoulRunProject.Runtime;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace SoulRunProject.InGame
{
    /// <summary>
    /// Enemyの遠距離攻撃処理の実装クラス
    /// </summary>
    [Serializable]
    [Name("遠距離攻撃")]
    public class EntityLongRangeAttacker : EntityAttacker
    {
        [SerializeField, CustomLabel("弾の生成間隔")] private float _interval = 3f;
        [SerializeField, CustomLabel("生成する弾丸")] private EnemyBullet _enemyBullet;
        [SerializeField, CustomLabel("追尾の有効化")] private bool _isHoming;
        [SerializeField, CustomLabel("攻撃した際の移動硬直時間")] private float _moveStopTime = 0.2f;
        [SerializeField, ShowWhenBoolean(nameof(_isHoming)), CustomLabel("追尾時間")] private float _homingTime;
        [SerializeField,  CustomLabel("攻撃アニメーション名")] private string _attackAnimState;
        [SerializeField, CustomLabel("アニメーション時間")] float _animationTime = 0.25f;
        private CommonObjectPool _bulletPool;
        private float _moveStopTimer;
        private float _timer;
        [NonSerialized] public bool CanMove = true;

        public override void OnStart(Transform myTransform)
        {
            _bulletPool = ObjectPoolManager.Instance.RequestPool(_enemyBullet);
            _timer = _interval;
            _moveStopTimer = _moveStopTime;
            CancellationTokenSource cts = new();
            cts = CancellationTokenSource.CreateLinkedTokenSource(cts.Token, myTransform.GetCancellationTokenOnDestroy());
            Fire(myTransform, cts.Token).Forget();
            myTransform.gameObject.OnDisableAsObservable().Subscribe(_ =>
            {
                cts?.Cancel();
                CanMove = true;
            }).AddTo(myTransform);
        }

        public override void OnUpdateAttack(Transform myTransform, Transform playerTransform)
        {
        }

        async UniTask Fire(Transform myTransform, CancellationToken token)
        {
            while (true)
            {
                await UniTask.WaitForSeconds(_interval, delayTiming: PlayerLoopTiming.Update, cancellationToken: token);
                StopMove(token).Forget();
                await PlayAnimation(token);
                CriAudioManager.Instance.Play(CriAudioType.CueSheet_SE, "SE_HowkAttack");
                var bullet = (EnemyBullet)_bulletPool.Rent();
                bullet.transform.position = myTransform.position + Vector3.back;
                bullet.InitializeHoming(_isHoming, _homingTime);
                bullet.Initialize();
                _moveStopTimer = 0;
                
                bullet.OnFinishedAsync.Take(1).Subscribe(_ => _bulletPool.Return(bullet));
            }
        }

        async UniTask PlayAnimation(CancellationToken token)
        {
            Animator.Play(_attackAnimState);
            await UniTask.WaitForSeconds(_animationTime, delayTiming: PlayerLoopTiming.Update, cancellationToken: token);
        }

        async UniTaskVoid StopMove(CancellationToken token)
        {
            CanMove = false;
            await UniTask.WaitForSeconds(_moveStopTime, delayTiming: PlayerLoopTiming.Update, cancellationToken: token);
            CanMove = true;
        }
    }
}