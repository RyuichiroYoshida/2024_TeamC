using System.Threading;
using Cinemachine;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace SoulRunProject
{
    /// <summary>
    /// プレイヤーのカメラクラス
    /// </summary>
    public class PlayerCamera : MonoBehaviour
    {
        [SerializeField] private CinemachineImpulseSource _impulseSource;

        private void Awake()
        {

        }

        private void Update()
        {

        }

        public async UniTask DoStartIngameMove(CancellationToken cts)
        {
            return;
        }

        public void DamageCam()
        {
            _impulseSource.GenerateImpulse(Vector3.one);
        }
        
        public void StartFollowPlayer()
        {
        }
    }
}
