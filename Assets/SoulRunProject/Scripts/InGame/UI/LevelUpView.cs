using System;
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

        /// <summary>
        /// LevelUpPanelの表示を切り替える
        /// </summary>
        /// <param name="isShow"></param>
        public void OpenLevelUpPanel()
        {
            Debug.Log("OpenLevelUpPanel");
            _levelUpPanel.SetActive(true);
            _popupView.OpenPopup();
            EventSystem.current.SetSelectedGameObject(_upgradeButtons[0].InputUIButton.gameObject);
            _upgradeButtons[0].InputUIButton.OnSelect(null);
        }
        
        public async UniTask CloseLevelUpPanel()
        {
            Debug.Log("CloseLevelUpPanel");
            await _popupView.ClosePopup();
            _levelUpPanel.SetActive(false);
        }

        /// <summary>
        /// Buttonと表示素材をリンク
        /// </summary>
        [Serializable]
        public class ButtonAndView
        {
            [SerializeField] private InputUIButton _inputUIButton;
            [SerializeField] private Text _nameAndLevelText;
            [SerializeField] private Text _explanatoryText;
            [SerializeField] private Image _buttonIconImage;
            
            public InputUIButton InputUIButton => _inputUIButton;
            public Text NameAndLevelText => _nameAndLevelText;
            public Text ExplanatoryText => _explanatoryText;
            public Image ButtonIconImage => _buttonIconImage;
        }
    }
}
