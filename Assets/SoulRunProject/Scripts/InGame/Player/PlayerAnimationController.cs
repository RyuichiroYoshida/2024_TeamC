using SoulRunProject.Common;
using UnityEngine;
using UniRx;

namespace SoulRunProject.InGame
{
    /// <summary>
    /// PlayerのAnimationを制御する
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public class PlayerAnimationController : MonoBehaviour
    {
        [SerializeField] private PlayerMovement _playerMovement;
        [SerializeField] private PlayerManager _playerManager;
        private void Awake()
        {
            Animator playerAnimator = GetComponent<Animator>();

            _playerMovement.IsGround.Subscribe(isGround =>
                {
                    playerAnimator.SetBool("IsGround", isGround);
                })
                .AddTo(this);
            _playerMovement.OnJumped += () => playerAnimator.SetTrigger("OnJump");
            _playerManager.OnDead += () => playerAnimator.SetTrigger("OnDead");
        }
        /// <summary>
        /// プレイヤーの足音再生
        /// AnimationEventから呼び出される
        /// </summary>
        public void PlayRunSound()
        {
            CriAudioManager.Instance.PlaySE("SE_Run");
        }
    }
}
