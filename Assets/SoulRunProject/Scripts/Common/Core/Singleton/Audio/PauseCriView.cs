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
        private CriVolumeControl _bgmMeCriVolumeControl; // BGMとME用
        private CriVolumeControl _seVoiceCriVolumeControl; // SEとVoice用

        private CriAudioManager _criAudioManager;

        private void Start()
        {
            _criAudioManager = CriAudioManager.Instance;

            // マスターボリュームコントロールを作成
            _masterCriVolumeControl = CreateVolumeControl("Master", 
                _criAudioManager.MasterVolume.Value,
                CriAudioType.Master, OnMasterVolumeSliderChanged, OnMasterVolumeInputChanged);

            // BGMとMEのボリュームコントロールを作成
            _bgmMeCriVolumeControl = CreateVolumeControl("BGM/ME",
                _criAudioManager.GetPlayerVolume(CriAudioType.CueSheet_BGM), CriAudioType.CueSheet_BGM,
                OnBgmAndMeVolumeSliderChanged, OnBgmAndMeVolumeInputChanged);

            // SEとVoiceのボリュームコントロールを作成
            _seVoiceCriVolumeControl = CreateVolumeControl("SE/Voice",
                _criAudioManager.GetPlayerVolume(CriAudioType.CueSheet_SE), CriAudioType.CueSheet_SE,
                OnSeAndVoiceVolumeSliderChanged, OnSeAndVoiceVolumeInputChanged);
        }

        // CriVolumeControlの作成
        private CriVolumeControl CreateVolumeControl(string label, float initialValue, CriAudioType audioType,
            UnityAction<float> onSliderChanged, UnityAction<string> onInputChanged)
        {
            var volumeControlObject = Instantiate(_volumeControlPrefab, _volumeControlsParent);
            volumeControlObject.Initialize(label, initialValue, audioType, onSliderChanged, onInputChanged);
            return volumeControlObject;
        }

        // マスターボリュームのスライダーが変更された時の処理
        private void OnMasterVolumeSliderChanged(float value)
        {
            _criAudioManager.MasterVolume.Value = value / 100;
            _masterCriVolumeControl.SetVolume(value / 100);
        }

        // マスターボリュームの入力フィールドが変更された時の処理
        private void OnMasterVolumeInputChanged(string value)
        {
            if (float.TryParse(value, out float floatValue))
            {
                _criAudioManager.MasterVolume.Value = floatValue / 100;
                _masterCriVolumeControl.SetVolume(floatValue / 100);
            }
        }

        // BGMとMEのスライダーが変更された時の処理
        private void OnBgmAndMeVolumeSliderChanged(float value)
        {
            var bgmPlayer = _criAudioManager.GetPlayer(CriAudioType.CueSheet_BGM);
            if (bgmPlayer != null)
            {
                bgmPlayer.Volume.Value = value / 100;
                _bgmMeCriVolumeControl.SetVolume(value / 100);
            }

            var mePlayer = _criAudioManager.GetPlayer(CriAudioType.CueSheet_ME);
            if (mePlayer != null)
            {
                mePlayer.Volume.Value = value / 100;
                _bgmMeCriVolumeControl.SetVolume(value / 100);
            }
        }

        // BGMとMEの入力フィールドが変更された時の処理
        private void OnBgmAndMeVolumeInputChanged(string value)
        {
            if (float.TryParse(value, out float floatValue))
            {
                var bgmPlayer = _criAudioManager.GetPlayer(CriAudioType.CueSheet_BGM);
                if (bgmPlayer != null)
                {
                    bgmPlayer.Volume.Value = floatValue / 100;
                    _bgmMeCriVolumeControl.SetVolume(floatValue / 100);
                }

                var mePlayer = _criAudioManager.GetPlayer(CriAudioType.CueSheet_ME);
                if (mePlayer != null)
                {
                    mePlayer.Volume.Value = floatValue / 100;
                    _bgmMeCriVolumeControl.SetVolume(floatValue / 100);
                }
            }
        }

        // SEとVoiceのスライダーが変更された時の処理
        private void OnSeAndVoiceVolumeSliderChanged(float value)
        {
            var sePlayer = _criAudioManager.GetPlayer(CriAudioType.CueSheet_SE);
            if (sePlayer != null)
            {
                sePlayer.Volume.Value = value / 100;
                _seVoiceCriVolumeControl.SetVolume(value / 100);
            }

            var voicePlayer = _criAudioManager.GetPlayer(CriAudioType.CueSheet_VOICE);
            if (voicePlayer != null)
            {
                voicePlayer.Volume.Value = value / 100;
                _seVoiceCriVolumeControl.SetVolume(value / 100);
            }
        }

        // SEとVoiceの入力フィールドが変更された時の処理
        private void OnSeAndVoiceVolumeInputChanged(string value)
        {
            if (float.TryParse(value, out float floatValue))
            {
                var sePlayer = _criAudioManager.GetPlayer(CriAudioType.CueSheet_SE);
                if (sePlayer != null)
                {
                    sePlayer.Volume.Value = floatValue / 100;
                    _seVoiceCriVolumeControl.SetVolume(floatValue / 100);
                }

                var voicePlayer = _criAudioManager.GetPlayer(CriAudioType.CueSheet_VOICE);
                if (voicePlayer != null)
                {
                    voicePlayer.Volume.Value = floatValue / 100;
                    _seVoiceCriVolumeControl.SetVolume(floatValue / 100);
                }
            }
        }
    }
}
