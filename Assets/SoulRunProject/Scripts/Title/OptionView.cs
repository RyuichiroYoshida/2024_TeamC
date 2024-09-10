using System.Collections;
using System.Collections.Generic;
using SoulRun.InGame;
using SoulRunProject.InGame;
using UnityEngine;
using UnityEngine.UI;

namespace SoulRunProject
{
    public class OptionView : MonoBehaviour
    {
        [SerializeField] private InputUIButton _resumeButton;
        [SerializeField] private InputUIButton _exitButton;
        [SerializeField] private PopupView _popupView;
        public InputUIButton ResumeButton => _resumeButton;
        public InputUIButton ExitButton => _exitButton;

        public async void DisplayOption()
        {
            await _popupView.OpenResultPopUp(); // ポップアップ表示
        }

        public async void CloseOption()
        {
            var token = new System.Threading.CancellationToken();
            await _popupView.ClosePopup(token); // ポップアップ閉じる
        }
    }
}