using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SoulRunProject.Common
{
    public class AoESkillLevelUpEventTableType : ILevelUpEventTableType
    {
        [SerializeField, Header("レベルアップイベントテーブル")] 
        List<AoESkillLevelUpEventGroup> _levelUpTable;
        public List<ILevelUpEventGroup> LevelUpTable => 
            _levelUpTable.OfType<ILevelUpEventGroup>().ToList();
    }
}