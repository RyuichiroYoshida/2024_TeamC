using Cysharp.Threading.Tasks;
using SoulRunProject.Common;
using UnityEngine;

namespace HikanyanLaboratory.Fade
{
    public class FadeController : AbstractSingletonMonoBehaviour<FadeController>
    {
        private IFadeStrategy _fadeStrategy;
        private Material _fadeMaterial;
        protected override bool UseDontDestroyOnLoad => true;

        public async UniTask FadeOut(IFadeStrategy fadeStrategy = null)
        {
            // Debug.Log("Starting FadeOut");
            fadeStrategy ??= _fadeStrategy;
            if (fadeStrategy != null)
            {
                await fadeStrategy.FadeOut(_fadeMaterial);
            }

            // Debug.Log("FadeOut complete");
        }

        public async UniTask FadeIn(IFadeStrategy fadeStrategy = null)
        {
            // Debug.Log("Starting FadeIn");
            fadeStrategy ??= _fadeStrategy;
            if (fadeStrategy != null)
            {
                await fadeStrategy.FadeIn(_fadeMaterial);
            }

            // Debug.Log("FadeIn complete");
        }


        public void SetFadeMaterial(Material fadeMaterial)
        {
            _fadeMaterial = fadeMaterial;
        }

        public void SetFadeStrategy(IFadeStrategy fadeStrategy)
        {
            _fadeStrategy = fadeStrategy;
        }
    }
}