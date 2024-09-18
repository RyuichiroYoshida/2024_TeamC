using SoulRunProject.Common;
using ActionMapType = SoulRunProject.InGame.PlayerInputManager.ActionMapType;

namespace SoulRunProject.InGame
{
    public class LevelUpState : State
    {
        private PlayerManager _playerManager;
        private PlayerInputManager _playerInputManager;
        private State _lastState;

        public State StateToReturn => _lastState;
        
        public LevelUpState(PlayerManager playerManager, PlayerInputManager playerInputManager)
        {
            _playerManager = playerManager;
            _playerInputManager = playerInputManager;
        }

        protected override void OnEnter(State currentState)
        {
            _lastState = currentState;
            PauseManager.Pause(true);
            _playerInputManager.SwitchActionMap(ActionMapType.UI);
        }

        public void EndSelectSkill()
        {
            _playerInputManager.SwitchActionMap(ActionMapType.Player);
            //PauseManager.Pause(false);
            StateChange();
        }
    }
}
