using System;
using System.Collections.Generic;
using CriWare;
using SoulRunProject.Common;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;

namespace SoulRunProject.Audio
{
    public class CriAudioManager : AbstractSingletonMonoBehaviour<CriAudioManager>
    {
        [SerializeField] private CriAudioSetting _audioSetting;
        private float _masterVolume = 1F; // マスターボリューム
        private const float Diff = 0.01F; // 音量の変更があったかどうかの判定に使う

        private Action<float> _masterVolumeChanged; // マスターボリューム変更時のイベント
        private Dictionary<CriAudioType, ICriAudioPlayerService> _audioPlayers; // 各音声の再生を管理するクラス

        private CriAtomListener _listener; // リスナー
        protected override bool UseDontDestroyOnLoad => true;

        private void Awake()
        {
            InitializeCriAtom();
            InitializeListeners();
            InitializeAudioPlayers();
            SubscribeToEvents();
        }

        private void OnDestroy()
        {
            SceneManager.sceneUnloaded -= Unload;
        }

        public float MasterVolume
        {
            get => _masterVolume;
            set
            {
                if (Mathf.Abs(_masterVolume - value) > Diff)
                {
                    _masterVolume = value;
                    _masterVolumeChanged?.Invoke(value);
                }
            }
        }

        public void Play(CriAudioType type, string cueName)
        {
            Play(type, cueName, 1f, false);
        }

        public void Play(CriAudioType type, string cueName, bool isLoop)
        {
            Play(type, cueName, 1f, isLoop);
        }

        public void Play(CriAudioType type, string cueName, float volume, bool isLoop)
        {
            if (_audioPlayers.TryGetValue(type, out var player))
            {
                Debug.Log($"CriAudioType: {type}, CueName: {cueName}");
                player.Play(cueName, volume, isLoop);
            }
            else
            {
                Debug.LogWarning($"Audio type {type} not supported.");
            }
        }

        public void Play3D(Transform transform, CriAudioType type, string cueName)
        {
            Play3D(transform, type, cueName, 1f, false);
        }

        public void Play3D(Transform transform, CriAudioType type, string cueName, bool isLoop)
        {
            Play3D(transform, type, cueName, 1f, isLoop);
        }

        public void Play3D(Transform transform, CriAudioType type, string cueName, float volume, bool isLoop)
        {
            if (_audioPlayers.TryGetValue(type, out var player))
            {
                Debug.Log($"CriAudioType: {type}, CueName: {cueName}");
                player.Play3D(transform, cueName, volume, isLoop);
            }
            else
            {
                Debug.LogWarning($"3D audio type {type} not supported.");
            }
        }

        public void Pause(CriAudioType type, string cueName)
        {
            if (_audioPlayers.TryGetValue(type, out var player))
            {
                player.Pause(cueName);
            }
            else
            {
                Debug.LogWarning($"Audio type {type} not supported.");
            }
        }

        public void Resume(CriAudioType type, string cueName)
        {
            if (_audioPlayers.TryGetValue(type, out var player))
            {
                player.Resume(cueName);
            }
            else
            {
                Debug.LogWarning($"Audio type {type} not supported.");
            }
        }

        public void Stop(CriAudioType type, string cueName)
        {
            if (_audioPlayers.TryGetValue(type, out var player))
            {
                player.Stop(cueName);
            }
            else
            {
                Debug.LogWarning($"Audio type {type} not supported.");
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

        public List<ICriAudioPlayerService> GetPlayers(CriAudioType type)
        {
            if (_audioPlayers.TryGetValue(type, out var player))
            {
                return new List<ICriAudioPlayerService> { player };
            }

            return new List<ICriAudioPlayerService>();
        }

        private void Unload(Scene scene)
        {
            foreach (var player in _audioPlayers.Values)
            {
                player.Dispose();
            }
        }

        public void Dispose()
        {
            SceneManager.sceneUnloaded -= Unload;
            foreach (var player in _audioPlayers.Values)
            {
                player.Dispose();
            }
        }

        private void InitializeCriAtom()
        {
            string path = Application.streamingAssetsPath + $"/{_audioSetting.StreamingAssetsPathAcf}.acf";
            CriAtomEx.RegisterAcf(null, path);
            gameObject.AddComponent<CriAtom>();
        }

        private void InitializeListeners()
        {
            _listener = FindObjectOfType<CriAtomListener>();
            if (_listener == null)
            {
                _listener = gameObject.AddComponent<CriAtomListener>();
            }
        }

        private void InitializeAudioPlayers()
        {
            _audioPlayers = new Dictionary<CriAudioType, ICriAudioPlayerService>();

            foreach (var cueSheet in _audioSetting.AudioCueSheet)
            {
                CriAtom.AddCueSheet(cueSheet.CueSheetName, $"{cueSheet.AcbPath}.acb",
                    !string.IsNullOrEmpty(cueSheet.AwbPath) ? $"{cueSheet.AwbPath}.awb" : null, null);

                switch (cueSheet.CueSheetName)
                {
                    case var name when name == CriAudioType.CueSheet_BGM.ToString():
                        _audioPlayers.Add(CriAudioType.CueSheet_BGM, new BGMPlayer(cueSheet.CueSheetName, _listener));
                        break;
                    case var name when name == CriAudioType.CueSheet_SE.ToString():
                        _audioPlayers.Add(CriAudioType.CueSheet_SE, new SEPlayer(cueSheet.CueSheetName, _listener));
                        break;
                    case var name when name == CriAudioType.CueSheet_ME.ToString():
                        _audioPlayers.Add(CriAudioType.CueSheet_ME, new MEPlayer(cueSheet.CueSheetName, _listener));
                        break;
                    case var name when name == CriAudioType.Other.ToString():
                        _audioPlayers.Add(CriAudioType.Other, new OtherPlayer(cueSheet.CueSheetName, _listener));
                        break;
                }
            }
        }

        private void SubscribeToEvents()
        {
            _masterVolumeChanged += volume =>
            {
                foreach (var player in _audioPlayers.Values)
                {
                    player.SetVolume(volume);
                }
            };

            SceneManager.sceneUnloaded += Unload;
        }
    }
}
