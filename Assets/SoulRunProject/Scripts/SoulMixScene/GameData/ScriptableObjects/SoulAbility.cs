using SoulRunProject.Common;
using UnityEngine;
using UnityEngine.Serialization;

namespace SoulRunProject.SoulMixScene
{
    [CreateAssetMenu(fileName = "SoulAbility", menuName = "SoulRunProject/SoulAbility")]
    public class SoulAbility : ScriptableObject
    {
        [SerializeField] private int _uniqueAbilityID;

        public int UniqueAbilityID
        {
            get => _uniqueAbilityID;
            set => _uniqueAbilityID = value;
        }

        // 技名
        [SerializeField] private string _abilityName;

        public string AbilityName
        {
            get => _abilityName;
            set => _abilityName = value;
        }

        // クールタイム
        [SerializeField] private float _coolTime;

        public float CoolTime
        {
            get => _coolTime;
            set => _coolTime = value;
        }

        // 技効果
        [SerializeField] private SkillBase _skillBase;

        public SkillBase SkillBase
        {
            get => _skillBase;
            set => _skillBase = value;
        }

        // 効果テキスト
        [SerializeField] private string _effectText;

        public string EffectText
        {
            get => _effectText;
            set => _effectText = value;
        }
    }
}