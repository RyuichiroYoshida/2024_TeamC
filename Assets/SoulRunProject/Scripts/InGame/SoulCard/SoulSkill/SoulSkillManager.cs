using System;
using SoulRunProject.Audio;
using SoulRunProject.Framework;
using UniRx;
using UnityEngine;

namespace SoulRunProject.InGame
{
    /// <summary>
    /// ソウル技を管理するクラス
    /// </summary>
    public class SoulSkillManager : MonoBehaviour
    {
        [SerializeField] private SoulSkillBase _currentSoulSkill;
        [SerializeField] private float _initialSoul;
        [SerializeField] private float _regenerativePerSec;
        
        private FloatReactiveProperty _currentSoul = new FloatReactiveProperty(0);
        private float _usingSkillTimer;
        public float RequiredSoul => _currentSoulSkill.RequiredSoul;
        public IObservable<float> CurrentSoul => _currentSoul;

        private void Start()
        {
            _currentSoul.AddTo(this);
            AddSoul(_initialSoul);
        }

        private void FixedUpdate()
        {
            if (_usingSkillTimer > 0)
            {
                _usingSkillTimer -= Time.fixedDeltaTime;
                _currentSoul.Value -= RequiredSoul / _currentSoulSkill.Duration * Time.fixedDeltaTime;
            }
            
            AddSoul(_regenerativePerSec * Time.fixedDeltaTime);
        }
        
        public void AddSoul(float soul)
        {
            if (_usingSkillTimer > 0) return;
            
            if (_currentSoul.Value >= RequiredSoul)
            {
                _currentSoul.Value = RequiredSoul;
                return;
            }
            
            _currentSoul.Value += soul;
            
            if (_currentSoul.Value >= RequiredSoul)
            {
                _currentSoul.Value = RequiredSoul;
                CriAudioManager.Instance.Play(CriAudioType.CueSheet_SE, "SE_SoulSkillAvailable");
            }
        }
        
        public void UseSoulSkill()
        { 
            DebugClass.Instance.ShowLog($"現在のソウル値：{_currentSoul.Value}/必要ソウル値：{RequiredSoul}");
            //TODO 処理の前後をずらしてテストように呼び出せるようにしている
            if (_currentSoul.Value < RequiredSoul || _usingSkillTimer > 0)
            {
                Debug.Log("ソウルが足りません。");
                return;
            }
            CriAudioManager.Instance.Play(CriAudioType.CueSheet_VOICE, "VOICE_SoulSkill");
            _currentSoulSkill.StartSoulSkill();
            _usingSkillTimer = _currentSoulSkill.Duration;
        }
    }
}
