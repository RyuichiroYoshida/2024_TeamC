using System.Threading;
using Cysharp.Threading.Tasks;
using HikanyanLaboratory.Fade;
using SoulRunProject.Common;
using SoulRunProject.Framework;
using SoulRunProject.InGame;
using UniRx;
using ActionMapType = SoulRunProject.InGame.PlayerInputManager.ActionMapType;
using SceneManager = HikanyanLaboratory.SceneManagement.SceneManager;

namespace SoulRunProject
{
    public class PauseState : State
    {
        private readonly PlayerManager _playerManager;
        private readonly PlayerInputManager _playerInputManager;
        private State _lastState;
        private CancellationTokenSource _cts;

        public State StateToReturn => _lastState;
        
        public PauseState(PlayerManager playerManager, PlayerInputManager playerInputManager, PauseView pauseView)
        {
            _playerManager = playerManager;
            _playerInputManager = playerInputManager;
            pauseView.ResumeButton.OnClick.Subscribe(_ =>
            {
                StateChange();
            }).AddTo(pauseView);
        }

        protected override void OnEnter(State currentState)
        {
            DebugClass.Instance.ShowLog("ポーズステート開始");
            //停止させる仕組み自体は共通だが、こちらのステートに移行する際にはポーズUIを表示する。
            PauseManager.Pause(true);
            _playerInputManager.SwitchActionMap(ActionMapType.UI);
            _lastState = currentState;
            _cts = CancellationTokenSource.CreateLinkedTokenSource(_playerManager.destroyCancellationToken);
            
            // PlayerInputへの購読
            _playerInputManager.PauseInput
                .Subscribe(_ =>
                {
                    StateChange();
                })
                .AddTo(_cts.Token);
        }

        public async UniTaskVoid ExitToTitle()
        {
            PauseManager.Pause(false);
            await SceneManager.Instance.LoadSceneWithFade("Title");
            //  アクションマップをUIに変更する。
            PlayerInputManager.Instance.SwitchActionMap(ActionMapType.UI);
        }

        protected override void OnExit(State nextState)
        {
            _cts.Cancel();
            _playerInputManager.SwitchActionMap(ActionMapType.Player);
        }
    }
}
