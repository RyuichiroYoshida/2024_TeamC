using System.Collections.Generic;
using CriWare;
using UnityEngine;

namespace SoulRunProject.Audio
{
    public abstract class CriAudioPlayerService : ICriAudioPlayerService
    {
        protected readonly CriAtomExPlayer _criAtomExPlayer; // 複数の音声を再生するためのプレイヤー
        protected readonly CriAtomEx3dSource _criAtomEx3dSource; // 3D音源
        protected readonly Dictionary<string, CriAtomExPlayback> _playbacks; // 再生中の音声を管理
        protected readonly CriAtomListener _criAtomListener; // リスナー
        protected readonly string _cueSheetName; // ACBファイルの名前
        private const float MasterVolume = 1f; // マスターボリューム
        private float _volume = 1f; // ボリューム

        public CriAudioPlayerService(string cueSheetName, CriAtomListener criAtomListener)
        {
            _cueSheetName = cueSheetName;
            _criAtomListener = criAtomListener;
            _criAtomExPlayer = new CriAtomExPlayer();
            _criAtomEx3dSource = new CriAtomEx3dSource();
            _playbacks = new Dictionary<string, CriAtomExPlayback>();
        }

        ~CriAudioPlayerService()
        {
            Dispose();
        }

        public virtual void Play(string cueName, float volume = 1f, bool isLoop = false)
        {
            if (!CheckCueSheet())
            {
                Debug.LogWarning($"ACBがNullです。CueSheet: {_cueSheetName}");
                return;
            }

            var tempAcb = CriAtom.GetCueSheet(_cueSheetName).acb;
            tempAcb.GetCueInfo(cueName, out var cueInfo);


            PrePlayCheck(cueName);
            _criAtomExPlayer.SetCue(tempAcb, cueName);
            _criAtomExPlayer.SetVolume(volume * _volume * MasterVolume);
            _criAtomExPlayer.Loop(isLoop);

            var playback = _criAtomExPlayer.Start();
            _playbacks[cueName] = playback;
        }

        public virtual void Play3D(Transform transform, string cueName, float volume = 1f, bool isLoop = false)
        {
            if (!CheckCueSheet())
            {
                Debug.LogWarning($"ACBがNullです。CueSheet: {_cueSheetName}");
                return;
            }

            var tempAcb = CriAtom.GetCueSheet(_cueSheetName).acb;
            tempAcb.GetCueInfo(cueName, out var cueInfo);

            if (_playbacks.ContainsKey(cueName) && _playbacks[cueName].GetStatus() == CriAtomExPlayback.Status.Playing)
            {
                return;
            }

            PrePlayCheck(cueName);
            _criAtomEx3dSource.SetPosition(transform.position.x, transform.position.y, transform.position.z);
            _criAtomEx3dSource.Update();

            _criAtomExPlayer.Set3dSource(_criAtomEx3dSource);
            _criAtomExPlayer.Set3dListener(_criAtomListener.nativeListener);
            _criAtomExPlayer.SetCue(tempAcb, cueName);
            _criAtomExPlayer.SetVolume(volume * _volume * MasterVolume);
            _criAtomExPlayer.Loop(isLoop);

            var playback = _criAtomExPlayer.Start();
            _playbacks[cueName] = playback;
        }

        public void Stop(string cueName)
        {
            if (_playbacks.ContainsKey(cueName))
            {
                _playbacks[cueName].Stop();
                _playbacks.Remove(cueName);
            }
        }

        public void Pause(string cueName)
        {
            if (_playbacks.ContainsKey(cueName))
            {
                _playbacks[cueName].Pause();
            }
        }

        public void Resume(string cueName)
        {
            if (_playbacks.ContainsKey(cueName))
            {
                _playbacks[cueName].Resume(CriAtomEx.ResumeMode.PausedPlayback);
            }
        }

        public void StopAll()
        {
            foreach (var playback in _playbacks.Values)
            {
                playback.Stop();
            }

            _playbacks.Clear();
        }

        public void PauseAll()
        {
            foreach (var playback in _playbacks.Values)
            {
                playback.Pause();
            }
        }

        public void SetVolume(float volume)
        {
            _volume = volume;
            _criAtomExPlayer.SetVolume(_volume * MasterVolume);
        }

        public void Dispose()
        {
            foreach (var playback in _playbacks.Values)
            {
                playback.Stop();
            }

            _criAtomExPlayer.Dispose();
            _criAtomEx3dSource.Dispose();
        }

        public bool CheckCueSheet()
        {
            var tempAcb = CriAtom.GetCueSheet(_cueSheetName)?.acb;
            if (tempAcb == null)
            {
                Debug.LogWarning($"ACBがNullです。CueSheet: {_cueSheetName}");
                return false;
            }

            return true;
        }

        public void CheckPlayerStatus()
        {
            var cuesToRemove = new List<string>();

            foreach (var kvp in _playbacks)
            {
                if (kvp.Value.GetStatus() == CriAtomExPlayback.Status.Removed)
                {
                    cuesToRemove.Add(kvp.Key);
                }
            }

            foreach (var cue in cuesToRemove)
            {
                _playbacks.Remove(cue);
            }
        }

        protected virtual void PrePlayCheck(string cueName)
        {
        }
    }
}