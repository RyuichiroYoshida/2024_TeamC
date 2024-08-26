using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace HikanyanLaboratory.Fade
{
    [Serializable]
    public class BasicFadeStrategy : IFadeStrategy
    {
        [SerializeField] private Texture _maskTexture;
        [SerializeField] private float _fadeDuration = 1.0f;
        [SerializeField, Range(0, 1)] private float _cutoutRange = 0f;
        [SerializeField] private Ease _ease = Ease.Linear;

        private static readonly int Range1 = Shader.PropertyToID("_Range");
        private static readonly int MaskTex = Shader.PropertyToID("_MaskTex");

        public async UniTask FadeOut(Material fadeMaterial)
        {
            if (_maskTexture != null)
            {
                fadeMaterial.SetTexture(MaskTex, _maskTexture);
            }

            await DOTween.To(() => _cutoutRange, x => _cutoutRange = x, 1, _fadeDuration)
                .OnUpdate(() => fadeMaterial.SetFloat(Range1, 1 - _cutoutRange))
                .SetEase(_ease)
                .ToUniTask();
        }

        public async UniTask FadeIn(Material fadeMaterial)
        {
            if (_maskTexture != null)
            {
                fadeMaterial.SetTexture(MaskTex, _maskTexture);
            }

            await DOTween.To(() => _cutoutRange, x => _cutoutRange = x, 0, _fadeDuration)
                .OnUpdate(() => fadeMaterial.SetFloat(Range1, 1 - _cutoutRange))
                .SetEase(_ease)
                .ToUniTask();
        }
    }
}