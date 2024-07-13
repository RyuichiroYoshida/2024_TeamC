using System;
using CriWare;
using UniRx;
using UnityEngine;

namespace SoulRunProject.Audio
{
    public class BGMPlayer : CriAudioPlayerService
    {
        private CriPlayerData? _currentBGM;
        private readonly Subject<CriAtomExPlayback.Status> _statusSubject = new Subject<CriAtomExPlayback.Status>();
        private IDisposable _statusDisposable;
        private IDisposable _playbackStatusDisposable;
        public BGMPlayer(string cueSheetName, CriAtomListener listener)
            : base(cueSheetName, listener)
        {
            _statusDisposable = _statusSubject
            .DistinctUntilChanged()
            .Subscribe(status =>
            {
                Debug.Log($"BGM Status Changed: {status}");
            });
        }
        ~BGMPlayer()
        {
            _statusDisposable?.Dispose();
        }
        public override void Play(string cueName, float volume, bool isLoop)
        {
            // 既存のBGMを停止する
            StopAllBGM();
            // 新しいBGMを再生する
            base.Play(cueName, volume, isLoop);
        }

        private void StopAllBGM()
        {
            if (_currentBGM.HasValue && _currentBGM.Value.Playback.GetStatus() == CriAtomExPlayback.Status.Playing)
            {
                _currentBGM.Value.Player.Stop();
                _currentBGM.Value.Player.Dispose();
                _currentBGM = null;
            }
        }

        protected override void OnPlayerCreated(CriPlayerData playerData)
        {
            _currentBGM = playerData;

            // 以前の監視を解除
            _playbackStatusDisposable?.Dispose();

            // CriAtomExPlaybackのステータスを監視するObservableを作成
            var playbackStatusObservable = Observable.EveryUpdate()
                .Select(_ => _currentBGM?.Playback.GetStatus() ?? CriAtomExPlayback.Status.Removed)
                .DistinctUntilChanged();

            // 監視を開始し、ステータスをSubjectに通知
            _playbackStatusDisposable = playbackStatusObservable
                .Subscribe(status =>
                {
                    _statusSubject.OnNext(status);
                });
        }
    }
}