using System;
using System.Globalization;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;
using VContainer;

namespace SoulRunProject.Audio
{
    /// <summary>
    /// 各種音声を再生するためのビュー
    /// </summary>
    public class CirView : MonoBehaviour
    {
        [SerializeField] private GameObject _volumeControlPrefab;
        [SerializeField] private GameObject _cueNameControlPrefab;
        [SerializeField] private Transform _volumeControlsParent;
        [SerializeField] private Transform _cueNameControlsParent;

        [SerializeField] private LabelButton _bgmButton;
        [SerializeField] private LabelButton _seButton;
        [SerializeField] private LabelButton _meButton;
        [SerializeField] private LabelButton _voiceButton;

        private CriAudioManager _criAudioManager;

        private VolumeControl _masterVolumeControl;
        private VolumeControl _bgmVolumeControl;
        private VolumeControl _seVolumeControl;
        private VolumeControl _meVolumeControl;
        private VolumeControl _voiceVolumeControl;

        private CueNameControl _bgmCueNameControl;
        private CueNameControl _seCueNameControl;
        private CueNameControl _meCueNameControl;
        private CueNameControl _voiceCueNameControl;

        private float _bgmVolume = 1f;
        private float _seVolume = 1f;
        private float _meVolume = 1f;
        private float _voiceVolume = 1f;

        [Inject]
        public void Construct(CriAudioManager criAudioManager)
        {
            _criAudioManager = criAudioManager;
        }

        private void Start()
        {
            _criAudioManager = CriAudioManager.Instance;

            _masterVolumeControl = CreateVolumeControl("Master Volume", _criAudioManager.MasterVolume,
                OnMasterVolumeSliderChanged, OnMasterVolumeInputChanged);
            _bgmVolumeControl = CreateVolumeControl("BGM Volume", _bgmVolume, OnBgmVolumeSliderChanged,
                OnBgmVolumeInputChanged);
            _seVolumeControl =
                CreateVolumeControl("SE Volume", _seVolume, OnSeVolumeSliderChanged, OnSeVolumeInputChanged);
            _meVolumeControl =
                CreateVolumeControl("ME Volume", _meVolume, OnMeVolumeSliderChanged, OnMeVolumeInputChanged);
            _voiceVolumeControl =
                CreateVolumeControl("Voice Volume", _voiceVolume, OnVoiceVolumeSliderChanged,
                    OnVoiceVolumeInputChanged);

            _bgmCueNameControl = CreateCueNameControl("BGM Cue Name");
            _seCueNameControl = CreateCueNameControl("SE Cue Name");
            _meCueNameControl = CreateCueNameControl("ME Cue Name");
            _voiceCueNameControl = CreateCueNameControl("Voice Cue Name");

            _bgmButton.Initialize(_bgmCueNameControl.GetCueName(), CriAudioType.CueSheet_BGM, _bgmCueNameControl);
            _seButton.Initialize(_seCueNameControl.GetCueName(), CriAudioType.CueSheet_SE, _seCueNameControl);
            _meButton.Initialize(_meCueNameControl.GetCueName(), CriAudioType.CueSheet_ME, _meCueNameControl);
            _voiceButton.Initialize(_voiceCueNameControl.GetCueName(), CriAudioType.CueSheet_Voice,
                _voiceCueNameControl);
        }

        private CueNameControl CreateCueNameControl(string label)
        {
            var cueNameControlObject = Instantiate(_cueNameControlPrefab, _cueNameControlsParent);
            var cueNameControl = cueNameControlObject.GetComponent<CueNameControl>();
            cueNameControl.Initialize(label);
            return cueNameControl;
        }

        private VolumeControl CreateVolumeControl(string label, float initialValue, UnityAction<float> onSliderChanged,
            UnityAction<string> onInputChanged)
        {
            var volumeControlObject = Instantiate(_volumeControlPrefab, _volumeControlsParent);
            var volumeControl = volumeControlObject.GetComponent<VolumeControl>();
            volumeControl.Initialize(label, initialValue, onSliderChanged, onInputChanged);
            return volumeControl;
        }

        private void OnMasterVolumeSliderChanged(float value)
        {
            _criAudioManager.MasterVolume = value / 100;
            _masterVolumeControl.SetValue(value / 100);
        }

        private void OnBgmVolumeSliderChanged(float value)
        {
            _bgmVolume = value / 100;
            _bgmVolumeControl.SetValue(value / 100);
            UpdateBGMVolume();
        }

        private void OnSeVolumeSliderChanged(float value)
        {
            _seVolume = value / 100;
            _seVolumeControl.SetValue(value / 100);
            UpdateSEVolume();
        }

        private void OnMeVolumeSliderChanged(float value)
        {
            _meVolume = value / 100;
            _meVolumeControl.SetValue(value / 100);
            UpdateMEVolume();
        }

        private void OnVoiceVolumeSliderChanged(float value)
        {
            _voiceVolume = value / 100;
            _voiceVolumeControl.SetValue(value / 100);
            UpdateVoiceVolume();
        }

        private void OnMasterVolumeInputChanged(string value)
        {
            if (float.TryParse(value, out float floatValue))
            {
                _criAudioManager.MasterVolume = floatValue / 100;
                _masterVolumeControl.SetValue(floatValue / 100);
            }
        }

        private void OnBgmVolumeInputChanged(string value)
        {
            if (float.TryParse(value, out float floatValue))
            {
                _bgmVolume = floatValue / 100;
                _bgmVolumeControl.SetValue(floatValue / 100);
                UpdateBGMVolume();
            }
        }

        private void OnSeVolumeInputChanged(string value)
        {
            if (float.TryParse(value, out float floatValue))
            {
                _seVolume = floatValue / 100;
                _seVolumeControl.SetValue(floatValue / 100);
                UpdateSEVolume();
            }
        }

        private void OnMeVolumeInputChanged(string value)
        {
            if (float.TryParse(value, out float floatValue))
            {
                _meVolume = floatValue / 100;
                _meVolumeControl.SetValue(floatValue / 100);
                UpdateMEVolume();
            }
        }

        private void OnVoiceVolumeInputChanged(string value)
        {
            if (float.TryParse(value, out float floatValue))
            {
                _voiceVolume = floatValue / 100;
                _voiceVolumeControl.SetValue(floatValue / 100);
                UpdateVoiceVolume();
            }
        }

        private void UpdateBGMVolume()
        {
            var players = _criAudioManager.GetPlayers(CriAudioType.CueSheet_BGM);
            foreach (var player in players)
            {
                player.SetVolume(_bgmVolume);
            }
        }

        private void UpdateSEVolume()
        {
            var players = _criAudioManager.GetPlayers(CriAudioType.CueSheet_SE);
            foreach (var player in players)
            {
                player.SetVolume(_seVolume);
            }
        }

        private void UpdateMEVolume()
        {
            var players = _criAudioManager.GetPlayers(CriAudioType.CueSheet_ME);
            foreach (var player in players)
            {
                player.SetVolume(_meVolume);
            }
        }

        private void UpdateVoiceVolume()
        {
            var players = _criAudioManager.GetPlayers(CriAudioType.CueSheet_Voice);
            foreach (var player in players)
            {
                player.SetVolume(_voiceVolume);
            }
        }
    }
}
