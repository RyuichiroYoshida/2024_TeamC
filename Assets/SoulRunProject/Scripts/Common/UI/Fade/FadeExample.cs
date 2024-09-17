using Cysharp.Threading.Tasks;
using SoulRunProject.Common;
using UnityEngine;

namespace HikanyanLaboratory.Fade
{
    public class FadeExample : MonoBehaviour
    {
        [SerializeReference, SubclassSelector, Tooltip("FadeStrategyを設定して下さい")]
        private IFadeStrategy _fadeStrategy;

        private FadeController _fadeController;

        private void Start()
        {
            _fadeController = FadeController.Instance;
            // 何らかの条件でフェードを開始する
            StartFadeExample().Forget();
        }

        private async UniTaskVoid StartFadeExample()
        {
            Debug.Log("FadeOut");
            await _fadeController.FadeOut(_fadeStrategy);
            await UniTask.Delay(1000);
            Debug.Log("FadeIn");
            await _fadeController.FadeIn(_fadeStrategy);
        }
    }
}