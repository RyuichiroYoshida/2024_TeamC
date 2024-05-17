using SoulRunProject.Common;

namespace SoulRunProject.InGame
{
    public class LevelUpState : State
    {
        private PlayerManager _playerManager;
        private State _lastState;

        public State StateToReturn => _lastState;
        
        public LevelUpState(PlayerManager playerManager)
        {
            _playerManager = playerManager;
        }

        protected override void OnEnter(State currentState)
        {
            _lastState = currentState;
            PauseManager.Instance.Pause(true);
        }

        public void SelectedSkill()
        {
            PauseManager.Instance.Pause(false);
            StateChange();
        }
    }
}
