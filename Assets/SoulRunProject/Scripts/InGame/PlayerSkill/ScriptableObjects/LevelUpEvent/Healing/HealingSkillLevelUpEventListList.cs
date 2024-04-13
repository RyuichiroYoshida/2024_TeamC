using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace SoulRunProject.Common
{
    [Serializable]
    public class HealingSkillLevelUpEventListList : ILevelUpEventListList
    {
        [SerializeField, Header("レベルアップイベントテーブル")] 
        List<HealingSkillLevelUpEventList> _levelUpEventListList;
        public List<ILevelUpEventList> LevelUpEventListList => 
            _levelUpEventListList.OfType<ILevelUpEventList>().ToList();
    }
}
