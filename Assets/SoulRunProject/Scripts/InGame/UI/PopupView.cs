using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace SoulRunProject.InGame
{
    /// <summary>
    /// ポーズ時、レベルアップ時、リザルト時に表示するポップアップの処理を書いたクラス
    /// </summary>
    public class PopupView : MonoBehaviour
    {
        [SerializeField] private Image _popupPanel;
        [SerializeField] private GameObject _popupContent;
        [SerializeField] private float _fadeDuration = 0.1f;
        [SerializeField] private float _maxHeight = 2169;
        [SerializeField] private float _minHeight = 865;
        [SerializeField] private float _sizeChangeDuration = 0.2f;
        [SerializeField] Ease _popupEase = Ease.OutQuad;
        private float _centerbackMaxSizeY = 1;
        private float _centerbackMinSizeY = 0.1f;

        public void OpenPopup()
        {
            //下準備
            _popupPanel.color = new Color(255, 255, 255, 0);
            _popupPanel.rectTransform.sizeDelta = new Vector2(_popupPanel.rectTransform.sizeDelta.x, _minHeight);
            _popupPanel.gameObject.SetActive(true);
            Vector2 newSize = new Vector2(_popupPanel.rectTransform.sizeDelta.x, _maxHeight);
            _popupContent.gameObject.SetActive(false);
            //アニメーション
            var sequence = DOTween.Sequence();
            sequence.Append(_popupPanel.DOFade(1, _fadeDuration).SetLink(_popupPanel.gameObject))
                .SetUpdate(true); // フェードイン
            sequence.Append(_popupPanel.rectTransform.DOSizeDelta(newSize, _sizeChangeDuration)
                    .SetLink(_popupPanel.gameObject)).SetUpdate(true)
                .OnComplete(() => _popupContent.SetActive(true)); //画像の立幅を指定した秒数かけて変更
            sequence.Play().SetUpdate(true);
        }

        public async UniTask ClosePopup(CancellationToken token)
        {
            //下準備
            Vector2 newSize = new Vector2(_popupPanel.rectTransform.sizeDelta.x, _minHeight);
            _popupPanel.rectTransform.sizeDelta = new Vector2(_popupPanel.rectTransform.sizeDelta.x, _maxHeight);
            _popupContent.gameObject.SetActive(false);
            //アニメーション
            var sequence = DOTween.Sequence();
            sequence.Append(_popupPanel.rectTransform.DOSizeDelta(newSize, _sizeChangeDuration));//画像の立幅を指定した秒数かけて変更
            sequence.Append(_popupPanel.DOFade(0, _fadeDuration));// フェードアウト
            sequence.Play().SetUpdate(true)
                .OnComplete(() => gameObject.SetActive(false))
                .OnKill(() =>
                {
                    _popupPanel.DOComplete();
                    gameObject.SetActive(false);
                })
                .ToUniTask(cancellationToken: token);
        }

        //リザルト用に使います
        public async UniTask OpenResultPopUp()
        {
            //_popupPanel.color = new Color(255, 255, 255, 0);
            _popupPanel.rectTransform.sizeDelta = new Vector2(_popupPanel.rectTransform.sizeDelta.x, _minHeight);
            _popupPanel.gameObject.SetActive(true);
            Vector2 newSize = new Vector2(_popupPanel.rectTransform.sizeDelta.x, _maxHeight);
            _popupContent.gameObject.SetActive(false);
            //アニメーション
            var sequence = DOTween.Sequence();
            sequence.Append(_popupPanel.DOFade(1, _fadeDuration).SetLink(_popupPanel.gameObject))
                .SetUpdate(true); // フェードイン
            sequence.Append(_popupPanel.rectTransform.DOSizeDelta(newSize, _sizeChangeDuration)
                    .SetLink(_popupPanel.gameObject)).SetEase(_popupEase).SetUpdate(true)
                .OnComplete(() => _popupContent.SetActive(true)); //画像の立幅を指定した秒数かけて変更
            await sequence.Play().SetUpdate(true).ToUniTask();
        }
    }
}