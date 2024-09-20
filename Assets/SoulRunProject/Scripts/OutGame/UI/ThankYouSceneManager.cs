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

        private void Start()
        {
            Invoke(nameof(ToTitle), _duration);
            CriAudioManager.Instance.Play(CriAudioType.CueSheet_BGM, _bgmName);
        }

        void ToTitle()
        {
            _ = SceneManager.Instance.LoadSceneWithFade("Title");
        }
    }
}
