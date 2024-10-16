﻿using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

namespace SoulRunProject.Audio
{
    public class TestCri : MonoBehaviour
    {
        private void Start()
        {
            // 初期状態でBGMを再生
            CriAudioManager.Instance.Play(CriAudioType.CueSheet_BGM, "BGM_Title");

            // キー入力を監視
            Observable.EveryUpdate()
                .Where(_ => Input.GetKeyDown(KeyCode.P))
                .Subscribe(_ => PlayBGM());

            Observable.EveryUpdate()
                .Where(_ => Input.GetKeyDown(KeyCode.O))
                .Subscribe(_ => PlaySE());

            _ = Task();
        }

        private void PlayBGM()
        {
            CriAudioManager.Instance.Play(CriAudioType.CueSheet_BGM, "BGM_Stage1");
        }

        private void PlaySE()
        {
            CriAudioManager.Instance.Play(CriAudioType.CueSheet_SE, "SE_Decision");
        }

        async UniTask Task()
        {
            await UniTask.Delay(1000);
            // 1秒後にSEを再生
            for (int i = 0; i < 34; i++)
            {
                CriAudioManager.Instance.Play(CriAudioType.CueSheet_SE, "SE_Decision");
            }
        }
    }
}