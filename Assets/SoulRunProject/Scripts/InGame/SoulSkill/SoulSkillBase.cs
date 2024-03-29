using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using SoulRunProject.Common;
using UniRx;
using UnityEngine;

namespace SoulRunProject
{
    /// <summary>
    /// ソウル技のインターフェース
    /// </summary>
    [Serializable]
    public abstract class SoulSkillBase : MonoBehaviour
    {
        [SerializeField] protected SkillParameterBase _skillParameterBase;
        [SerializeField] private float _requiredSoul;
        private FloatReactiveProperty _currentSoul = new FloatReactiveProperty(0);
        public IObservable<float> OnCurrentSoulChanged => _currentSoul;
        public float RequiredSoul => _requiredSoul;
        
        public void AddSoul(float soul)
        {
            _currentSoul.Value += soul;
            if (_currentSoul.Value >= _requiredSoul)
            {
                _currentSoul.Value = _requiredSoul;
            }
        }

        public void UseSoulSkill()
        {
            if (_currentSoul.Value < _requiredSoul)
            {
                return;
            }
            _currentSoul.Value -= _requiredSoul;
            StartSoulSkill();
        }
        
        /// <summary>
        /// ソウル技を実行する
        /// </summary>
        public abstract void StartSoulSkill();
        
        /// <summary>
        /// ソウル技を一時停止する
        /// </summary>
        public abstract void PauseSoulSkill(bool isPause);
    }
}
