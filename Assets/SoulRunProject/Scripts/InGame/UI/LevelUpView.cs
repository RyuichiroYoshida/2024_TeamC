using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using SoulRun.InGame;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SoulRunProject.InGame
{
    public class LevelUpView : MonoBehaviour
    {
        [SerializeField] private GameObject _levelUpPanel;
        [SerializeField] private PopupView _popupView;

        /// <summary> [0]:Skill, [1,2]:Passive </summary>
        [SerializeField] private ButtonAndView[] _upgradeButtons;

        public ButtonAndView[] UpgradeButtons => _upgradeButtons;
        private CancellationTokenSource _cts = new CancellationTokenSource();

        /// <summary>
        /// LevelUpPanelの表示を切り替える
        /// </summary>
        /// <param name="isShow"></param>
        public void OpenLevelUpPanel()
        {
            if (_cts != null)
            {
                _cts.Cancel();
            }
            _levelUpPanel.SetActive(true);
            _popupView.OpenPopup();
            EventSystem.current.SetSelectedGameObject(_upgradeButtons[0].CustomButton.gameObject);
            _upgradeButtons[0].CustomButton.OnSelect(null);
        }
        
        public void CloseLevelUpPanel()
        {
            _cts = new CancellationTokenSource();
            _popupView.ClosePopup(_cts.Token).Forget();
        }

        /// <summary>
        /// Buttonと表示素材をリンク
        /// </summary>
        [Serializable]
        public class ButtonAndView
        {
            [SerializeField] private CustomButtonBase _customButton;
            [SerializeField] private Text _nameAndLevelText;
            [SerializeField] private Text _explanatoryText;
            [SerializeField] private Image _buttonIconImage;
            
            public CustomButtonBase CustomButton => _customButton;
            public Text NameAndLevelText => _nameAndLevelText;
            public Text ExplanatoryText => _explanatoryText;
            public Image ButtonIconImage => _buttonIconImage;
        }
    }
}
