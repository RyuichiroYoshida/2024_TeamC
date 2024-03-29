using SoulRunProject.Common;
using UniRx;
using UnityEngine;

namespace SoulRunProject.InGame
{
    public class InGameCoin : DropBase
    {
        [SerializeField] private int _coin;
        protected override void PickUp(PlayerManager playerManager)
        {
            playerManager.ResourceContainer.Coin += _coin;
            FinishedSubject.OnNext(Unit.Default);
        }
    }
}
