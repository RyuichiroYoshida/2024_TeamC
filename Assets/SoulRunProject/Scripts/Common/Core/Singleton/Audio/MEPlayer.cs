using System.Collections.Generic;
using CriWare;
using UniRx;
using UnityEngine;

namespace SoulRunProject.Audio
{
    public class MEPlayer : CriAudioPlayerService
    {
        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        public MEPlayer(string cueSheetName, CriAtomListener listener)
            : base(cueSheetName, listener)
        {
            Observable.EveryUpdate()
                .Subscribe(_ => CheckPlayerStatus())
                .AddTo(_disposables);
        }

        protected override void PrePlayCheck(string cueName)
        {
            // 既に再生中の場合は再生しない
            if (_playbacks.ContainsKey(cueName) && _playbacks[cueName].GetStatus() == CriAtomExPlayback.Status.Playing)
            {
                return;
            }

            // BGM 再生時には既存の BGM を止める
            StopAllME();
        }

        private void StopAllME()
        {
            var cuesToStop = new List<string>(_playbacks.Keys);
            foreach (var cue in cuesToStop)
            {
                Stop(cue);
            }
        }

        ~MEPlayer()
        {
            _disposables.Dispose();
        }
    }
}