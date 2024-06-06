using System.Collections.Generic;
using UnityEngine;

namespace SoulRunProject.Common
{
    [CreateAssetMenu(menuName = "SoulRunProject/PlayerSkill/LaserSkill")]
    public class LaserSkillData : AbstractSkillData
    {
        //  プロパティ
        public override List<List<ILevelUpEvent>> LevelUpTable { get; }
    }
}