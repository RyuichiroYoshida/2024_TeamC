using SoulRunProject.Audio;
using SoulRunProject.Title;
using UniRx;
using UnityEngine;

namespace SoulRunProject
{
    public class TitlePresenter : MonoBehaviour
    {
        [SerializeField] private TitleModel _titleModel;
        [SerializeField] private TitleView _titleView;

        private void Start()
        {
            CriAudioManager.Instance.Play(CriAudioType.CueSheet_BGM, "BGM_Title", true);
            _titleView.StartButton.OnClick.Subscribe(_ => _titleModel.StartGame()).AddTo(this);
            _titleView.OptionButton.OnClick.Subscribe(_ => _titleModel.OpenOption(_titleView.OptionPanel, _titleView.BasePanel)).AddTo(this);
            _titleView.OptionCloseButton.OnClick.Subscribe(_ => _titleModel.CloseOption(_titleView.OptionPanel, _titleView.BasePanel)).AddTo(this);
            _titleView.ExitButton.OnClick.Subscribe(_ => _titleModel.Exit()).AddTo(this);
            _titleModel.CloseOption(_titleView.OptionPanel, _titleView.BasePanel);
        }
    }
}