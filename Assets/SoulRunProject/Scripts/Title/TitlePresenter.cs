using SoulRunProject.Audio;
using SoulRunProject.InGame;
using SoulRunProject.Title;
using UniRx;
using UnityEngine;

namespace SoulRunProject
{
    public class TitlePresenter : MonoBehaviour
    {
        [SerializeField] private TitleModel _titleModel;
        [SerializeField] private TitleView _titleView;
        private SoulRunInput _input;

        private void Start()
        {
            _input = new SoulRunInput();
            _input.Enable();

            _titleView.StartButton.OnClick.Subscribe(_ => _titleModel.StartGame()).AddTo(this);
            _titleView.OptionButton.OnClick
                .Subscribe(_ => _titleModel.OpenOption(_titleView.OptionPanel, _titleView.BasePanel)).AddTo(this);
            _titleView.OptionCloseButton.OnClick
                .Subscribe(_ => _titleModel.CloseOption(_titleView.OptionPanel, _titleView.BasePanel)).AddTo(this);
            _titleView.ExitButton.OnClick.Subscribe(_ => _titleModel.Exit()).AddTo(this);


            _titleModel.CloseOption(_titleView.OptionPanel, _titleView.BasePanel);

            _input.Player.Move.performed += _ => { _titleModel.ResetCountDown(); };
            _input.Player.Jump.performed += _ => { _titleModel.ResetCountDown(); };
            _input.UI.Submit.performed   += _ => { _titleModel.ResetCountDown(); };
            _input.UI.Navigate.performed += _ => { _titleModel.ResetCountDown(); };


            _titleModel.OnDemoMoviePlay.Subscribe(_ => { PlayDemoMovie(); }).AddTo(this);

            _titleModel.IsDemoMoviePlaying
                .Subscribe(isPlaying =>
                {
                    if (!isPlaying)
                    {
                        // Videoが終了したらカウントダウンを再スタートする
                        StopDemoMovie();
                        _titleModel.ResetCountDown();
                    }
                    else
                    {
                        PlayDemoMovie();
                    }
                }).AddTo(this);
        }

        private void PlayDemoMovie()
        {
            _titleView.VideoPlayer.Play();
            _titleModel.IsDemoMoviePlaying.Value = true;
            _titleModel.OpenMovie(_titleView.VideoPanel);
        }

        private void StopDemoMovie()
        {
            _titleView.VideoPlayer.Stop();
            _titleModel.IsDemoMoviePlaying.Value = false;
            _titleModel.CloseMovie(_titleView.VideoPanel);
        }

        private void OnDestroy()
        {
            // InputSystemを無効化してリソースを解放
            _input.Disable();
        }
    }
}