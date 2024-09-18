using SoulRunProject.Common;
using SoulRunProject.Framework;
using UnityEngine.SceneManagement;
using ActionMapType = SoulRunProject.InGame.PlayerInputManager.ActionMapType;

namespace SoulRunProject.InGame
{
    /// <summary>
    /// ゲーム終了時に行われるステート
    /// </summary>
    public class ResultState : State
    {
        private PlayerManager _playerManager;
        private readonly PlayerInputManager _playerInputManager;
        
        public ResultState(PlayerManager playerManager, PlayerInputManager playerInputManager)
        {
            _playerManager = playerManager;
            _playerInputManager = playerInputManager;
        }
        
        protected override void OnEnter(State currentState)
        {
            DebugClass.Instance.ShowLog("リザルトステート開始");
            _playerInputManager.SwitchActionMap(ActionMapType.UI);
            PauseManager.Pause(true);
        }

        public void ExitToTitle()
        {
            PauseManager.Pause(false);
            // タイトルへ遷移
            SceneManager.LoadScene(0);
        }

        public void ExitToThankYouForPlaying()
        {
            PauseManager.Pause(false);
            // 遷移
            SceneManager.LoadScene(6);
        }

        public void RetryStage()
        {
            _playerInputManager.SwitchActionMap(ActionMapType.Player);
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
