using SoulRunProject.Common;
using SoulRunProject.Framework;
using UnityEngine.SceneManagement;

namespace SoulRunProject.InGame
{
    /// <summary>
    /// ゲーム終了時に行われるステート
    /// </summary>
    public class ResultState : State
    {
        private PlayerManager _playerManager;
        
        public ResultState(PlayerManager playerManager)
        {
            _playerManager = playerManager;
        }
        
        protected override void OnEnter(State currentState)
        {
            DebugClass.Instance.ShowLog("リザルトステート開始");
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
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
