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
        [SerializeField] private GameObject _optionPanel;
        [SerializeField] private InputUIButton _returnButton;
        [SerializeField] private PopupView _popupView;
        public InputUIButton ReturnButton => _returnButton;
        public GameObject OptionPanel => _optionPanel;
    }
}