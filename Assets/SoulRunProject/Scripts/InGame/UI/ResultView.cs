using Cysharp.Threading.Tasks;
using DG.Tweening;
using SoulRun.InGame;
using SoulRunProject.Framework;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UnityEngine.EventSystems;

namespace SoulRunProject.InGame
{
    /// <summary>
    /// リザルトのUI関連の処理を行うクラス
    /// </summary>
    public class ResultView : MonoBehaviour
    {
        [SerializeField] private InputUIButton _restartButton;
        [SerializeField] private InputUIButton _exitButton;
        [SerializeField] private GameObject _resultPanel;
        [SerializeField] private Text _scoreText;
        [SerializeField] private Text _scoreTitleText;
        [SerializeField] private Text _coinText;
        [SerializeField] private Text _coinTitleText;
        [SerializeField] private Text _highScoreText;
        [SerializeField] private Text _highScoreTitleText;
        [SerializeField] private PopupView _popupView;
        [SerializeField] private Image _rankImage;
        
        public InputUIButton RestartButton => _restartButton;
        public InputUIButton ExitButton => _exitButton;

        private void Start()
        {
            _restartButton.OnClick.Subscribe(_ => DebugClass.Instance.ShowLog("リスタートボタンが押されました。"));
        }
        
        /// <summary>
        /// リザルト画面の表示非表示を設定する
        /// </summary>
        /// <param name="isShow"></param>
        public async void SetResultPanelVisibility(bool isShow)
        {
            //await UniTask.Delay(TimeSpan.FromSeconds(_displayDelayTime), DelayType.UnscaledDeltaTime);
            _resultPanel.SetActive(isShow);
        }
        
        /// <summary>
        /// 上から順番にスコア、コイン、ランク、ハイスコアを表示する。
        /// </summary>
        /// <param name="score"></param>
        /// <param name="coin"></param>
        public async void DisplayResult(int score, int coin)
        {
            var targetScore = score;
            var targetCoin = coin;
            var targetHighScore = PlayerPrefs.GetInt("HighScore", 0);
            _scoreText.text = "0";
            _coinText.text = "0";
            _highScoreText.text = "0";
            _scoreText.gameObject.SetActive(false);
            _scoreTitleText.gameObject.SetActive(false);
            _coinText.gameObject.SetActive(false);
            _coinTitleText.gameObject.SetActive(false);
            _highScoreText.gameObject.SetActive(false);
            _highScoreTitleText.gameObject.SetActive(false);

            _popupView.gameObject.SetActive(true);
            await _popupView.OpenResultPopUp();
            var sequence = DOTween.Sequence();
            sequence.AppendCallback(() => _scoreTitleText.gameObject.SetActive(true));
            sequence.AppendInterval(0.5f);
            sequence.AppendCallback(() => _scoreText.gameObject.SetActive(true));
            sequence.Append(DOTween.To(() => int.Parse(_scoreText.text), x => _scoreText.text = x.ToString(), targetScore, 1f));
            sequence.AppendCallback(() => _coinTitleText.gameObject.SetActive(true));
            sequence.AppendInterval(0.5f);
            sequence.AppendCallback(() => _coinText.gameObject.SetActive(true));
            sequence.Append(DOTween.To(() => int.Parse(_coinText.text), x => _coinText.text = x.ToString(), targetCoin, 1f));
            sequence.AppendCallback(() => _highScoreTitleText.gameObject.SetActive(true));
            sequence.AppendInterval(0.5f);
            sequence.AppendCallback(() => _highScoreText.gameObject.SetActive(true));
            sequence.Append(DOTween.To(() => int.Parse(_highScoreText.text), x => _highScoreText.text = x.ToString(), targetScore > targetHighScore ? targetScore : targetHighScore, 1f));
            // ランク表示のイメージをアクティブにする
            sequence.AppendCallback(() => _rankImage.gameObject.SetActive(true));
            sequence.Play().SetUpdate(true).SetLink(this.gameObject).ToUniTask();
            await sequence;
            EventSystem.current.SetSelectedGameObject(_restartButton.gameObject);
            _restartButton.OnSelect(null);
        }
        
        public enum ResultType
        {
            Clear,
            GameOver,
        }
    }
}
