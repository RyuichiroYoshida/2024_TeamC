using System.Collections;
using System.Collections.Generic;
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
        [SerializeField] private float _fadeDuration = 0.5f;
        
        public void ShowPopup()
        {
            _popupPanel.color = new Color(255, 255, 255, 0);
            _popupPanel.DOFade(1, _fadeDuration).SetLink(_popupPanel.gameObject);
        }
    }
}
