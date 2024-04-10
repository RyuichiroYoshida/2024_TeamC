using System;
using UnityEngine;

namespace SoulRunProject.Common
{
    public class ShieldSkillParameter : ISkillParameter
    {
        [SerializeField, Header("次にこのスキルを使えるまでの時間")] float _coolTime;
        [SerializeField, Header("ダメージ無効化回数")] int _shieldCount;
        
        [NonSerialized] public float CoolTime;
        [NonSerialized] public int ShieldCount;
        public void InitializeParamOnSceneLoaded()
        {
            CoolTime = _coolTime;
            ShieldCount = _shieldCount;
        }
    }
}