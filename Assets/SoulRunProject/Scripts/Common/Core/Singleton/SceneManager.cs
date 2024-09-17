using Cysharp.Threading.Tasks;
using HikanyanLaboratory.Fade;
using SoulRunProject.Common;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HikanyanLaboratory.SceneManagement
{
    public class SceneManager : AbstractSingletonMonoBehaviour<SceneManager>
    {
        protected override bool UseDontDestroyOnLoad => true;

        private bool _isTransitioning;


        public async UniTask LoadSceneWithFade(string sceneName, IFadeStrategy fadeStrategy = null)
        {
            if (_isTransitioning)
            {
                Debug.LogWarning("シーン遷移が進行中です。新しい遷移は無視されます。");
                return;
            }

            _isTransitioning = true;

            try
            {
                if (FadeController.Instance == null)
                {
                    Debug.LogError("FadeControllerが設定されていません。シーン遷移を中止します。");
                    return;
                }

                fadeStrategy ??= new BasicFadeStrategy();

                // フェードアウト
                await FadeController.Instance.FadeOut(fadeStrategy);

                // シーンを非同期でロード
                var loadSceneOperation =
                    UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
                if (loadSceneOperation != null)
                {
                    loadSceneOperation.allowSceneActivation = false;

                    // ロードが完了するまで進行状況を監視
                    while (loadSceneOperation.progress < 0.9f)
                    {
                        await UniTask.Yield();
                    }

                    // シーンをアクティブにする
                    loadSceneOperation.allowSceneActivation = true;

                    // シーンが完全にアクティブになるまで待機
                    while (!loadSceneOperation.isDone)
                    {
                        await UniTask.Yield();
                    }
                }

                // フェードイン
                await FadeController.Instance.FadeIn(fadeStrategy);
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"シーン遷移中にエラーが発生しました: {ex.Message}");
            }
            finally
            {
                _isTransitioning = false;
            }
        }
    }
}