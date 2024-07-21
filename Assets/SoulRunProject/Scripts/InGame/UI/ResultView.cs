using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using SoulRun.InGame;
using SoulRunProject.Framework;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

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
            _restartButton.OnClickAsObservable().Subscribe(_ => DebugClass.Instance.ShowLog("リスタートボタンが押されました。"));
        }

        /// <summary>
        /// リザルト画面の表示非表示を設定する
        /// </summary>
        /// <param name="isShow"></param>
        public void SetResultPanelVisibility(bool isShow)
        {
            _resultPanel.SetActive(isShow);
        }
        
        /// <summary>
        /// 上から順番にスコア、コイン、ランク、ハイスコアを表示する。
        /// </summary>
        /// <param name="score"></param>
        /// <param name="coin"></param>
        public async void ShowResult(int score, int coin)
        {
            var targetScore = score.ToString();
            var targetCoin = coin.ToString();
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
            sequence.Append(DOTween.To(() => int.Parse(_scoreText.text), x => _scoreText.text = x.ToString(), score, 1f));
            sequence.AppendCallback(() => _coinTitleText.gameObject.SetActive(true));
            sequence.AppendInterval(0.5f);
            sequence.AppendCallback(() => _coinText.gameObject.SetActive(true));
            sequence.Append(DOTween.To(() => int.Parse(_coinText.text), x => _coinText.text = x.ToString(), coin, 1f));
            sequence.AppendCallback(() => _highScoreTitleText.gameObject.SetActive(true));
            sequence.AppendInterval(0.5f);
            sequence.AppendCallback(() => _highScoreText.gameObject.SetActive(true));
            sequence.Append(DOTween.To(() => int.Parse(_highScoreText.text), x => _highScoreText.text = x.ToString(), score > targetHighScore ? score : targetHighScore, 1f));
            // ランク表示のイメージをアクティブにする
            sequence.AppendCallback(() => _rankImage.gameObject.SetActive(true));
            sequence.Play().SetUpdate(true).SetLink(this.gameObject);
        }
        
        public enum ResultType
        {
            Clear,
            GameOver,
        }
    }
}
