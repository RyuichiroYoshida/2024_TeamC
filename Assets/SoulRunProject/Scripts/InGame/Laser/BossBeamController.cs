using System;
using SoulRunProject.Common;
using UnityEngine;

namespace SoulRunProject
{
    /// <summary>
    /// ボスのビームを制御する
    /// </summary>
    public class BossBeamController : MonoBehaviour
    {
        [SerializeField] private GameObject _flashObj;
        [SerializeField] private GameObject _hitEffectObj;
        [SerializeField] private float _maxLength;
        [SerializeField] private float _hitOffset;

        private LineRenderer _line;
        private ParticleSystem[] _hitEffects;
        private float _damage;
        private LayerMask _hitLayer;
        ///<summary> 一回の行動ですでにダメージを与えているか </summary>>
        private bool _afterHitting;
        /// <summary> 着弾地点 </summary>
        private Vector3 _impactPoint;
        private bool _isActiveBeam;

        /// <summary> 有効なビームかのフラグ </summary>
        public bool IsActiveBeam
        {
            set
            {
                _isActiveBeam = value;
                _line.enabled = value;
                _flashObj.SetActive(value);
                _hitEffectObj.SetActive(value);
            }
        }

        private void Awake()
        {
            _line = GetComponent<LineRenderer>();
            _hitEffects = _hitEffectObj.GetComponentsInChildren<ParticleSystem>();
            IsActiveBeam = false;
        }

        public void Initialize(float damage, LayerMask hitLayer)
        {
            _damage = damage;
            _hitLayer = hitLayer;
            _afterHitting = false;
        }

        private void Update()
        {
            if (!_line || !_isActiveBeam) return;
            
            _line.material.SetTextureScale("_MainTex", Vector2.one);                 
            _line.material.SetTextureScale("_Noise", Vector2.one);
            
            _line.SetPosition(0, transform.position);
            
            if (Physics.Raycast(transform.position, _impactPoint - transform.position, out var hit, _maxLength, _hitLayer))
            {
                _line.SetPosition(1, hit.point);

                _hitEffectObj.transform.position = hit.point + hit.normal * _hitOffset;
                _hitEffectObj.transform.LookAt(hit.point + hit.normal);

                foreach (var particle in _hitEffects)
                {
                    if (!particle.isPlaying) particle.Play();
                }

                if (hit.collider.TryGetComponent(out PlayerManager playerManager) && !_afterHitting)
                {
                    _afterHitting = true;
                    playerManager.Damage(_damage);
                }
            }
            else
            {
                var endPos = transform.position + transform.forward * _maxLength;
                _line.SetPosition(1, endPos);
                _hitEffectObj.transform.position = endPos;
                
                foreach (var particle in _hitEffects)
                {
                    if (particle.isPlaying) particle.Stop();
                }
            }
        }

        public void SetImpactPoint(Vector3 point)
        {
            _impactPoint = point;
        }
    }
}
