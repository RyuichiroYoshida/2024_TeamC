using System;
using SoulRunProject.SoulMixScene;
using UnityEngine;

namespace SoulRunProject.Common
{
    public class ShieldSkillParameter : ISkillParameter
    {
        [SerializeField, CustomLabel("次にこのスキルを使えるまでの時間")] float _coolTime;
        [SerializeField, CustomLabel("ダメージ無効化回数")] int _shieldCount;
        
        [NonSerialized] public float BaseCoolTime;
        [NonSerialized] public int BaseShieldCount;
        public void InitializeParamOnSceneLoaded()
        {
            BaseCoolTime = _coolTime;
            BaseShieldCount = _shieldCount;
        }

        public float CoolTime => BaseCoolTime * _status.CoolTimeReductionRate;
        public float ShieldCount => BaseShieldCount;

        private PlayerStatus _status;
        public void SetPlayerStatus(in PlayerStatus status)
        {
            _status = status;
        }

    }
}