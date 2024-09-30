using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using SoulRunProject.Common;
using UnityEngine;

namespace SoulRunProject.InGame
{
    [Serializable]
    [Name("パス移動")]
    public class PathMover : EntityMover
    {
        [SerializeField, CustomLabel("経由地点")] private List<GameObject> _posMarkers;
        [SerializeField, CustomLabel("終了地点と始点を繋げる")] private bool _isPosLoop;
        [SerializeField, CustomLabel("現在位置から最初のパスまでの線形補間を行わない")] private bool _notFirstInterpolate;
        [SerializeField, CustomLabel("トウィーン終了時に初期位置に戻る")] private bool _returnToInitialPosition;
        [SerializeField, CustomLabel("Loop回数指定")] [Tooltip("-1で無限ループ")] private int _loopCount;
        [SerializeField, CustomLabel("LoopType設定")] private LoopType _type;
        [SerializeField, CustomLabel("イージングの種類")] private Ease _easeType = Ease.Linear;
        
        private Tweener _tweener;
        private Vector3[] _posArr;

        public override void OnStart(Transform myTransform = null, PlayerManager pm = null)
        {
            if (myTransform is null) return;
            _posArr = _posMarkers.Select(anchor => myTransform.TransformPoint(anchor.transform.localPosition)).ToArray();
            var initialPosition = myTransform.position;
            if (_notFirstInterpolate && _posArr.Length > 0)
            {
                myTransform.position = _posArr[0];
            }
            foreach (var item in _posMarkers) item.SetActive(false);
            _tweener = myTransform.DOLocalPath
                (
                    _posArr,
                    _moveSpeed,
                    PathType.CatmullRom,
                    gizmoColor: Color.red
                )
                .SetOptions
                (
                    false,
                    lockRotation: AxisConstraint.X
                )
                .SetOptions(_isPosLoop)
                .SetLoops(_loopCount, _type)
                .SetEase(_easeType)
                .SetLink(myTransform.gameObject, LinkBehaviour.KillOnDisable)
                .SetLink(myTransform.gameObject)
                .OnComplete(() =>
                {
                    if (_returnToInitialPosition) myTransform.position = initialPosition;
                    Complete();
                }).SetUpdate(UpdateType.Manual);
        }

        public override void OnUpdateMove(Transform myTransform, Transform playerTransform)
        {
            _tweener?.ManualUpdate(Time.deltaTime, Time.unscaledDeltaTime);
        }
    }
}