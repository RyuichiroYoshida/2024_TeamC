using System;
using UnityEngine;

namespace SoulRunProject
{
    /// <summary>
    /// HovlStudioのエフェクトを制御する
    /// </summary>
    public class LaserEffectController : MonoBehaviour
    {
        [SerializeField] private GameObject _flashEffectObj;
        [SerializeField] private GameObject _hitEffectObj;
        [SerializeField] private float _maxLength;
        [SerializeField] private float _hitOffset;

        private LineRenderer _line;
        private ParticleSystem[] _flashEffects;
        private ParticleSystem[] _hitEffects;

        public LayerMask HitLayer { get; set; }

        private void Awake()
        {
            _line = GetComponent<LineRenderer>();
            _flashEffects = _flashEffectObj.GetComponentsInChildren<ParticleSystem>();
            _hitEffects = _hitEffectObj.GetComponentsInChildren<ParticleSystem>();
        }

        private void Update()
        {
            _line.material.SetTextureScale("_MainTex", Vector2.one);                 
            _line.material.SetTextureScale("_Noise", Vector2.one);

            if (!_line) return;
            
            _line.SetPosition(0, transform.position);
            
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out var hit, _maxLength, HitLayer))
            {
                _line.SetPosition(1, hit.point);

                _hitEffectObj.transform.position = hit.point + hit.normal * _hitOffset;
                _hitEffectObj.transform.LookAt(hit.point + hit.normal);

                foreach (var particle in _hitEffects)
                {
                    if (!particle.isPlaying) particle.Play();
                }
                
                // Length[0] = MainTextureLength * Vector3.Distance(transform.position, hit.point);
                // Length[2] = NoiseTextureLength * Vector3.Distance(transform.position, hit.point);
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
                
                // Length[0] = MainTextureLength * (Vector3.Distance(transform.position, endPos));
                // Length[2] = NoiseTextureLength * (Vector3.Distance(transform.position, endPos));
            }
        }
    }
}
