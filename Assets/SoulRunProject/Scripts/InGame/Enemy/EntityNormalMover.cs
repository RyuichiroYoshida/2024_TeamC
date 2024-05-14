using System;
using SoulRunProject.Common;
using UnityEngine;

namespace SoulRunProject.InGame
{
    /// <summary>
    /// Enemyの通常移動処理の実装クラス
    /// </summary>
    [Serializable, Name("通常移動")]
    public class EntityNormalMover : EntityMover
    {
        [SerializeField, CustomLabel("移動方向(ワールド基準)")] private Direction _direction;
        bool _isPaused;

        public override void OnUpdateMove(Transform self, Transform target = default)
        {
            if (_isPaused) return;
            self.position += DirectionToVector3(_direction) * (_moveSpeed * Time.deltaTime);
        }
        public override void Pause()
        {
            _isPaused = true;
        }

        public override void Resume()
        {
            _isPaused = false;
        }

        Vector3 DirectionToVector3(Direction direction)
        {
            switch (direction)
            {
                case Direction.PositiveX :
                    return Vector3.right;
                case Direction.NegativeX :
                    return Vector3.left;
                case Direction.PositiveZ :
                    return Vector3.forward;
                case Direction.NegativeZ :
                    return Vector3.back;
            }
            return Vector3.zero;
        }
        enum Direction
        {
            [InspectorName("x正方向")]
            PositiveX,
            [InspectorName("x負方向")]
            NegativeX,
            [InspectorName("z正方向")]
            PositiveZ,
            [InspectorName("z負方向")]
            NegativeZ,
        }
    }
}