using System;
using DG.Tweening;
using SoulRunProject.Common;
using UnityEngine;

namespace SoulRunProject.InGame
{
    [Serializable, Name("カーブ移動")]
    public class CurveMover : EntityMover
    {
        [SerializeField, CustomLabel("経由地点")] GameObject _middlePosMarker;
        [SerializeField, CustomLabel("終了地点")] GameObject _endPosMarker;
        Tweener _tweener;
        bool _onceFlag;
        Vector3 _middlePos;
        Vector3 _endPos;

        public override void OnStart()
        {
            _onceFlag = false;
            _endPos = _endPosMarker.GetComponent<Transform>().position;
            _middlePos = _middlePosMarker.GetComponent<Transform>().position;
            _endPosMarker.SetActive(false);
            _middlePosMarker.SetActive(false);
        }

        public override void OnUpdateMove(Transform myTransform, Transform playerTransform)
        {
            // Tweenは一度だけ動かしたい
            if (_onceFlag)
            {
                return;
            }

            _tweener = myTransform.DOLocalPath
                (
                    path: new[] { myTransform.position, _middlePos, _endPos },
                    duration: _moveSpeed,
                    PathType.CatmullRom
                )
                .SetOptions
                (
                    closePath: false,
                    lockRotation: AxisConstraint.X
                );
            _onceFlag = true;
        }

        public override void Pause()
        {
            _tweener.Pause();
        }

        public override void Resume()
        {
            _tweener.Play();
        }
    }
}