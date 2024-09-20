using System;
using System.Collections.Generic;
using CriWare;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

namespace SoulRunProject.Audio
{
    public class SEPlayer : CriAudioPlayerService
    {
        private const int MaxSameSePlayCount = 10; // 同時再生数の上限
        private readonly Dictionary<string, int> sePlayCountDict = new Dictionary<string, int>(); // SEの再生回数をカウント

        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        public SEPlayer(string cueSheetName, CriAtomListener listener)
            : base(cueSheetName, listener)
        {
            Observable.EveryUpdate()
                .Subscribe(_ => CheckPlayerStatus())
                .AddTo(_disposables);
        }

        ~SEPlayer()
        {
            _disposables.Dispose();
        }

        // ReSharper disable Unity.PerformanceAnalysis
        public override Guid Play(string cueName, float volume = 1f, bool isLoop = false)
        {
            // SEの再生回数を確認
            if (!sePlayCountDict.ContainsKey(cueName))
            {
                sePlayCountDict[cueName] = 0;
            }

            // 同じSEの再生回数が上限に達している場合、再生しない
            if (sePlayCountDict[cueName] >= MaxSameSePlayCount)
            {
                Debug.LogWarning($"SE {cueName} の再生が制限されました（上限: {MaxSameSePlayCount}）");
                return Guid.Empty;
            }

            // 通常のSE再生処理
            Guid playbackId = base.Play(cueName, volume, isLoop);
            if (playbackId == Guid.Empty) return playbackId;
            sePlayCountDict[cueName]++;

            // 再生終了時にカウントを減少
            var playback = _playbacks[playbackId];
            Observable.EveryUpdate()
                .Where(_ => playback.GetStatus() == CriAtomExPlayback.Status.Removed)
                .Take(1)
                .Subscribe(_ => { sePlayCountDict[cueName]--; }).AddTo(_disposables);

            return playbackId;
        }
    }
}