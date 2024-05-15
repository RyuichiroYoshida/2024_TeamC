using SoulRunProject.Common;
using SoulRunProject.InGame;
using SoulRunProject.Framework;
using System.Threading;
using UniRx;
using Cysharp.Threading.Tasks;

namespace SoulRunProject
{
    public class PauseState : State
    {
        private readonly PlayerManager _playerManager;
        private readonly PlayerInput _playerInput;
        private State _lastState;
        private CancellationTokenSource _cts;

        public State StateToReturn => _lastState;
        
        public PauseState(PlayerManager playerManager, PlayerInput playerInput)
        {
            _playerManager = playerManager;
            _playerInput = playerInput;
        }

        protected override void OnEnter(State currentState)
        {
            DebugClass.Instance.ShowLog("ポーズステート開始");
            //停止させる仕組み自体は共通だが、こちらのステートに移行する際にはポーズUIを表示する。
            PauseManager.Instance.Pause(true);
            _lastState = currentState;
            _cts = new CancellationTokenSource();
            
            // PlayerInputへの購読
            _playerInput.PauseInput
                .SkipLatestValueOnSubscribe()
                .Where(x => x)
                .Subscribe( _ =>
                {
                    StateChange();
                })
                .AddTo(_cts.Token);
        }

        protected override void OnExit(State nextState)
        {
            _cts.Cancel();
            _cts.Dispose();
        }
    }
}
