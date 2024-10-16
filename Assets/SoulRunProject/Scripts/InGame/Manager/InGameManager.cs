using Cysharp.Threading.Tasks;
using SoulRunProject.Framework;
using SoulRunProject.InGame;
using UnityEngine;
using VContainer.Unity;

namespace SoulRunProject.Common
{
    /// <summary>
    /// インゲームの進行を管理するクラス
    /// </summary>
    public class InGameManager : StateMachine, IStartable, ITickable
    {
        private EnterInGameState _enterInGameState;
        private EnterStageState _enterStageState;
        private PlayingRunGameState _playingRunGameState;
        private EnterBossStageState _enterBossStageState;
        private PlayingBossStageState _playingBossStageState;
        private ResultState _resultState;
        private PauseState _pauseState;
        private LevelUpState _levelUpState;
        
        public InGameManager( GameObject owner, 
            EnterInGameState firstState,
            EnterStageState enterStageState,
            PlayingRunGameState playingRunGameState,
            EnterBossStageState enterBossStageState,
            PlayingBossStageState playingBossStageState,
            ResultState resultState,
            PauseState pauseState,
            LevelUpState levelUpState,
            ExitInGameState exitInGameState)
        {   //ステートの追加、遷移処理の設定を行う。
            _currentState = firstState;
            _owner = owner;
            AddState(0, firstState);
            AddState(1, enterStageState);
            AddState(2, playingRunGameState);
            AddState(4, enterBossStageState);
            AddState(5, playingBossStageState);
            AddState(6, resultState);
            AddState(7, pauseState);
            AddState(8, levelUpState);
            AddState(9, exitInGameState);
            firstState.OnStateExit += _ => ChangeState(1);
            enterStageState.OnStateExit += _ => ChangeState(2);
            playingRunGameState.OnStateExit += _ =>
            {   
                if (playingRunGameState.ArrivedBossStagePosition) //Playerが生きていて、ボスステージに移行する場合
                    ChangeState(4);
                else if (playingRunGameState.SwitchToPauseState) // PauseStateへの移行
                    ChangeState(7);
                else if (playingRunGameState.SwitchToLevelUpState) // LevelUpStateへの移行
                    ChangeState(8);
                else if (playingRunGameState.IsPlayerDead) // プレイヤーが死んだ
                    ChangeState(9);
            };
            pauseState.OnStateExit += _ =>
            {
                if (pauseState.StateToReturn == playingRunGameState)
                {
                    ChangeState(2);
                }
                else if (pauseState.StateToReturn == playingBossStageState)
                {
                    ChangeState(5);
                }
            };
            levelUpState.OnStateExit += _ =>
            {
                if (levelUpState.StateToReturn == playingRunGameState)
                {
                    ChangeState(2);
                }
                else if (levelUpState.StateToReturn == playingBossStageState)
                {
                    ChangeState(5);
                }
            };
            enterBossStageState.OnStateExit += _ => ChangeState(5);
            playingBossStageState.OnStateExit += state =>
            {
                if (playingBossStageState.IsBossDefeated) //ボスを倒した場合
                    ChangeState(9);
                else if (playingBossStageState.SwitchToPauseState) // PauseStateへの移行
                    ChangeState(7);
                else if (playingBossStageState.SwitchToLevelUpState) // LevelUpStateへの移行
                    ChangeState(8);
                else if (playingBossStageState.IsPlayerDead) //プレイヤーが死んだ場合
                    ChangeState(9);
            };
            exitInGameState.OnStateExit += state =>
            {
                ChangeState(6);
            };
        }

        public void Start()
        {
            DebugClass.Instance.ShowLog("InGameManager起動");
            var token = _owner.GetCancellationTokenOnDestroy();
            _currentState.EnterAsync(null, token).Forget();
        }

        public void Tick()
        {
            _currentState.Update();
        }
    }
}
