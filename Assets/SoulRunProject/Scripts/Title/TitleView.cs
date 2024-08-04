using System;
using System.Collections;
using UnityEngine;
using SoulRun.InGame;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.Universal;

namespace SoulRunProject.Title
{
    public class TitleView : MonoBehaviour
    {
        [SerializeField] private InputUIButton _startButton;
        [SerializeField] private InputUIButton _optionButton;
        [SerializeField] private InputUIButton _exitSimButton;
        [SerializeField] private InputUIButton _returnButton;
        [SerializeField] private Light2D _soulLight2D;
        [SerializeField] private ParticleSystem _particleSystem;
        [SerializeField] private GameObject _optionPanel;
        public InputUIButton StartButton => _startButton;
        public InputUIButton OptionButton => _optionButton;
        public InputUIButton ExitButton => _exitSimButton;
        public InputUIButton ReturnButton => _returnButton;
        
        public GameObject OptionPanel => _optionPanel;
        
        public float minFalloffIntensity = 0.5f;
        public float maxFalloffIntensity = 1f;
        public float duration = 2.0f;

        private void Start()
        {
            EventSystem.current.SetSelectedGameObject(_startButton.gameObject);
            _startButton.OnSelect(null);
            Debug.Log(_particleSystem);
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