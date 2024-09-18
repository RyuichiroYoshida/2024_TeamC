using Cysharp.Threading.Tasks;
using DG.Tweening;
using SoulRunProject.Common;
using UnityEngine;
using UnityEngine.UI;

namespace HikanyanLaboratory.Fade
{
    public class FadeController : AbstractSingletonMonoBehaviour<FadeController>
    {
        [SerializeField] private Image _fadeImage;
        private static readonly int Range1 = Shader.PropertyToID("_Range");
        private static readonly int MaskTex = Shader.PropertyToID("_MaskTex");
        protected override bool UseDontDestroyOnLoad => true;

        /// <summary>
        /// FadeOut用
        /// IFadeStrategyを指定しない場合、FadeViewの設定を優先します
        /// </summary>
        public async UniTask FadeOut(float duration, Ease ease)
        {
            var material = _fadeImage.material;
            // フェードアウト中にRangeを更新しながらアルファをフェード
            await DOTween.To(() => material.GetFloat(Range1), 
                x => material.SetFloat(Range1, x), 0f, duration).SetEase(ease);
        }

        /// <summary>
        /// FadeIn用
        /// IFadeStrategyを指定しない場合、FadeViewの設定を優先します
        /// </summary>
        /// <param name="fadeStrategy"></param>
        public async UniTask FadeIn(float duration, Ease ease)
        {
            var material = _fadeImage.material;
            // フェードイン中にRangeを更新しながらアルファをフェード
            await DOTween.To(() => material.GetFloat(Range1), 
                x => material.SetFloat(Range1, x), 1f, duration).SetEase(ease);
        }
    }
}