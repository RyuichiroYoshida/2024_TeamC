// 日本語対応

using System.Collections.Generic;
using CriWare;
using UnityEngine.SceneManagement;
using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace SoulRunProject.Common
{
    public class CriAudioManager : AbstractSingletonMonoBehaviour<CriAudioManager>
    {
        [SerializeField] string streamingAssetsPathAcf = "SoulRun";
        [SerializeField] string cueSheetBGM = "CueSheet_BGM"; //.acb
        [SerializeField] string cueSheetSe = "CueSheet_SE"; //.acb

        [FormerlySerializedAs("cueSheetVoice")] [SerializeField]
        string cueSheetME = "CueSheet_ME"; //.acb

        private float _masterVolume = 1F;
        private float _bgmVolume = 1F;
        private float _seVolume = 1F;
        private float _meVolume = 1F;
        private const float Diff = 0.01F; //音量の変更があったかどうかの判定に使う

        /// <summary>マスターボリュームが変更された際に呼ばれるEvent</summary>
        public Action<float> MasterVolumeChanged;

        /// <summary>BGMボリュームが変更された際に呼ばれるEvent</summary>
        public Action<float> BGMVolumeChanged;

        /// <summary>SEボリュームが変更された際に呼ばれるEvent</summary>
        public Action<float> SEVolumeChanged;

        /// <summary>MEボリュームが変更された際に呼ばれる処理</summary>
        public Action<float> MEVolumeChanged;

        private CriAtomExPlayer _bgmPlayer;
        private CriAtomExPlayback _bgmPlayback;

        private CriAtomExPlayer _sePlayer;
        private CriAtomExPlayer _loopSEPlayer;
        private List<CriPlayerData> _seData;

        private CriAtomExPlayer _mePlayer;
        private List<CriPlayerData> _meData;

        private string _currentBGMCueName = "";
        private CriAtomExAcb _currentBGMAcb = null;


        /// <summary>マスターボリューム</summary>
        /// <value>変更したい値</value>
        public float MasterVolume
        {
            get => _masterVolume;
            set
            {
                if (!(_masterVolume + Diff < value) && !(_masterVolume - Diff > value)) return;
                MasterVolumeChanged.Invoke(value);
                _masterVolume = value;
            }
        }

        /// <summary>BGMボリューム</summary>
        /// <value>変更したい値</value>
        public float BGMVolume
        {
            get => _bgmVolume;
            set
            {
                if (!(_bgmVolume + Diff < value) && !(_bgmVolume - Diff > value)) return;
                BGMVolumeChanged.Invoke(value);
                _bgmVolume = value;
            }
        }

        /// <summary>マスターボリューム</summary>
        /// <value>変更したい値</value>
        public float SEVolume
        {
            get => _seVolume;
            set
            {
                if (!(_seVolume + Diff < value) && !(_seVolume - Diff > value)) return;
                SEVolumeChanged.Invoke(value);
                _seVolume = value;
            }
        }

        public float MEVolume
        {
            get => _meVolume;
            set
            {
                if (!(_meVolume + Diff < value) && !(_meVolume - Diff > value)) return;
                MEVolumeChanged.Invoke(value);
                _meVolume = value;
            }
        }

        /// <summary>SEのPlayerとPlaback</summary>
        private struct CriPlayerData
        {
            private CriAtomExPlayback _playback;
            private CriAtomEx.CueInfo _cueInfo;


            public CriAtomExPlayback Playback
            {
                get => _playback;
                set => _playback = value;
            }

            public CriAtomEx.CueInfo CueInfo
            {
                get => _cueInfo;
                set => _cueInfo = value;
            }

            public bool IsLoop
            {
                get => _cueInfo.length < 0;
            }
        }


        protected override bool UseDontDestroyOnLoad => true;

        /// <summary>CriAtom の追加。acb追加</summary>
        private void Awake()
        {
            // acf設定
            string path = Application.streamingAssetsPath + $"/{streamingAssetsPathAcf}.acf";
            CriAtomEx.RegisterAcf(null, path);
            // CriAtom作成
            new GameObject().AddComponent<CriAtom>();
            // BGM acb追加
            CriAtom.AddCueSheet(cueSheetBGM, $"{cueSheetBGM}.acb", null, null);
            // SE acb追加
            CriAtom.AddCueSheet(cueSheetSe, $"{cueSheetSe}.acb", null, null);
            //Voice acb追加
            CriAtom.AddCueSheet(cueSheetME, $"{cueSheetME}.acb", null, null);

            _bgmPlayer = new CriAtomExPlayer();
            _sePlayer = new CriAtomExPlayer();
            _loopSEPlayer = new CriAtomExPlayer();
            _mePlayer = new CriAtomExPlayer();
            _seData = new List<CriPlayerData>();
            _meData = new List<CriPlayerData>();

            MasterVolumeChanged += volume =>
            {
                _bgmPlayer.SetVolume(volume * _bgmVolume);
                _bgmPlayer.Update(_bgmPlayback);

                foreach (var se in _seData)
                {
                    if (se.IsLoop)
                    {
                        _loopSEPlayer.SetVolume(volume * _seVolume);
                        _loopSEPlayer.Update(se.Playback);
                    }
                    else
                    {
                        _sePlayer.SetVolume(volume * _seVolume);
                        _sePlayer.Update(se.Playback);
                    }
                }

                foreach (var voice in _meData)
                {
                    _mePlayer.SetVolume(_masterVolume * volume);
                    _mePlayer.Update(voice.Playback);
                }
            };

            BGMVolumeChanged += volume =>
            {
                _bgmPlayer.SetVolume(_masterVolume * volume);
                _bgmPlayer.Update(_bgmPlayback);
            };

            SEVolumeChanged += volume =>
            {
                foreach (var se in _seData)
                {
                    if (se.IsLoop)
                    {
                        _loopSEPlayer.SetVolume(_masterVolume * volume);
                        _loopSEPlayer.Update(se.Playback);
                    }
                    else
                    {
                        _sePlayer.SetVolume(_masterVolume * volume);
                        _sePlayer.Update(se.Playback);
                    }
                }
            };

            MEVolumeChanged += volume =>
            {
                foreach (var voice in _meData)
                {
                    _mePlayer.SetVolume(_masterVolume * volume);
                    _mePlayer.Update(voice.Playback);
                }
            };

            SceneManager.sceneUnloaded += Unload;
        }

        private void OnDestroy()
        {
            SceneManager.sceneUnloaded -= Unload;
        }
        // ここに音を鳴らす関数を書いてください

        /// <summary>BGMを開始する</summary>
        /// <param name="cueName">流したいキューの名前</param>
        public void PlayBGM(string cueName)
        {
            var temp = CriAtom.GetCueSheet(cueSheetBGM)?.acb;

            if (_currentBGMAcb == temp && _currentBGMCueName == cueName &&
                _bgmPlayer.GetStatus() == CriAtomExPlayer.Status.Playing)
            {
                return;
            }

            StopBGM();

            _bgmPlayer.SetCue(temp, cueName);
            _bgmPlayback = _bgmPlayer.Start();
            _currentBGMAcb = temp;
            _currentBGMCueName = cueName;
        }

        /// <summary>BGMを中断させる</summary>
        public void PauseBGM()
        {
            if (_bgmPlayer.GetStatus() == CriAtomExPlayer.Status.Playing)
            {
                _bgmPlayer.Pause();
            }
        }

        /// <summary>中断したBGMを再開させる</summary>
        public void ResumeBGM()
        {
            _bgmPlayer.Resume(CriAtomEx.ResumeMode.PausedPlayback);
        }

        /// <summary>BGMを停止させる</summary>
        public void StopBGM()
        {
            if (_bgmPlayer.GetStatus() == CriAtomExPlayer.Status.Playing)
            {
                _bgmPlayer.Stop();
            }
        }

        /// <summary>SEを流す関数</summary>
        /// <param name="cueName">流したいキューの名前</param>
        /// <param name="volume">音量</param>
        /// <returns>停止する際に必要なIndex</returns>
        public int PlaySE(string cueName, float volume = 1f)
        {
            CriPlayerData newAtomPlayer = new CriPlayerData();


            var tempAcb = CriAtom.GetCueSheet(cueSheetSe).acb;
            if (tempAcb == null)
            {
                Debug.LogWarning("ACBがNullです。");
                return -1;
            }

            tempAcb.GetCueInfo(cueName, out var cueInfo);

            newAtomPlayer.CueInfo = cueInfo;

            if (newAtomPlayer.IsLoop)
            {
                _loopSEPlayer.SetCue(tempAcb, cueName);
                _loopSEPlayer.SetVolume(volume * _seVolume * _masterVolume);
                newAtomPlayer.Playback = _loopSEPlayer.Start();
            }
            else
            {
                _sePlayer.SetCue(tempAcb, cueName);
                _sePlayer.SetVolume(volume * _seVolume * _masterVolume);
                newAtomPlayer.Playback = _sePlayer.Start();
            }

            _seData.Add(newAtomPlayer);
            return _seData.Count - 1;
        }

        /// <summary>SEをPauseさせる </summary>
        /// <param name="index">一時停止させたいPlaySE()の戻り値 (-1以下を渡すと処理を行わない)</param>
        public void PauseSE(int index)
        {
            if (index < 0) return;

            _seData[index].Playback.Pause();
        }

        /// <summary>PauseさせたSEを再開させる</summary>
        /// <param name="index">再開させたいPlaySE()の戻り値 (-1以下を渡すと処理を行わない)</param>
        public void ResumeSE(int index)
        {
            if (index < 0) return;

            _seData[index].Playback.Resume(CriAtomEx.ResumeMode.AllPlayback);
        }

        /// <summary>SEを停止させる </summary>
        /// <param name="index">止めたいPlaySE()の戻り値 (-1以下を渡すと処理を行わない)</param>
        public void StopSE(int index)
        {
            if (index < 0) return;

            _seData[index].Playback.Stop();
        }

        /// <summary>ループしているすべてのSEを止める</summary>
        public void StopLoopSE()
        {
            _loopSEPlayer.Stop();
        }

        /// <summary>MEを流す関数</summary>
        /// <param name="cueName">流したいキューの名前</param>
        /// <returns>停止する際に必要なIndex</returns>
        public int PlayME(string cueName, float volume = 1f)
        {
            CriAtomEx.CueInfo cueInfo;
            CriPlayerData newAtomPlayer = new CriPlayerData();

            var tempAcb = CriAtom.GetCueSheet(cueSheetME).acb;
            tempAcb.GetCueInfo(cueName, out cueInfo);

            newAtomPlayer.CueInfo = cueInfo;

            _mePlayer.SetCue(tempAcb, cueName);
            _mePlayer.SetVolume(volume * _masterVolume * _meVolume);
            newAtomPlayer.Playback = _mePlayer.Start();

            _meData.Add(newAtomPlayer);
            return _meData.Count - 1;
        }

        /// <summary>MEをPauseさせる </summary>
        /// <param name="index">一時停止させたいPlayVoice()の戻り値 (-1以下を渡すと処理を行わない)</param>
        public void PauseME(int index)
        {
            if (index < 0) return;

            _meData[index].Playback.Pause();
        }

        /// <summary>PauseさせたMEを再開させる</summary>
        /// <param name="index">再開させたいPlayME()の戻り値 (-1以下を渡すと処理を行わない)</param>
        public void ResumeME(int index)
        {
            if (index < 0) return;

            _meData[index].Playback.Resume(CriAtomEx.ResumeMode.AllPlayback);
        }

        /// <summary>MEを停止させる </summary>
        /// <param name="index">止めたいPlayME()の戻り値 (-1以下を渡すと処理を行わない)</param>
        public void StopME(int index)
        {
            if (index < 0) return;

            _meData[index].Playback.Stop();
        }

        private void Unload(Scene scene)
        {
            StopLoopSE();

            var removeIndex = new List<int>();
            for (int i = _seData.Count - 1; i >= 0; i--)
            {
                if (_seData[i].Playback.GetStatus() == CriAtomExPlayback.Status.Removed)
                {
                    removeIndex.Add(i);
                }
            }

            foreach (var i in removeIndex)
            {
                _seData.RemoveAt(i);
            }
        }
    }
}