using Cysharp.Threading.Tasks;
using DG.Tweening;
using SoulRunProject.Common;
using UnityEngine;
using Sequence = DG.Tweening.Sequence;

namespace SoulRunProject.InGame
{
    /// <summary>
    /// 被ダメージ表現を行うクラス
    /// </summary>
    public class HitDamageEffectManager : MonoBehaviour
    {
        [SerializeField, CustomLabel("点滅間隔"), Range(0, 0.5f)] float _duration;
        [SerializeField, CustomLabel("点滅回数")] int _loopCount;
        [SerializeField, CustomLabel("ヒットエフェクト")] private ParticleSystem _hitEffect;
        [SerializeField] private float _dissolveFadeTime = 1f;
        
        Renderer _renderer;
        private Material _copyMaterial;
        Sequence _sequence;
        private readonly int _alphaClipThresholdID = Shader.PropertyToID("_AlphaClipThreshold");
        
        public Material CopyMaterial => _copyMaterial;

        /// <summary>
        /// マテリアルの複製を行う
        /// </summary>
        protected void Awake()
        {
            if (!TryGetComponent(out _renderer))
            {
                _renderer = GetComponentInChildren<Renderer>();
            }
            
            _copyMaterial = _renderer.material;

            if (_copyMaterial == null)
            {
                Debug.LogWarning($"{gameObject.name}のMaterialが正常にコピー出来ていません");
            }
        }

        /// <summary>
        /// 白色点滅メソッド
        /// </summary>
        public void HitFadeBlinkWhite()
        {
            HitFadeBlink();
        }

        /// <summary>
        /// 色指定点滅メソッド
        /// </summary>
        /// <param name="color">点滅色</param>
        public void HitFadeBlink()
        {
            _copyMaterial.SetBool("_Boolean", true);
            _sequence?.Kill();
            _sequence = DOTween.Sequence();
            _sequence.AppendInterval(_duration);
            _sequence.AppendCallback(() => _copyMaterial.SetBool("_Boolean", false));
            _sequence.SetLoops(_loopCount, LoopType.Restart);
            _sequence.SetLink(gameObject);//.SetLink(gameObject, LinkBehaviour.KillOnDisable);
            _sequence.Play();
            _sequence.SetUpdate(true);

            // ヒットエフェクトの再生
            if (_hitEffect) _hitEffect.Play();
        }

        private void OnEnable()
        {
            _copyMaterial.SetFloat(_alphaClipThresholdID, 0);
        }

        public async UniTask DissolveFade()
        {
            if (!_copyMaterial.HasFloat(_alphaClipThresholdID)) return;
            
            await DOTween.To(() => _copyMaterial.GetFloat(_alphaClipThresholdID),
                    a => _copyMaterial.SetFloat(_alphaClipThresholdID, a), 1f, _dissolveFadeTime)
                .SetUpdate(true);
        }

        private void OnDestroy()
        {
            Destroy(_copyMaterial);
        }
    }
}

namespace SoulRunProject.Common
{
    public static class MaterialExtension
    {
        // 参考 : https://bravememo.hatenablog.com/entry/2023/11/13/220646
        /// <summary>
        /// マテリアルのboolプロパティに値をセットするメソッド
        /// </summary>
        /// <param name="material">セットする対象</param>
        /// <param name="name">セットするプロパティ名</param>
        /// <param name="flag">セットする値</param>
        public static void SetBool(this Material material, string name, bool flag)
        {
            var num = flag ? 1 : 0;
            material.SetInt(name, num);
        }
    }
}