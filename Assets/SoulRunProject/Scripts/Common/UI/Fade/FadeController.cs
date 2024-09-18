using Cysharp.Threading.Tasks;
using SoulRunProject.Common;
using UnityEngine;

namespace HikanyanLaboratory.Fade
{
    public class FadeController : AbstractSingletonMonoBehaviour<FadeController>
    {
        private Material _fadeMaterial;
        protected override bool UseDontDestroyOnLoad => true;

        /// <summary>
        /// FadeOut用
        /// IFadeStrategyを指定しない場合、FadeViewの設定を優先します
        /// </summary>
        public async UniTask FadeOut(IFadeStrategy fadeStrategy = null)
        {
            if (fadeStrategy != null)
            {
                await fadeStrategy.FadeOut(_fadeMaterial);
            }
        }

        /// <summary>
        /// FadeIn用
        /// IFadeStrategyを指定しない場合、FadeViewの設定を優先します
        /// </summary>
        /// <param name="fadeStrategy"></param>
        public async UniTask FadeIn(IFadeStrategy fadeStrategy = null)
        {
            if (fadeStrategy != null)
            {
                await fadeStrategy.FadeIn(_fadeMaterial);
            }
        }


        /// <summary>
        /// FadeViewのMaterialを取得
        /// </summary>
        /// <param name="material"></param>
        public void SetFadeMaterial(Material material)
        {
            _fadeMaterial = material;
        }
    }
}