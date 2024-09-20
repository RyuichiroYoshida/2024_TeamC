using System.Collections;
using UnityEngine;
using SoulRun.InGame;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.Universal;

namespace SoulRunProject.Title
{
    public class TitleView : MonoBehaviour
    {
        [SerializeField] private InputUIButtonBase _startButton;
        [SerializeField] private InputUIButtonBase _optionButton;
        [SerializeField] private InputUIButtonBase _exitButton;
        [SerializeField] private InputUIButtonBase _optionCloseButton;
        [SerializeField] private Light2D _soulLight2D;
        [SerializeField] private ParticleSystem _particleSystem;
        [SerializeField] private CanvasGroup _optionPanel;
        [SerializeField] private CanvasGroup _basePanel;
        public InputUIButtonBase StartButton => _startButton;
        public InputUIButtonBase OptionButton => _optionButton;
        public InputUIButtonBase OptionCloseButton => _optionCloseButton;
        public InputUIButtonBase ExitButton => _exitButton;
        public CanvasGroup OptionPanel => _optionPanel;
        public CanvasGroup BasePanel => _basePanel;
        
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