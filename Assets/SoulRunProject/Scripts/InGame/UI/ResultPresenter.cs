using SoulRunProject.Audio;
using SoulRunProject.Common;
using SoulRunProject.Framework;
using VContainer.Unity;
using UniRx;

namespace SoulRunProject.InGame
{
    public class ResultPresenter : IInitializable
    {
        private ResultView _resultView;
        private ResultState _resultState;
        private PlayerManager _playerManager;
        private ScoreData _scoreData;
        
        public ResultPresenter(ResultView resultView, ResultState resultState, PlayerManager playerManager, ScoreData scoreData)
        {
            _resultView = resultView;
            _resultState = resultState;
            _playerManager = playerManager;
            _scoreData = scoreData;
        }

        public void Initialize()
        {
            _resultState.OnStateEnter += _ =>
            {
                _resultView.SetResultPanelVisibility(true);
                var score = ScoreManager.Instance.OnScoreChanged.Value;
                _resultView.DisplayResult(score, _playerManager.ResourceContainer.Coin, _scoreData.GetSprite(score));
                CriAudioManager.Instance.Play(CriAudioType.CueSheet_ME, _playerManager.CurrentHp.Value > 0 ?
                    "ME_Stage_Clear" : "ME_GameOver");
            };
            _resultView.RestartButton.OnClick.Subscribe(_ =>
            {
                DebugClass.Instance.ShowLog("リスタートボタンが押されました。");
                _resultView.SetResultPanelVisibility(false);
                _resultState.RetryStage();
            });
            _resultView.ExitButton.OnClick.Subscribe(_ =>
            {
                _resultView.SetResultPanelVisibility(false);
            });
            
            _resultView.SetResultPanelVisibility(false);
        }
    }
}
