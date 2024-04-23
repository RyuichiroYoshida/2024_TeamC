using System;
using DG.Tweening;
using SoulRunProject.Common;
using UnityEngine;

namespace SoulRunProject.InGame
{
    public class BossBeamCleave : IBossBehavior
    {
        [SerializeField, CustomLabel("エフェクトプレハブ")] private GameObject _razer;
        [SerializeField, CustomLabel("ビーム原点")] private GameObject _beamOrigin;
        [SerializeField, CustomLabel("ビーム開始時着弾地点")] private Vector3 _startImpactPosition;
        [SerializeField, CustomLabel("ビーム終了時着弾地点")] private Vector3 _finishImpactPosition;
        [SerializeField, CustomLabel("ビーム時間")] private float _beamTime;

        private GameObject _laserInstance;
        private Vector3 _startRotate;
        private Vector3 _finishRotate;
        private float _percentageOfRotation;
        private float _timer;

        /// <summary> Action終了時に呼ばれる </summary>
        public Action OnFinishAction;

        public void Initialize()
        {
            _laserInstance = GameObject.Instantiate(_razer, _beamOrigin.transform);
            _laserInstance.SetActive(false);
            _startRotate = _startImpactPosition - _beamOrigin.transform.position;
            _finishRotate = _finishImpactPosition - _beamOrigin.transform.position;
        }
        
        public void BeginAction()
        {
            _laserInstance.SetActive(true);
            _percentageOfRotation = 0;
            _beamOrigin.transform.rotation = Quaternion.LookRotation(_startRotate);
            _beamOrigin.transform.DORotate(_finishRotate, _beamTime, RotateMode.Fast)
                .OnComplete(() => OnFinishAction?.Invoke());
        }

        public void UpdateAction(float deltaTime)
        {
            
        }
    }
}
