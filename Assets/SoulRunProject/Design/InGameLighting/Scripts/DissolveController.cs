using Cysharp.Threading.Tasks;
using DG.Tweening;
using SoulRunProject.InGame;
using UnityEngine;

namespace SoulRunProject
{
    public class DissolveController : MonoBehaviour
    {
        //[SerializeField] private DamageableEntity _damageableEntity;
        [SerializeField] private float _fadeTime = 1f;

        private Renderer _renderer;
        //private Material _material;
        private Sequence _sequence;
        private int _alphaClipThresholdID = Shader.PropertyToID("_AlphaClipThreshold");

        private void Awake()
        {
            return;
            if (!TryGetComponent(out _renderer))
            {
                _renderer = GetComponentInChildren<Renderer>();
            }

            // if (!TryGetComponent(out _material))
            // {
            //     _material = _renderer.material;
            // }

            // if (!_material.HasFloat(_alphaClipThresholdID)) // パラメータ名が存在しているか
            // {
            //     _alphaClipThresholdID = Shader.PropertyToID("_AlphaClipThreshold");
            // }

            //_damageableEntity.OnDead += DissolveFade;
        }

        private void OnEnable()
        {
            return;
            _sequence?.Kill();
            _renderer.material.SetFloat(_alphaClipThresholdID, 0);
        }

        private void OnDestroy()
        {
            //_damageableEntity.OnDead -= DissolveFade;
        }

        /// <summary>ディゾルブ表現でのフェードアウトを行います</summary>
        public async UniTask DissolveFade()
        {
            if (!_renderer.material.HasFloat(_alphaClipThresholdID)) return;

            _sequence?.Kill();
            _sequence = DOTween.Sequence();
            _sequence.Append(DOTween.To(() => _renderer.material.GetFloat(_alphaClipThresholdID),
                a => _renderer.material.SetFloat(_alphaClipThresholdID, a), 1f, _fadeTime));
            _sequence.SetUpdate(true);
            await _sequence.Play();
        }
    }
}
