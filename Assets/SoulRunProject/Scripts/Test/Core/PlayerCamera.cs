using System.Threading;
using Cinemachine;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace SoulRunProject
{
    /// <summary>
    /// プレイヤーのカメラクラス
    /// </summary>
    public class PlayerCamera : MonoBehaviour
    {
        [SerializeField] private CinemachineImpulseSource _impulseSource;
        [SerializeField] private float _impulseMagnitude;

        public async UniTask DoStartIngameMove(CancellationToken cts)
        {
            return;
        }

        public void DamageCam()
        {
            _impulseSource.GenerateImpulse(Vector3.one * _impulseMagnitude);
        }
        
        public void StartFollowPlayer()
        {
        }
    }
}
