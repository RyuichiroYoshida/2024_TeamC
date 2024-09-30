using System.Collections;
using UnityEngine;
using SoulRun.InGame;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.Universal;
using UnityEngine.Video;

namespace SoulRunProject.Title
{
    public class TitleView : MonoBehaviour
    {
        [SerializeField] private CustomButtonBase _startButton;
        [SerializeField] private CustomButtonBase _optionButton;
        [SerializeField] private CustomButtonBase _exitButton;
        [SerializeField] private CustomButtonBase _optionCloseButton;
        [SerializeField] private Light2D _soulLight2D;
        [SerializeField] private ParticleSystem _particleSystem;
        [SerializeField] private CanvasGroup _optionPanel;
        [SerializeField] private CanvasGroup _basePanel;
        [SerializeField] private CanvasGroup _videoPanel;
        [SerializeField] private VideoPlayer _videoPlayer;
        public CustomButtonBase StartButton => _startButton;
        public CustomButtonBase OptionButton => _optionButton;
        public CustomButtonBase OptionCloseButton => _optionCloseButton;
        public CustomButtonBase ExitButton => _exitButton;
        public CanvasGroup OptionPanel => _optionPanel;
        public CanvasGroup BasePanel => _basePanel;

        public CanvasGroup VideoPanel => _videoPanel;
        public VideoPlayer VideoPlayer => _videoPlayer;

        public float minFalloffIntensity = 0.5f;
        public float maxFalloffIntensity = 1f;
        public float duration = 2.0f;

        private void Start()
        {
            EventSystem.current.SetSelectedGameObject(_startButton.gameObject);
            _startButton.OnSelect(null);
            //Debug.Log(_particleSystem);
            _particleSystem.Play();
            StartCoroutine(AnimateLightFalloff());
        }

        private IEnumerator AnimateLightFalloff()
        {
            while (true)
            {
                float time = 0;
                while (time < duration)
                {
                    _soulLight2D.intensity = Mathf.Lerp(minFalloffIntensity, maxFalloffIntensity, time / duration);
                    time += Time.deltaTime;
                    yield return null;
                }

                time = 0;
                while (time < duration)
                {
                    _soulLight2D.intensity = Mathf.Lerp(maxFalloffIntensity, minFalloffIntensity, time / duration);
                    time += Time.deltaTime;
                    yield return null;
                }
            }
        }
    }
}