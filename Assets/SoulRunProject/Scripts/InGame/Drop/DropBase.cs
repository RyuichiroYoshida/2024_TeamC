using DG.Tweening;
using SoulRunProject.Common;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SoulRunProject.InGame
{
    /// <summary>
    /// ドロップした経験値やアイテムの基底クラス
    /// </summary>
    public abstract class DropBase : PooledObject
    {
        [SerializeField, CustomLabel("跳ねるアニメーションの乗数(増やすとより大きく跳ねる)")] float _multiplier = 1f;
        [SerializeField, CustomLabel("跳ねるアニメーションの時間")] float _projectileMotionTime = 0.5f;
        [SerializeField, CustomLabel("1回転する時間")] float _rotateTime = 0.5f;
        [SerializeField, CustomLabel("出現する時の高さ。ワールド座標。")] float _dropHeight = 0.7f;
        private Sequence _projectileMotionSequence;
        private Tween _rotateTween;
        private bool _isPause;
        private PlayerManager _player;
        private FieldMover _fieldMover;
        public MyConstraint Parent { get; set; }
        public bool ActiveParent { get; set; }
        
        /// <summary>プレイヤーがドロップ品を拾った時に呼ぶ処理</summary>
        protected abstract void PickUp(PlayerManager playerManager);

        protected new void OnDestroy()
        {
            base.OnDestroy();
        }

        public override void Initialize()
        {
            RandomProjectileMotion();
        }

        /// <summary>
        /// 参照を割り当てる
        /// </summary>
        public void ApplyReference(PlayerManager player, FieldMover fieldMover)
        {
            _player = player;
            _fieldMover = fieldMover;
        }

        /// <summary>
        /// ランダムな方向に斜方投射するメソッド
        /// </summary>
        private void RandomProjectileMotion()
        {
            // 現在の斜方投射が終わるまで次の投射を行わない
            if (_projectileMotionSequence != null)　return;
            //  最終的に同じ高さで止まるので空中に出現しても地面から始めるようにする
            Vector3 newPosition = transform.position;
            newPosition.y = _dropHeight;
            transform.position = newPosition;
            
            Vector3 randomDir = new Vector3(Random.Range(-1f, 1f), Random.Range(0.5f, 1f), Random.Range(-1f, 1f));
            randomDir *= _multiplier;
            _projectileMotionSequence = DOTween.Sequence();
            _projectileMotionSequence.Append(transform.DOBlendableMoveBy(new Vector3(randomDir.x, 0f ,randomDir.z), _projectileMotionTime));
            _projectileMotionSequence.Insert(0f , transform.DOBlendableMoveBy(new Vector3(0f , randomDir.y ,0f), _projectileMotionTime / 2));
            _projectileMotionSequence.Insert(_projectileMotionTime / 2 , transform.DOBlendableMoveBy(new Vector3(0f , - randomDir.y ,0f), _projectileMotionTime / 2));
            _projectileMotionSequence.SetLink(gameObject).SetLink(gameObject, LinkBehaviour.KillOnDisable).OnComplete(()=>_projectileMotionSequence = null);
        }

        private void Update()
        {
            if (_isPause) return;
            //  プレイヤーより後ろに行ったら吸引処理を行わない
            if (_player.transform.position.z > transform.position.z)
            {
                //  フィールド追従処理に自身を追加する
                if (Parent && !ActiveParent)
                {
                    Parent.Targets.Add(transform);
                    ActiveParent = true;
                }
                return;
            }
            
            var attractRange = _player.CurrentPlayerStatus.AttractItemRange;
            var distance = _player.transform.position - transform.position;
            float attractSpeed = _player.CurrentPlayerStatus.MoveSpeed + attractRange;
            if (distance.sqrMagnitude < attractRange * attractRange)
            {
                //  引き寄せられているときはフィールド追従処理から除外する
                if (Parent && ActiveParent)
                {
                    Parent.Targets.Remove(transform);
                    ActiveParent = false;
                }
                transform.position += distance.normalized * (attractSpeed * Time.deltaTime);
            }
        }

        private void OnEnable()
        {
            _rotateTween = transform.DORotate(new Vector3(0,360,0),_rotateTime, RotateMode.WorldAxisAdd)
                .SetEase(Ease.Linear).SetLoops(-1, LoopType.Restart).SetLink(gameObject).SetLink(gameObject, LinkBehaviour.KillOnDisable);
        }

        private void OnDisable()
        {
            _rotateTween?.Kill();
            _rotateTween = null;
            _projectileMotionSequence?.Kill();
            _projectileMotionSequence = null;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.TryGetComponent(out PlayerManager playerManager))
            {
                PickUp(playerManager);
            }
        }
        
    }
}
