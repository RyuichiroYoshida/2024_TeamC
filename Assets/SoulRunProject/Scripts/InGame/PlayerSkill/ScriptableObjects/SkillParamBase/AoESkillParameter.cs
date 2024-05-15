using System;
using SoulRunProject.SoulMixScene;
using UnityEngine;

namespace SoulRunProject.Common
{
    [Serializable , Name("範囲スキルパラメータ")]
    public class AoESkillParameter : ISkillParameter
    {
        [SerializeField, CustomLabel("敵にヒットしたときに与えるダメージ")] float _attackDamage;
        [SerializeField, CustomLabel("スキルのオブジェクトの大きさ")] float _size;
        
        [NonSerialized] public float BaseAttackDamage;
        [NonSerialized] public float BaseSize;
        
        public void InitializeParamOnSceneLoaded()
        {
            BaseAttackDamage = _attackDamage;
            BaseSize = _size;
        }
        
        private PlayerStatus _status;
        public void SetPlayerStatus(in PlayerStatus status)
        {
            _status = status;
        }
        

        public float AttackDamage => BaseAttackDamage + _status.AttackValue;
        public float Size => BaseSize * _status.SkillSizeUpRate;
    }
}