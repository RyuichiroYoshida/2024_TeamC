using UnityEngine;
using Cinemachine;
using SoulRunProject.InGame;

namespace SoulRunProject
{
 
    [ExecuteInEditMode] [SaveDuringPlay] [AddComponentMenu("")] // Hide in menu
    public class LockCameraY : CinemachineExtension
    {
        private PlayerMovement _playerMovement;
        [Tooltip("カメラのY座標を固定する値")]
        public float m_ZPosition = 10;
        public float m_YPosition = 10;
        [SerializeField] private float _maxJumpedDistance;

        protected override void Awake()
        {
            base.Awake();
            _playerMovement = FindObjectOfType<PlayerMovement>();
        }

        protected override void PostPipelineStageCallback(
            CinemachineVirtualCameraBase vcam,
            CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
        {
            if (stage == CinemachineCore.Stage.Body)
            {
                var pos = state.RawPosition;
                //pos.y =  _playerMovement.GroundHeight + m_YPosition;

                if (_playerMovement.IsJumping)
                {
                    pos.y = Mathf.Max(_playerMovement.GroundHeight + m_YPosition, _playerMovement.transform.position.y - _maxJumpedDistance);
                }
                else
                {
                    pos.y = _playerMovement.transform.position.y + m_YPosition - 1.5f; // 1.5 => player pivot distance
                }
                
                state.RawPosition = pos;
            }
        }
    }
}

