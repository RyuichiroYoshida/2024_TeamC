using Cysharp.Threading.Tasks;
using DG.Tweening;
using HikanyanLaboratory.Fade;
using SoulRunProject.Common;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HikanyanLaboratory.SceneManagement
{
    public class SceneManager : AbstractSingletonMonoBehaviour<SceneManager>
    {
        [SerializeField, Tooltip("Title -> TutorialScene"), Header("タイトル -> チュートリアル")]
        BasicFadeStrategy _titleToTutorial;

        [SerializeField, Tooltip("TutorialScene -> StraightInGame"), Header("チュートリアル -> メインゲーム")]
        BasicFadeStrategy _tutorialToMainGame;

        [SerializeField, Tooltip("StraightInGame -> ThankYouForPlaying"), Header("メインゲーム -> リザルト")]
        BasicFadeStrategy _mainGameToResult;

        [SerializeField, Tooltip("ThankYouForPlaying -> Title"), Header("リザルト -> タイトル")]
        BasicFadeStrategy _resultToTitle;

        protected override bool UseDontDestroyOnLoad => true;

        private bool _isTransitioning;

        // タイトル -> チュートリアル
        // チュートリアル -> メインゲーム
        // メインゲーム -> リザルト
        // リザルト -> タイトル


        public async UniTask LoadSceneWithFade(string sceneName)
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

                // フェードアウト
                await FadeController.Instance.FadeOut(CheckFadeStrategy(sceneName));
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

                // シーンを切り替えたらPause解除
                _isTransitioning = false;
                PauseManager.Pause(false);
                // フェードイン
                await FadeController.Instance.FadeIn(CheckFadeStrategy(sceneName));
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"シーン遷移中にエラーが発生しました: {ex.Message}");
            }
        }

        private IFadeStrategy CheckFadeStrategy(string sceneName)
        {
            return sceneName switch
            {
                "TutorialScene" => _titleToTutorial,
                "StraightInGame" => _tutorialToMainGame,
                "ThankYouForPlaying" => _mainGameToResult,
                "Title" => _resultToTitle,
                _ => (IFadeStrategy)null
            };
        }
    }
}