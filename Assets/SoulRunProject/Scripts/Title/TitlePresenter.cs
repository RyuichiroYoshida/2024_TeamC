using System;
using System.Collections;
using System.Collections.Generic;
using SoulRunProject.Audio;
using SoulRunProject.Common;
using SoulRunProject.Title;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace SoulRunProject
{
    public class TitlePresenter : MonoBehaviour
    {
        [SerializeField] private TitleModel _titleModel;
        [SerializeField] private TitleView _titleView;
        [SerializeField] private OptionPresenter _optionPresenter;
        private void Start()
        {
            CriAudioManager.Instance.Play(CriAudioType.CueSheet_BGM, "BGM_Title", true);
            _titleView.StartButton.OnClick.Subscribe(_ => _titleModel.StartGame());
            _titleView.OptionButton.OnClick.Subscribe(_ => _titleModel.Option(_titleView.OptionPanel));
            _titleView.ExitButton.OnClick.Subscribe(_ => _titleModel.Exit());
            _titleView.ReturnButton.OnClick.Subscribe(_ => _titleModel.Return(_titleView.OptionPanel));
        }
    }
}