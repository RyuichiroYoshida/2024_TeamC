using SoulRunProject.Audio;
using SoulRunProject.Common;
using UnityEngine;
using UniRx;
using Unity.Mathematics;

namespace SoulRunProject.InGame
{
    /// <summary>
    /// PlayerのAnimationを制御する
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public class PlayerAnimationController : MonoBehaviour
    {
        [SerializeField] private GameObject _playerRagdoll;
        [SerializeField] private float _ragdollFallSpeed = 2f;
        [SerializeField] private PlayerMovement _playerMovement;
        [SerializeField] private PlayerManager _playerManager;
        [SerializeField] private float _moveSpeedDivisor = 15f;
        private SpriteRenderer _sr;
        private GameObject _ragdoll;
        private Animator _animator;
        private void Awake()
        {
            _sr = GetComponent<SpriteRenderer>();
            _animator = GetComponent<Animator>();
            _playerManager.ObserveEveryValueChanged(pm => pm.CurrentPlayerStatus.MoveSpeed)
                .Subscribe(speed=> _animator.SetFloat("MoveSpeed", speed / _moveSpeedDivisor)).AddTo(this);
            _playerMovement.IsGround.Subscribe(isGround =>
                {
                    _animator.SetBool("IsGround", isGround);
                })
                .AddTo(this);
            _playerMovement.OnJumped += () => _animator.SetTrigger("OnJump");
            _playerManager.OnDead += () =>
            {
                _ragdoll = Instantiate(_playerRagdoll, transform.position, quaternion.identity);
                _sr.enabled = false;
            };
        }

        private void Update()
        {
            // //  死亡時の演出
            // if (!_playerMovement || !_ragdoll) return;
            //
            // var threshold = _playerMovement.GroundHeight + _playerMovement.DistanceBetweenPivotAndGroundPoint;
            // if (_ragdoll.transform.position.y > threshold)
            // {
            //     _ragdoll.transform.position += Vector3.down * (_ragdollFallSpeed * Time.unscaledDeltaTime);
            // }
            // else if(_ragdoll.transform.position.y < threshold)
            // {
            //     var temp = _ragdoll.transform.position;
            //     temp.y = threshold;
            //     _ragdoll.transform.position = temp;
            // }
        }

        /// <summary>
        /// プレイヤーの足音再生
        /// AnimationEventから呼び出される
        /// </summary>
        public void PlayRunSound()
        {
            CriAudioManager.Instance.Play(CriAudioType.CueSheet_SE, "SE_Run");
        }
    }
}
