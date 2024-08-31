using Cysharp.Threading.Tasks;
using SoulRunProject.Common;
using UnityEngine;

namespace HikanyanLaboratory.Fade
{
    public class FadeExample : MonoBehaviour
    {
        private FadeController _fadeController;
        [SerializeReference, SubclassSelector] IFadeStrategy _fadeStrategy;

        private void Start()
        {
            _fadeController = FadeController.Instance;
            if (_fadeController == null)
            {
                Debug.LogError("FadeControllerが見つかりません。");
            }

            // 何らかの条件でフェードを開始する
            StartFadeExample().Forget();
        }

        private async UniTaskVoid StartFadeExample()
        {
            if (_fadeController == null)
            {
                Debug.LogError("FadeControllerが設定されていません。");
                return;
            }

            // フェードアウト
            Debug.Log("Fade out started.");
            await _fadeController.FadeOut(_fadeStrategy);

            // ここでシーンの切り替えや他の処理を行います
            await UniTask.Delay(1000); // 1秒の遅延を挟む

            // フェードイン
            Debug.Log("Fade in started.");
            await _fadeController.FadeIn(_fadeStrategy);

            Debug.Log("Fade completed.");
        }
    }
}