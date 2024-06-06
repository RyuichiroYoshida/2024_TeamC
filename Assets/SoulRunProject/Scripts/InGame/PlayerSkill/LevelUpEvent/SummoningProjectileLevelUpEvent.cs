using UnityEngine;

namespace SoulRunProject.Common
{
    public class SummoningProjectileLevelUpEvent : ILevelUpEvent
    {
        enum LevelUpType
        {
            [InspectorName("何もない")] None1,
            [InspectorName("何もない")] None2
        }
        [SerializeField] private LevelUpType _levelUpType;
        public void LevelUp(in ISkillParameter skillParameter)
        {
            if (skillParameter is ProjectileSkillParameter param)
            {
                switch (_levelUpType)
                {
                    case LevelUpType.None1 :
                        //param.BaseCoolTime *= (100 - _reduceCoolTime) / 100;
                        break;
                    case LevelUpType.None2 :
                        //param.BaseShieldCount += _increaseShieldCount;
                        break;
                }
            }
            else
            {
                Debug.LogError($"{nameof(ProjectileSkillParameter)}にキャストできませんでした");
            }
        }
    }
}