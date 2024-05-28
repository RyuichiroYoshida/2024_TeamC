using System;
using System.Collections.Generic;
using System.Linq;
using SoulRunProject.Framework;
using SoulRunProject.SoulMixScene;
using SoulRunProject.SoulRunProject.Scripts.Common.Core.Singleton;
using UniRx;
using UnityEngine;

namespace SoulRunProject.InGame
{
    /// <summary>
    /// ソウル技を管理するクラス
    /// </summary>
    public class SoulSkillManager : MonoBehaviour
    {
        [SerializeField] private float _initialSoul;
        [SerializeField] private float _regenerativePerSec;
        
        private FloatReactiveProperty _currentSoul = new FloatReactiveProperty(0);
        private readonly Dictionary<SoulSkillType , SoulSkillBase> _soulSkillReference = new();
        SoulSkillBase _currentSoulSkill;
        public float RequiredSoul { get; private set; }
        public IObservable<float> CurrentSoul => _currentSoul;

        private void Start()
        {
            _currentSoul.AddTo(this);
            _currentSoul.Value = _initialSoul;
            
            //TODO デバック用　ソウルフレイム設定。
            if (MyRepository.Instance.TryGetDataList<SoulSkillBase>(out var dataList))
            {
                foreach (var soulSkill in dataList)
                {
                    _soulSkillReference.Add(soulSkill.SkillType , soulSkill);
                }
            }

            SetSoulSkill(SoulSkillType.SoulFrame);
        }

        private void FixedUpdate()
        {
            AddSoul(_regenerativePerSec * Time.fixedDeltaTime);
        }

        public void SetSoulSkill(SoulSkillType soulSkillType)
        {
            _currentSoulSkill = _soulSkillReference[soulSkillType];
            RequiredSoul = _currentSoulSkill.RequiredSoul;
        }
        
        public void AddSoul(float soul)
        {
            _currentSoul.Value += soul;
            if (_currentSoul.Value >= RequiredSoul)
            {
                _currentSoul.Value = RequiredSoul;
            }
        }
        
        public void UseSoulSkill()
        { 
            DebugClass.Instance.ShowLog($"現在のソウル値：{_currentSoul.Value}/必要ソウル値：{RequiredSoul}");
            //TODO 処理の前後をずらしてテストように呼び出せるようにしている
            if (_currentSoul.Value < RequiredSoul)
            {
                Debug.Log("ソウルが足りません。");
                return;
            }
            _currentSoul.Value -= RequiredSoul;
            _currentSoulSkill.StartSoulSkill();
        }
    }
}
