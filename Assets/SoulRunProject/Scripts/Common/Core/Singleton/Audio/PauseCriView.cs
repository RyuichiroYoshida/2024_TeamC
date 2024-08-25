using System;
using UniRx;
using UnityEngine;
using UnityEngine.Events;

namespace SoulRunProject.Audio
{
    public class PauseCriView : MonoBehaviour
    {
        [SerializeField] private CriVolumeControl _volumeControlPrefab;
        [SerializeField] private Transform _volumeControlsParent;


        private CriVolumeControl _masterCriVolumeControl;
        private CriVolumeControl _bgmCriVolumeControl;
        private CriVolumeControl _seCriVolumeControl;
        private CriVolumeControl _meCriVolumeControl;
        private CriVolumeControl _voiceCriVolumeControl;

        private CriAudioManager _criAudioManager;

        private void Start()
        {
            _criAudioManager = CriAudioManager.Instance;

            _masterCriVolumeControl = CreateVolumeControl("Master Volume", _criAudioManager.MasterVolume.Value,
                CriAudioType.Master, OnMasterVolumeSliderChanged, OnMasterVolumeInputChanged);
            _bgmCriVolumeControl = CreateVolumeControl("BGM Volume",
                _criAudioManager.GetPlayerVolume(CriAudioType.CueSheet_BGM), CriAudioType.CueSheet_BGM,
                OnBgmAndMeVolumeSliderChanged, OnBgmAndMeVolumeInputChanged);
            _seCriVolumeControl = CreateVolumeControl("SE Volume",
                _criAudioManager.GetPlayerVolume(CriAudioType.CueSheet_SE), CriAudioType.CueSheet_SE,
                OnSeAndVoiceVolumeSliderChanged, OnSeAndVoiceVolumeInputChanged);
        }


        private CriVolumeControl CreateVolumeControl(string label, float initialValue, CriAudioType audioType,
            UnityAction<float> onSliderChanged, UnityAction<string> onInputChanged)
        {
            var volumeControlObject = Instantiate(_volumeControlPrefab, _volumeControlsParent);
            volumeControlObject.Initialize(label, initialValue, audioType, onSliderChanged, onInputChanged);
            return volumeControlObject;
        }

        private void OnMasterVolumeSliderChanged(float value)
        {
            _criAudioManager.MasterVolume.Value = value / 100;
            _masterCriVolumeControl.SetVolume(value / 100); // スライダーの値を直接反映
        }

        private void OnBgmVolumeSliderChanged(float value)
        {
            var player = _criAudioManager.GetPlayer(CriAudioType.CueSheet_BGM);
            if (player != null)
            {
                player.Volume.Value = value / 100;
                _bgmCriVolumeControl.SetVolume(value / 100); // スライダーの値を直接反映
            }
        }

        private void OnSeVolumeSliderChanged(float value)
        {
            var player = _criAudioManager.GetPlayer(CriAudioType.CueSheet_SE);
            if (player != null)
            {
                player.Volume.Value = value / 100;
                _seCriVolumeControl.SetVolume(value / 100); // スライダーの値を直接反映
            }
        }

        private void OnMeVolumeSliderChanged(float value)
        {
            var player = _criAudioManager.GetPlayer(CriAudioType.CueSheet_ME);
            if (player != null)
            {
                player.Volume.Value = value / 100;
                _meCriVolumeControl.SetVolume(value / 100); // スライダーの値を直接反映
            }
        }

        private void OnVoiceVolumeSliderChanged(float value)
        {
            var player = _criAudioManager.GetPlayer(CriAudioType.CueSheet_Voice);
            if (player != null)
            {
                player.Volume.Value = value / 100;
                _voiceCriVolumeControl.SetVolume(value / 100); // スライダーの値を直接反映
            }
        }

        private void OnMasterVolumeInputChanged(string value)
        {
            if (float.TryParse(value, out float floatValue))
            {
                _criAudioManager.MasterVolume.Value = floatValue / 100;
                _masterCriVolumeControl.SetVolume(floatValue / 100); // 入力フィールドの値を直接反映
            }
        }

        private void OnBgmVolumeInputChanged(string value)
        {
            if (float.TryParse(value, out float floatValue))
            {
                var player = _criAudioManager.GetPlayer(CriAudioType.CueSheet_BGM);
                if (player != null)
                {
                    player.Volume.Value = floatValue / 100;
                    _bgmCriVolumeControl.SetVolume(floatValue / 100); // 入力フィールドの値を直接反映
                }
            }
        }

        private void OnSeVolumeInputChanged(string value)
        {
            if (float.TryParse(value, out float floatValue))
            {
                var player = _criAudioManager.GetPlayer(CriAudioType.CueSheet_SE);
                if (player != null)
                {
                    player.Volume.Value = floatValue / 100;
                    _seCriVolumeControl.SetVolume(floatValue / 100); // 入力フィールドの値を直接反映
                }
            }
        }

        private void OnMeVolumeInputChanged(string value)
        {
            if (float.TryParse(value, out float floatValue))
            {
                var player = _criAudioManager.GetPlayer(CriAudioType.CueSheet_ME);
                if (player != null)
                {
                    player.Volume.Value = floatValue / 100;
                    _meCriVolumeControl.SetVolume(floatValue / 100); // 入力フィールドの値を直接反映
                }
            }
        }

        private void OnVoiceVolumeInputChanged(string value)
        {
            if (float.TryParse(value, out float floatValue))
            {
                var player = _criAudioManager.GetPlayer(CriAudioType.CueSheet_Voice);
                if (player != null)
                {
                    player.Volume.Value = floatValue / 100;
                    _voiceCriVolumeControl.SetVolume(floatValue / 100); // 入力フィールドの値を直接反映
                }
            }
        }

        private void BindVolumeControls()
        {
            _criAudioManager.GetPlayer(CriAudioType.CueSheet_BGM)?.Volume.Subscribe(volume =>
            {
                _bgmCriVolumeControl.SetVolume(volume);
            }).AddTo(this);

            _criAudioManager.GetPlayer(CriAudioType.CueSheet_SE)?.Volume.Subscribe(volume =>
            {
                _seCriVolumeControl.SetVolume(volume);
            }).AddTo(this);

            _criAudioManager.GetPlayer(CriAudioType.CueSheet_ME)?.Volume.Subscribe(volume =>
            {
                _meCriVolumeControl.SetVolume(volume);
            }).AddTo(this);

            _criAudioManager.GetPlayer(CriAudioType.CueSheet_Voice)?.Volume.Subscribe(volume =>
            {
                _voiceCriVolumeControl.SetVolume(volume);
            }).AddTo(this);

            _criAudioManager.MasterVolume.Subscribe(volume => { _masterCriVolumeControl.SetVolume(volume); })
                .AddTo(this);
        }

        private void OnBgmAndMeVolumeSliderChanged(float value)
        {
            var bgmPlayer = _criAudioManager.GetPlayer(CriAudioType.CueSheet_BGM);
            if (bgmPlayer != null)
            {
                bgmPlayer.Volume.Value = value / 100;
                _bgmCriVolumeControl.SetVolume(value / 100); // スライダーの値を直接反映
            }

            var mePlayer = _criAudioManager.GetPlayer(CriAudioType.CueSheet_ME);
            if (mePlayer != null)
            {
                mePlayer.Volume.Value = value / 100;
                _meCriVolumeControl.SetVolume(value / 100); // スライダーの値を直接反映
            }
        }

        private void OnSeAndVoiceVolumeSliderChanged(float value)
        {
            var sePlayer = _criAudioManager.GetPlayer(CriAudioType.CueSheet_SE);
            if (sePlayer != null)
            {
                sePlayer.Volume.Value = value / 100;
                _seCriVolumeControl.SetVolume(value / 100); // スライダーの値を直接反映
            }

            var voicePlayer = _criAudioManager.GetPlayer(CriAudioType.CueSheet_Voice);
            if (voicePlayer != null)
            {
                voicePlayer.Volume.Value = value / 100;
                _voiceCriVolumeControl.SetVolume(value / 100); // スライダーの値を直接反映
            }
        }

        private void OnBgmAndMeVolumeInputChanged(string value)
        {
            if (float.TryParse(value, out float floatValue))
            {
                var bgmPlayer = _criAudioManager.GetPlayer(CriAudioType.CueSheet_BGM);
                if (bgmPlayer != null)
                {
                    bgmPlayer.Volume.Value = floatValue / 100;
                    _bgmCriVolumeControl.SetVolume(floatValue / 100); // 入力フィールドの値を直接反映
                }

                var mePlayer = _criAudioManager.GetPlayer(CriAudioType.CueSheet_ME);
                if (mePlayer != null)
                {
                    mePlayer.Volume.Value = floatValue / 100;
                    _meCriVolumeControl.SetVolume(floatValue / 100); // 入力フィールドの値を直接反映
                }
            }
        }

        private void OnSeAndVoiceVolumeInputChanged(string value)
        {
            if (float.TryParse(value, out float floatValue))
            {
                var sePlayer = _criAudioManager.GetPlayer(CriAudioType.CueSheet_SE);
                if (sePlayer != null)
                {
                    sePlayer.Volume.Value = floatValue / 100;
                    _seCriVolumeControl.SetVolume(floatValue / 100); // 入力フィールドの値を直接反映
                }

                var voicePlayer = _criAudioManager.GetPlayer(CriAudioType.CueSheet_Voice);
                if (voicePlayer != null)
                {
                    voicePlayer.Volume.Value = floatValue / 100;
                    _voiceCriVolumeControl.SetVolume(floatValue / 100); // 入力フィールドの値を直接反映
                }
            }
        }
    }
}