using HikanyanLaboratory.SceneManagement;
using SoulRunProject.Audio;
using SoulRunProject.Common;
using SoulRunProject.InGame;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace SoulRunProject
{
    public class ThankYouSceneManager : MonoBehaviour
    {
        // [SerializeField, CustomLabel("タイトルに遷移するまでの時間")]
        // private float _duration;

        [SerializeField] private string _bgmName;
        [SerializeField] private string _voiceName = "VOICE_GameEnd_1";
        [SerializeField] private Button _titleButton;

        private void Start()
        {
            _titleButton.onClick.AddListener(ToTitle);
            ObservePlayerInput();
            CriAudioManager.Instance.Play(CriAudioType.CueSheet_BGM, _bgmName);
            CriAudioManager.Instance.Play(CriAudioType.CueSheet_VOICE, _voiceName);
        }

        void ToTitle()
        {
            _ = SceneManager.Instance.LoadSceneWithFade("Title");
        }

        private void ObservePlayerInput()
        {
            // PlayerInputManagerのPauseInputを監視し、入力があったらタイトル画面に戻る
            PlayerInputManager.Instance.PauseInput
                .Take(1)
                .Subscribe(_ =>
                {
                    ToTitle(); // 任意入力があったらタイトルに戻る
                    
                })
                .AddTo(this);
        }
    }
}