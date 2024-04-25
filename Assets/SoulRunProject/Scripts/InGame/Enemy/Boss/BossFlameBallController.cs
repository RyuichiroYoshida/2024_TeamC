using SoulRunProject.Common;
using UniRx;
using UniRx.Triggers;
using Unity.Mathematics;
using UnityEngine;

namespace SoulRunProject.InGame
{
    public class BossFlameBallController : MonoBehaviour
    {
        [SerializeField, CustomLabel("炎の床プレハブ")] private BossFlameFloorController _flameFloorPrefab;
        
        private float _flameFloorDamage;
        
        public void Initialize(float speed, float flameFloorDamage)
        {
            _flameFloorDamage = flameFloorDamage;
            
            // move
            this.FixedUpdateAsObservable()
                .TakeUntilDestroy(this)
                .Subscribe(_ =>
                {
                    transform.position += transform.forward * speed * Time.fixedDeltaTime;
                });
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.TryGetComponent(out FieldSegment field))
            {
                BossFlameFloorController flameFloor = Instantiate(_flameFloorPrefab, other.gameObject.transform);
                flameFloor.transform.position = gameObject.transform.position; //
                flameFloor.Initialize(_flameFloorDamage);
                Destroy(gameObject);
            }
        }
    }
}
