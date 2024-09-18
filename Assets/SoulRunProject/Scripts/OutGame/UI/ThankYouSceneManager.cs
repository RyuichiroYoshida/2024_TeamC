using SoulRunProject.Audio;
using SoulRunProject.Common;
using UnityEngine;
using UnityEngine.SceneManagement;

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
            SceneManager.LoadScene(0);
        }
    }
}
