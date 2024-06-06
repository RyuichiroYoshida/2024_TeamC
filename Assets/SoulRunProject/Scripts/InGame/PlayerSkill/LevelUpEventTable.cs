using System;
using System.Collections.Generic;

namespace SoulRunProject.Common
{
    [Serializable]
    public class LevelUpEventTable<T> where T : ILevelUpEvent
    {
        public List<LevelUpEventList<T>> List;
    }
    [Serializable]
    public class LevelUpEventList<T> where T : ILevelUpEvent
    {
        public List<T> List;
    }
}