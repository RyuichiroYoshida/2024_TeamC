﻿using Cysharp.Threading.Tasks;
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
                    Debug.LogError("FadeControllerが設定されていません。");
                    return;
                }

                fadeStrategy ??= new BasicFadeStrategy();

                await FadeController.Instance.FadeOut(fadeStrategy);

                var loadSceneOperation =
                    UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
                loadSceneOperation.allowSceneActivation = false;

                while (loadSceneOperation.progress < 0.9f)
                {
                    await UniTask.Yield();
                }

                loadSceneOperation.allowSceneActivation = true;
                await UniTask.Yield();


                await FadeController.Instance.FadeIn(fadeStrategy);
            }
            finally
            {
                _isTransitioning = false;
            }
        }
    }
}