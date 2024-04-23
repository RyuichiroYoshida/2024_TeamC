using System;
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
        [Header("性能")]
        [SerializeField, CustomLabel("薙ぎ払い時間")] private float _beamTime;
        [SerializeField, CustomLabel("ダメージ")] private float _damage;
        [SerializeField] private int _hitCount; // 多段ヒットなのか1回だけなのか

        private GameObject _laserInstance;
        private Vector3 _startVector;
        private Vector3 _finishVector;
        private float _timer;
        private int _hitCounter;

        /// <summary> Action終了時に呼ばれる </summary>
        public Action OnFinishAction;

        public void Initialize()
        {
            _laserInstance = GameObject.Instantiate(_razer, _beamOrigin.transform);
            _laserInstance.SetActive(false);
            _startVector = _startImpactPosition - _beamOrigin.transform.position;
            _finishVector = (_finishImpactPosition - _beamOrigin.transform.position);
            
            Debug.Log(_beamOrigin.transform.position);
        }
        
        public void BeginAction()
        {
            _laserInstance.SetActive(true);
            _timer = 0;
            _hitCounter = 0;
            _beamOrigin.transform.rotation = Quaternion.LookRotation(_startVector);
        }

        public void UpdateAction(float deltaTime)
        {
            // 角度とタイマーの計算
            _timer += deltaTime;

            if (_timer < _beamTime)
            {
                _beamOrigin.transform.rotation = 
                    Quaternion.LookRotation(Vector3.Slerp(_startVector, _finishVector, _timer / _beamTime));
            }
            else
            {
                // todo : finish action
                OnFinishAction?.Invoke();
            }
            
            // 当たり判定
            if (Physics.Raycast(_beamOrigin.transform.position, _beamOrigin.transform.forward, out RaycastHit hit))
            {
                if (hit.collider.gameObject.TryGetComponent(out PlayerManager playerManager))
                {
                    if (_hitCounter < _hitCount)
                    {
                        _hitCounter++;
                        playerManager.Damage(_damage);
                    }
                }
            }
        }
    }
}
