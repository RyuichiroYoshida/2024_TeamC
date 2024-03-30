using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SoulRunProject.Common
{
    [Serializable]
    public class ProjectileSkillLevelUpEventTableType : ILevelUpEventTableType
    {
        [SerializeField, Header("レベルアップイベントテーブル")] 
        List<ProjectileSkillLevelUpEventGroup> _levelUpTable;
        public List<ILevelUpEventGroup> LevelUpTable => 
            _levelUpTable.OfType<ILevelUpEventGroup>().ToList();
    }
}