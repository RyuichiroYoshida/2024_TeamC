﻿using System;
using System.Collections.Generic;
using CriWare;
using SoulRunProject.Common;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;

namespace SoulRunProject.Audio
{
    /// <summary>
    /// Audioの再生を管理するクラス
    /// </summary>
    public class CriAudioManager : AbstractSingletonMonoBehaviour<CriAudioManager>
    {
        [SerializeField] private CriAudioSetting _audioSetting;
        private const float Diff = 0.01F; // 音量の変更があったかどうかの判定に使う

        private Dictionary<CriAudioType, ICriAudioPlayerService> _audioPlayers; // 各音声の再生を管理するクラス

        private CriAtomListener _listener; // リスナー
        protected override bool UseDontDestroyOnLoad => true;

        public IReactiveProperty<float> MasterVolume { get; private set; } = new ReactiveProperty<float>(1f);
        public IReactiveProperty<float> BgmVolume { get; private set; } = new ReactiveProperty<float>(1f);
        public IReactiveProperty<float> SeVolume { get; private set; } = new ReactiveProperty<float>(1f);
        public IReactiveProperty<float> MeVolume { get; private set; } = new ReactiveProperty<float>(1f);
        public IReactiveProperty<float> VoiceVolume { get; private set; } = new ReactiveProperty<float>(1f);


        public override void OnAwake()
        {
            // ACF設定
            string path = Application.streamingAssetsPath + $"/{_audioSetting.StreamingAssetsPathAcf}.acf";
            CriAtomEx.RegisterAcf(null, path);

            // CriAtom作成
            gameObject.AddComponent<CriAtom>();

            _listener = FindObjectOfType<CriAtomListener>();
            if (_listener == null)
            {
                _listener = gameObject.AddComponent<CriAtomListener>();
            }

            _audioPlayers = new Dictionary<CriAudioType, ICriAudioPlayerService>();

            foreach (var cueSheet in _audioSetting.AudioCueSheet)
            {
                CriAtom.AddCueSheet(cueSheet.CueSheetName, $"{cueSheet.AcbPath}.acb",
                    !string.IsNullOrEmpty(cueSheet.AwbPath) ? $"{cueSheet.AwbPath}.awb" : null, null);
                if (cueSheet.CueSheetName == CriAudioType.CueSheet_BGM.ToString())
                {
                    _audioPlayers.Add(CriAudioType.CueSheet_BGM, new BGMPlayer(cueSheet.CueSheetName, _listener));
                }
                else if (cueSheet.CueSheetName == CriAudioType.CueSheet_SE.ToString())
                {
                    _audioPlayers.Add(CriAudioType.CueSheet_SE, new SEPlayer(cueSheet.CueSheetName, _listener));
                }
                else if (cueSheet.CueSheetName == CriAudioType.CueSheet_VOICE.ToString())
                {
                    _audioPlayers.Add(CriAudioType.CueSheet_VOICE, new VoicePlayer(cueSheet.CueSheetName, _listener));
                }
                else if (cueSheet.CueSheetName == CriAudioType.CueSheet_ME.ToString())
                {
                    _audioPlayers.Add(CriAudioType.CueSheet_ME, new MEPlayer(cueSheet.CueSheetName, _listener));
                }
                else if (cueSheet.CueSheetName == CriAudioType.Other.ToString())
                {
                    _audioPlayers.Add(CriAudioType.Other, new OtherPlayer(cueSheet.CueSheetName, _listener));
                }
                // 他のCriAudioTypeも同様に追加可能
            }

            // MasterVolumeの変更を監視して、各Playerに反映
            MasterVolume.Subscribe(OnMasterVolumeChanged).AddTo(this);
            BgmVolume.Subscribe(volume => OnVolumeChanged(CriAudioType.CueSheet_BGM, volume)).AddTo(this);
            SeVolume.Subscribe(volume => OnVolumeChanged(CriAudioType.CueSheet_SE, volume)).AddTo(this);
            MeVolume.Subscribe(volume => OnVolumeChanged(CriAudioType.CueSheet_ME, volume)).AddTo(this);
            VoiceVolume.Subscribe(volume => OnVolumeChanged(CriAudioType.CueSheet_VOICE, volume)).AddTo(this);
        }

        private void OnMasterVolumeChanged(float volume)
        {
            // MasterVolumeの変更に伴い、各プレイヤーのボリュームを更新する
            foreach (var player in _audioPlayers.Values)
            {
                player.SetVolume(Math.Min(volume, volume * VoiceVolume.Value));
            }
        }

        private void OnVolumeChanged(CriAudioType type, float volume)
        {
            if (_audioPlayers.TryGetValue(type, out var player))
            {
                // 各プレイヤーのボリュームがMasterVolumeを超えないように制御する
                player.SetVolume(MasterVolume.Value * volume);
            }
        }

        private void OnDestroy()
        {
            SceneManager.sceneUnloaded -= Unload;
        }

        public Guid Play(CriAudioType type, string cueName)
        {
            return Play(type, cueName, 1f, false);
        }

        public Guid Play(CriAudioType type, string cueName, bool isLoop)
        {
            return Play(type, cueName, 1f, isLoop);
        }

        public Guid Play(CriAudioType type, string cueName, float volume, bool isLoop = false)
        {
            if (_audioPlayers.TryGetValue(type, out var player))
            {
                float adjustedVolume = Math.Min(volume, MasterVolume.Value * volume);
                //Debug.Log($"CriAudioType: {type}, CueName: {cueName}, Volume: {adjustedVolume}");
                return player.Play(cueName, adjustedVolume, isLoop);
            }
            else
            {
                Debug.LogWarning($"Audio type {type} not supported.");
                return Guid.Empty;
            }
        }

        public Guid Play3D(Transform transform, CriAudioType type, string cueName)
        {
            return Play3D(transform, type, cueName, 1f, false);
        }

        public Guid Play3D(Transform transform, CriAudioType type, string cueName, bool isLoop)
        {
            return Play3D(transform, type, cueName, 1f, isLoop);
        }

        public Guid Play3D(Transform transform, CriAudioType type, string cueName, float volume, bool isLoop = false)
        {
            if (_audioPlayers.TryGetValue(type, out var player))
            {
                float adjustedVolume = Math.Min(volume, MasterVolume.Value * volume);
                Debug.Log($"CriAudioType: {type}, CueName: {cueName}");
                return player.Play3D(transform, cueName, adjustedVolume, isLoop);
            }
            else
            {
                Debug.LogWarning($"3D audio type {type} not supported.");
                return Guid.Empty;
            }
        }

        public void Stop(CriAudioType type, Guid id)
        {
            if (_audioPlayers.TryGetValue(type, out var player))
            {
                player.Stop(id);
            }
            else
            {
                Debug.LogWarning($"Audio type {type} not supported.");
            }
        }

        public void Pause(CriAudioType type, Guid id)
        {
            if (_audioPlayers.TryGetValue(type, out var player))
            {
                player.Pause(id);
            }
            else
            {
                Debug.LogWarning($"Audio type {type} not supported.");
            }
        }

        public void Resume(CriAudioType type, Guid id)
        {
            if (_audioPlayers.TryGetValue(type, out var player))
            {
                player.Resume(id);
            }
            else
            {
                Debug.LogWarning($"Audio type {type} not supported.");
            }
        }

        public void SetVolume(CriAudioType type, float volume)
        {
            if (_audioPlayers.TryGetValue(type, out var player))
            {
                float adjustedVolume = Math.Min(volume, MasterVolume.Value * volume);
                player.SetVolume(adjustedVolume);
            }
            else if (type == CriAudioType.Master)
            {
                //Debug.Log($"MasterVolume: {volume}");
            }
            else
            {
                Debug.LogWarning($"Audio type {type} not supported.");
            }
        }


        public void StopAll()
        {
            foreach (var player in _audioPlayers.Values)
            {
                player.StopAll();
            }
        }

        public void PauseAll()
        {
            foreach (var player in _audioPlayers.Values)
            {
                player.PauseAll();
            }
        }

        public void ResumeAll()
        {
            foreach (var player in _audioPlayers.Values)
            {
                player.ResumeAll();
            }
        }

        public ICriAudioPlayerService GetPlayer(CriAudioType type)
        {
            if (_audioPlayers.TryGetValue(type, out var player))
            {
                return player;
            }

            return null;
        }

        public float GetPlayerVolume(CriAudioType type)
        {
            if (_audioPlayers.TryGetValue(type, out var player))
            {
                return player.Volume.Value;
            }

            return type == CriAudioType.Master ? MasterVolume.Value : 1f;
        }

        private void Unload(Scene scene)
        {
            foreach (var player in _audioPlayers.Values)
            {
                player.Dispose();
            }
        }
    }
}