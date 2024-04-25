using System;
using SoulRunProject.Framework;
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
        [SerializeField] private FloatReactiveProperty _currentSoul = new FloatReactiveProperty(0);
        SoulSkillBase _currentSoulSkill;
        public float RequiredSoul;
        public IObservable<float> CurrentSoul => _currentSoul;

        private void Start()
        {
            //TODO デバック用　ソウルフレイム設定。
            if (MyRepository.Instance.TryGetDataList<SoulSkillBase>(out var dataList))
            {
                _currentSoulSkill = dataList[0];
            }
        }
        public void SetSoulSkill(SoulSkillBase soulSkill)
        {
            _currentSoulSkill = soulSkill;
            RequiredSoul = _currentSoulSkill.RequiredSoul;
        }
        
        public void AddSoul(float soul)
        {
            _currentSoul.Value += soul;
            if (_currentSoul.Value >= _currentSoulSkill.RequiredSoul)
            {
                _currentSoul.Value = _currentSoulSkill.RequiredSoul;
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
