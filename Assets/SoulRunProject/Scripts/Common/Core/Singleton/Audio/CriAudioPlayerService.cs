using System;
using System.Collections.Generic;
using CriWare;
using UnityEngine;

namespace SoulRunProject.Audio
{
    public abstract class CriAudioPlayerService : ICriAudioPlayerService
    {
        protected readonly string _cueSheetName;
        protected readonly CriAtomListener _listener;
        protected readonly List<CriPlayerData> _playersData = new List<CriPlayerData>(); // 再生中のプレーヤーデータを保持
        private float _volume = 1f;
        private const float MasterVolume = 1f;

        public CriAudioPlayerService(string cueSheetName, CriAtomListener listener)
        {
            _cueSheetName = cueSheetName;
            _listener = listener;
        }

        ~CriAudioPlayerService()
        {
            Dispose();
        }

        public virtual void Play(string cueName, float volume, bool isLoop)
        {
            if (!CheckCueSheet())
            {
                Debug.LogWarning($"ACBがNullです。CueSheet: {_cueSheetName}");
                return;
            }

            var playerData = CreateAndConfigurePlayer(cueName, volume, isLoop);
            playerData.Playback = playerData.Player.Start();
            _playersData.Add(playerData);
            OnPlayerCreated(playerData);
        }

        public virtual void Play3D(Transform transform, string cueName, float volume, bool isLoop)
        {
            if (!CheckCueSheet())
            {
                Debug.LogWarning($"ACBがNullです。CueSheet: {_cueSheetName}");
                return;
            }

            var playerData = CreateAndConfigurePlayer(cueName, volume, isLoop);
            playerData.Player.Set3dSource(Create3dSource(transform));
            playerData.Player.Set3dListener(_listener.nativeListener);
            playerData.Playback = playerData.Player.Start();
            _playersData.Add(playerData); // プレーヤーデータをリストに追加
            OnPlayerCreated(playerData);
        }

        private CriPlayerData CreateAndConfigurePlayer(string cueName, float volume, bool isLoop)
        {
            var tempAcb = CriAtom.GetCueSheet(_cueSheetName).acb;
            CriPlayerData playerData = new CriPlayerData();
            tempAcb.GetCueInfo(cueName, out var cueInfo);
            playerData.CueInfo = cueInfo;

            CriAtomExPlayer player = new CriAtomExPlayer();
            player.SetCue(tempAcb, cueName);
            player.SetVolume(volume * _volume * MasterVolume);
            player.Loop(isLoop);
            playerData.Player = player;
            playerData.IsLoop = isLoop;

            return playerData;
        }

        private CriAtomEx3dSource Create3dSource(Transform transform)
        {
            CriAtomEx3dSource source = new CriAtomEx3dSource();
            source.SetPosition(transform.position.x, transform.position.y, transform.position.z);
            source.Update();
            return source;
        }

        public void Pause(string cueName)
        {
            foreach (var playerData in _playersData)
            {
                if (playerData.CueInfo.name == cueName &&
                    playerData.Playback.GetStatus() == CriAtomExPlayback.Status.Playing)
                {
                    playerData.Player.Pause();
                }
            }
        }

        public void Resume(string cueName)
        {
            foreach (var playerData in _playersData)
            {
                if (playerData.CueInfo.name == cueName && playerData.Playback.IsPaused())
                {
                    playerData.Playback.Resume(CriAtomEx.ResumeMode.PausedPlayback);
                }
            }
        }

        public void Stop(string cueName)
        {
            for (int i = _playersData.Count - 1; i >= 0; i--)
            {
                var playerData = _playersData[i];
                if (playerData.CueInfo.name == cueName)
                {
                    playerData.Player.Stop();
                    playerData.Player.Dispose();
                    _playersData.RemoveAt(i);
                }
            }
        }

        public void SetVolume(float volume)
        {
            _volume = volume;
            foreach (var playerData in _playersData)
            {
                playerData.Player.SetVolume(_volume);
            }
        }

        public void PauseAll()
        {
            foreach (var playerData in _playersData)
            {
                if (playerData.Playback.GetStatus() == CriAtomExPlayback.Status.Playing)
                {
                    playerData.Player.Pause();
                }
            }
        }

        public void ResumeAll()
        {
            foreach (var playerData in _playersData)
            {
                if (playerData.Playback.IsPaused())
                {
                    playerData.Playback.Resume(CriAtomEx.ResumeMode.PausedPlayback);
                }
            }
        }

        public void Dispose()
        {
            foreach (var playerData in _playersData)
            {
                playerData.Player.Dispose();
            }

            _playersData.Clear();
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

        public bool CheckPlaying(string cueName)
        {
            foreach (var playerData in _playersData)
            {
                if (playerData.CueInfo.name == cueName &&
                    playerData.Playback.GetStatus() == CriAtomExPlayback.Status.Playing)
                {
                    return true;
                }
            }

            return false;
        }

        protected abstract void OnPlayerCreated(CriPlayerData playerData);
    }
}