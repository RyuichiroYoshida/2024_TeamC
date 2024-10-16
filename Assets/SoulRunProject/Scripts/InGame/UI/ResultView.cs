using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using HikanyanLaboratory.SceneManagement;
using SoulRun.InGame;
using SoulRunProject.Common;
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
        //[SerializeField, CustomLabel("リスタート")] private TweenButton _restartButton;
        [SerializeField, CustomLabel("終了")] private TweenButton _exitButton;
        [SerializeField, CustomLabel("リザルトパネル")] private GameObject _resultPanel;
        [SerializeField, CustomLabel("スコア数値表示")] private Text _scoreText;
        [SerializeField, CustomLabel("スコア文字表示")] private Text _scoreTitleText;
        [SerializeField, CustomLabel("コイン数値表示")] private Text _coinText;
        [SerializeField, CustomLabel("コイン文字表示")] private Text _coinTitleText;
        [SerializeField, CustomLabel("ハイスコア数値表示")] private Text _highScoreText;
        [SerializeField, CustomLabel("ハイスコア文字表示")] private Text _highScoreTitleText;
        [SerializeField, CustomLabel("リザルトパネルポップアップ")] private PopupView _popupView;
        [SerializeField, CustomLabel("ランク表示")] private Image _rankImage;
        [SerializeField] private float _duration;
        
        // public TweenButton RestartButton => _restartButton;
        public TweenButton ExitButton => _exitButton;

        // private void Start()
        // {
        //     _restartButton.OnClick.Subscribe(_ => DebugClass.Instance.ShowLog("リスタートボタンが押されました。"));
        // }
        
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
        public async UniTaskVoid DisplayResult(int score, int coin, Sprite rankSprite)
        {
            if (_rankImage)
            {
                _rankImage.sprite = rankSprite;
                _rankImage.enabled = false;
            }
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

            _rankImage.transform.localScale = new Vector3(0, 0, 0); 
            
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
            
            
            sequence.AppendCallback(() =>
            {
                _rankImage.enabled = true;
                _rankImage.transform.localScale = new Vector3(2.5f, 2.5f, 2.5f);
                _rankImage.transform.localRotation = Quaternion.Euler(0, 0, 0);
                _rankImage.color = new Color(_rankImage.color.r, _rankImage.color.g, _rankImage.color.b, 0); // 透明にする
            });
            // スケールダウンアニメーション
            sequence.Append(_rankImage.transform.DOScale(1.2f, 1f).SetEase(Ease.OutSine)); // スケールダウン
            //sequence.Join(_rankImage.transform.DORotate(new Vector3(0, 0, 360), 1f, RotateMode.FastBeyond360).SetEase(Ease.OutSine)); // 回転
            sequence.Append(_rankImage.DOFade(1, 0.5f)); // フェードイン
            
            sequence.AppendCallback(() => _rankImage.enabled = true);
            sequence.Play().SetUpdate(true).SetLink(gameObject).ToUniTask();
            await sequence;
            EventSystem.current.SetSelectedGameObject(_exitButton.gameObject);
            _exitButton.OnSelect(null);
            var ctSource = CancellationTokenSource.CreateLinkedTokenSource(destroyCancellationToken);
            // exitボタン、または時間でシーン遷移
            await UniTask.WhenAny(UniTask.WaitForSeconds(_duration, ignoreTimeScale: true, cancellationToken: ctSource.Token), _exitButton.OnClick.First().ToUniTask(cancellationToken: ctSource.Token));
            ctSource.Cancel();
            await SceneManager.Instance.LoadSceneWithFade("ThankYouForPlaying");
        }
        
        public enum ResultType
        {
            Clear,
            GameOver,
        }
    }
}
