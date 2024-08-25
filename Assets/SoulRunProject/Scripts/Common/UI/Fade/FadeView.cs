using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using HikanyanLaboratory.Fade;
using UnityEngine;
using UnityEngine.UI;

namespace GameJamProject.SceneManagement
{
    public class FadeView : MonoBehaviour
    {
        [SerializeField] private GameObject _loadingUI;
        [SerializeField] private Material _fadeMaterial;
        [SerializeField] private Texture _maskTexture;
        [SerializeField] private Slider _slider;
        private FadeController _fadeController;
        private static readonly int MaskTex = Shader.PropertyToID("_MaskTex");

        public Material FadeMaterial => _fadeMaterial;

        private void Awake()
        {
            _fadeController = FadeController.Instance;
            DontDestroyOnLoad(this);

            if (_fadeController != null)
            {
                _fadeController.SetFadeMaterial(_fadeMaterial);
            }
            else
            {
                Debug.LogError("FadeControllerが見つかりません。");
            }

            if (_fadeMaterial != null && _maskTexture != null)
            {
                _fadeMaterial.SetTexture(MaskTex, _maskTexture);
            }
        }

        public void SetLoadingUIActive(bool isActive)
        {
            _loadingUI.SetActive(isActive);
        }

        public async UniTask FadeOut(IFadeStrategy fadeStrategy = null)
        {
            if (_fadeController != null)
            {
                await _fadeController.FadeOut(fadeStrategy);
            }
        }

        public async UniTask FadeIn(IFadeStrategy fadeStrategy = null)
        {
            if (_fadeController != null)
            {
                await _fadeController.FadeIn(fadeStrategy);
            }
        }

        public void UpdateProgress(float progress)
        {
            _slider.value = progress;
        }
    }
}