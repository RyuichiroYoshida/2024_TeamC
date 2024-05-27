using SoulRunProject.Common;
using UnityEngine;
using UniRx;

namespace SoulRunProject.InGame
{
    /// <summary>
    /// PlayerのAnimationを制御する
    /// </summary>
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(PlayerMovement))]
    public class PlayerAnimationController : MonoBehaviour
    {
        private void Awake()
        {
            PlayerManager playerManager = GetComponent<PlayerManager>();
            PlayerMovement playerMovement = GetComponent<PlayerMovement>();
            Animator playerAnimator = GetComponent<Animator>();

            playerMovement.IsGround.Subscribe(isGround =>
                {
                    playerAnimator.SetBool("IsGround", isGround);
                })
                .AddTo(this);
            playerMovement.OnJumped += () => playerAnimator.SetTrigger("OnJump");
            playerManager.OnDead += () => playerAnimator.SetTrigger("OnDead");
        }
    }
}
