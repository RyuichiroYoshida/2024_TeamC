using System;
using Cysharp.Threading.Tasks;
using SoulRunProject.Common;
using UnityEngine;

namespace SoulRunProject.InGame
{
    public class ExitInGameState : State
    {
        private PlayerManager _playerManager;
        private StageManager _stageManager;
        
        public ExitInGameState(PlayerManager playerManager, StageManager stageManager)
        {
            _playerManager = playerManager;
            _stageManager = stageManager;
        }
        
        protected override void OnEnter(State currentState)
        {
            PauseManager.Pause(true);
            _ = DelayToResultState();
        }

        private async UniTask DelayToResultState()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(_playerManager.CurrentHp.Value <= 0 ? 
                _playerManager.DeadAnimationTime : _stageManager.CurrentBoss? _stageManager.CurrentBoss.DeadAnimationTime : 0),
                ignoreTimeScale: true);
            StateChange();
        }
    }
}
