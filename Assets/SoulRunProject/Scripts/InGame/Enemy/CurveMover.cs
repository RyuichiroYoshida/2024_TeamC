using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using SoulRunProject.Common;
using UnityEngine;

namespace SoulRunProject.InGame
{
    [Serializable, Name("カーブ移動")]
    public class CurveMover : EntityMover
    {
        [SerializeField,CustomLabel("移動時間")] float _moveSpeed;
        [SerializeField, CustomLabel("経由地点")] List<GameObject> _posMarkers;

        [SerializeField, CustomLabel("終了地点と始点を繋げる")]
        bool _isPosLoop;

        [SerializeField, CustomLabel("Loop回数指定"), Tooltip("-1で無限ループ")]
        int _loopCount;

        [SerializeField, CustomLabel("LoopType設定")]
        LoopType _type;

        Tweener _tweener;
        bool _onceFlag;
        Vector3[] _posArr;

        public override void OnStart()
        {
            _onceFlag = false;

            foreach (var item in _posMarkers)
            {
                _posArr = _posMarkers.Select(target => target.transform.position).ToArray();
                item.SetActive(false);
            }
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
                path: _posArr,
                duration: _moveSpeed,
                PathType.CatmullRom
                )
                .SetOptions
                (
                    closePath: false,
                    lockRotation: AxisConstraint.X
                )
                .SetOptions(_isPosLoop)
                .SetLoops(_loopCount, _type)
                .SetEase(Ease.Linear)
                .SetLink(myTransform.gameObject);
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