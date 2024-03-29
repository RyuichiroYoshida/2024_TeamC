using SoulRunProject.Common;
using UniRx;
using UnityEngine;

namespace SoulRunProject.InGame
{
    public class InGameExp : DropBase
    {
        [SerializeField] private int _exp;
        protected override void PickUp(PlayerManager playerManager)
        {
            playerManager.GetExp(_exp);
            FinishedSubject.OnNext(Unit.Default);
        }
    }
}
