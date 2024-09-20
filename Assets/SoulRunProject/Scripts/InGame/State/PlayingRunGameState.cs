using System.Threading;
using Cysharp.Threading.Tasks;
using SoulRunProject.Common;
using SoulRunProject.Framework;
using UniRx;

namespace SoulRunProject.InGame
{
    /// <summary>
    /// ランゲームプレイ中の管理を行うクラス
    /// </summary>
    public class PlayingRunGameState : State
    {
        private StageManager _stageManager;
        private PlayerManager _playerManager;
        private PlayerInputManager _playerInputManager;
        private PlayerLevelManager _playerLevelManager;
        private CompositeDisposable _compositeDisposable = new CompositeDisposable();
        
        public bool ArrivedBossStagePosition { get; private set; }
        public bool SwitchToPauseState { get; private set; }
        public bool IsPlayerDead { get; private set; }
        public bool SwitchToLevelUpState { get; private set; }
        public PlayingRunGameState(StageManager stageManager, PlayerManager playerManager, 
            PlayerInputManager playerInputManager, PlayerLevelManager playerLevelManager)
        {
            _stageManager = stageManager;
            _playerManager = playerManager;
            _playerInputManager = playerInputManager;
            _playerLevelManager = playerLevelManager;
        }
        
        protected override void OnEnter(State currentState)
        {
            DebugClass.Instance.ShowLog("プレイ中ステート開始");
            PauseManager.Pause(false);
            SwitchToLevelUpState = false;
            ArrivedBossStagePosition = false;
            SwitchToPauseState = false;

            var cts = CancellationTokenSource.CreateLinkedTokenSource(_playerManager.destroyCancellationToken);
            // PlayerInputへの購読
            _playerInputManager.PauseInput
                .Subscribe(_ =>
                {
                    SwitchToPauseState = true;
                    StateChange();
                }).AddTo(_compositeDisposable).AddTo(cts.Token);
            
            // レベルアップの購読
            _playerLevelManager.OnLevelUp
                .SkipLatestValueOnSubscribe()
                .Subscribe(_ =>
                {
                    SwitchToLevelUpState = true;
                    StateChange();
                })
                .AddTo(_compositeDisposable);
            //プレイヤーのHPの監視
             _playerManager.OnDead += () =>
             {
                 IsPlayerDead = true;
                 StateChange();
             };
            
            // ボスステージへの遷移への購読
            _stageManager.ToBossStage += ToBossStage;
        }
        
        protected override void OnUpdate()
        { 
            if (_playerManager.CurrentHp.Value <= 0)
            {   //プレイヤーのHPが0になったら遷移
                IsPlayerDead = true;
                StateChange();
            }
            _stageManager.PlayingRunGameUpdate();
        }

        protected override void OnExit(State nextState)
        {
            _compositeDisposable.Clear();
            ArrivedBossStagePosition = false;
            _stageManager.ToBossStage -= ToBossStage;
        }

        void ToBossStage()
        {
            ArrivedBossStagePosition = true;
            StateChange();
        }
    }
}
