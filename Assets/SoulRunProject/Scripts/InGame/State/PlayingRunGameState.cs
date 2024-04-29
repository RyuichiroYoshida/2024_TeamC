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
        private PlayerManager _playerManager;
        private PlayerInput _playerInput;
        private PlayerLevelManager _playerLevelManager;
        private FieldMover _fieldMover;
        private CompositeDisposable _compositeDisposable = new CompositeDisposable();
        
        //TODO：ボスステージ開始前のプレイヤーの位置を設定する場所を検討
        private float _enterBossStagePosition = 111440;
        public bool ArrivedBossStagePosition { get; private set; }
        public bool SwitchToPauseState { get; private set; }
        public bool IsPlayerDead { get; private set; }
        public bool SwitchToLevelUpState { get; private set; }
        
        public PlayingRunGameState(PlayerManager playerManager, PlayerInput playerInput, 
            PlayerLevelManager playerLevelManager, FieldMover fieldMover)
        {
            _playerManager = playerManager;
            _playerInput = playerInput;
            _playerLevelManager = playerLevelManager;
            _fieldMover = fieldMover;
        }
        
        protected override void OnEnter(State currentState)
        {
            DebugClass.Instance.ShowLog("プレイ中ステート開始");
            PauseManager.Pause(false);
            SwitchToLevelUpState = false;
            
            // PlayerInputへの購読
            _playerInput.PauseInput
                .SkipLatestValueOnSubscribe()
                .Subscribe(toPause =>
                {
                    SwitchToPauseState = toPause;
                    if (toPause) StateChange();
                })
                .AddTo(_compositeDisposable);
            
            _playerLevelManager.OnLevelUp
                .SkipLatestValueOnSubscribe()
                .Subscribe(_ =>
                {
                    SwitchToLevelUpState = true;
                    StateChange();
                })
                .AddTo(_compositeDisposable);
            _fieldMover.StartBossStage += StartBossStage;
        }
        
        protected override void OnUpdate()
        { 
            if (_playerManager.CurrentHp.Value <= 0)
            {   //プレイヤーのHPが0になったら遷移
                IsPlayerDead = true;
                StateChange();
            }
        }

        protected override void OnExit(State nextState)
        {
            _compositeDisposable.Clear();
            _fieldMover.StartBossStage -= StartBossStage;
            ArrivedBossStagePosition = false;
        }

        /// <summary>
        /// ボスステージ開始
        /// </summary>
        void StartBossStage()
        {
            PauseManager.Pause(false);
            ArrivedBossStagePosition = true;
            StateChange();
        }
    }
}
