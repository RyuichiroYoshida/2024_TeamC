using System;
using SoulRunProject.Common;
using UnityEditor;
using UnityEngine;

namespace SoulRunProject.InGame
{
    /// <summary>
    /// ノックバックを与えるDamageable
    /// </summary>
    [RequireComponent(typeof(DamageableEntity))]
    public class KnockBackDamageable : MonoBehaviour
    {
        [SerializeField, CustomLabel("向き")] private Direction _direction;
        [SerializeField] private float _duration;
        [SerializeField, CustomLabel("プレイヤーの半径")] private float _playerRadius;
        [SerializeField] private MeshCollider _mesh;

        private float _knockBackPosRight;
        private float _knockBackPosLeft;
        
        private void Awake()
        {
            GetComponent<DamageableEntity>().OnCollisionDamage += KnockBack;
        }

        private void OnEnable()
        {
            _knockBackPosRight = float.MinValue;
            _knockBackPosLeft = float.MaxValue;
            Vector3[] vertices = _mesh.sharedMesh.vertices;

            foreach (var v in vertices)
            {
                float worldPointX = transform.TransformPoint(v).x;

                if (worldPointX + _playerRadius > _knockBackPosRight) _knockBackPosRight = worldPointX + _playerRadius;
                else if (worldPointX - _playerRadius < _knockBackPosLeft) _knockBackPosLeft = worldPointX - _playerRadius;
            }
        }

        void KnockBack(PlayerManager playerManager)
        {
            if (_direction == Direction.Both)
            {
                float posX = playerManager.transform.position.x;
                // 近いほうにKnockBackする
                playerManager.TakeKnockBack(Math.Abs(_knockBackPosLeft - posX) < Math.Abs(_knockBackPosRight - posX) ?
                    _knockBackPosLeft : _knockBackPosRight, _duration);
            }
            else
            {
                playerManager.TakeKnockBack(_direction == Direction.Left ? _knockBackPosLeft : _knockBackPosRight, _duration);
            }
        }

        enum Direction
        {
            Left,
            Right,
            Both
        }
        
        #if UNITY_EDITOR
        [CustomEditor(typeof(KnockBackDamageable))]
        class KnockBackPos : Editor
        {
            private KnockBackDamageable _knockBackDamageable;

            private void OnEnable()
            {
                _knockBackDamageable = target as KnockBackDamageable;
            }

            private void OnSceneGUI()
            {
                
            }
        }
        #endif
    }
}
