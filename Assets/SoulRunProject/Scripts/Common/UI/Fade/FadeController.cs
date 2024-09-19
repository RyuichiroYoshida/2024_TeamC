using Cysharp.Threading.Tasks;
using SoulRunProject.Common;
using UnityEngine;

namespace HikanyanLaboratory.Fade
{
    public class FadeController : AbstractSingletonMonoBehaviour<FadeController>
    {
        [SerializeField] private Material _fadeMaterial;
        protected override bool UseDontDestroyOnLoad => true;

        public async UniTask FadeOut(IFadeStrategy fadeStrategy = null)
        {
            if (fadeStrategy != null)
            {
                await fadeStrategy.FadeOut(_fadeMaterial);
            }
            else
            {
                Debug.LogWarning("FadeStrategy is null");
            }
            // Debug.Log("FadeOut complete");
        }

        public async UniTask FadeIn(IFadeStrategy fadeStrategy = null)
        {
            if (fadeStrategy != null)
            {
                await fadeStrategy.FadeIn(_fadeMaterial);
            }
            else
            {
                Debug.LogWarning("FadeStrategy is null");
            }
            // Debug.Log("FadeIn complete");
        }
    }
}