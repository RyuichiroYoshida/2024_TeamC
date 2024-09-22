using HikanyanLaboratory.SceneManagement;
using SoulRunProject.Audio;
using SoulRunProject.Common;
using UnityEngine;

namespace SoulRunProject
{
    public class ThankYouSceneManager : MonoBehaviour
    {
        [SerializeField, CustomLabel("タイトルに遷移するまでの時間")] private float _duration;
        [SerializeField] private string _bgmName;
        [SerializeField] private string _voiceName = "VOICE_GameEnd_1";

        private void Start()
        {
            Invoke(nameof(ToTitle), _duration);
            CriAudioManager.Instance.Play(CriAudioType.CueSheet_BGM, _bgmName);
            CriAudioManager.Instance.Play(CriAudioType.CueSheet_VOICE, _voiceName);
        }

        void ToTitle()
        {
            _ = SceneManager.Instance.LoadSceneWithFade("Title");
        }
    }
}
