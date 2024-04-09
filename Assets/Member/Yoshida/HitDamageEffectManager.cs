using DG.Tweening;
using SoulRunProject.Common;
using UnityEngine;

namespace SoulRunProject.InGame
{
    /// <summary>
    /// 被ダメージ表現を行うクラス
    /// </summary>
    public class HitDamageEffectManager : MonoBehaviour
    {
        // シェーダーのカラープロパティ取得
        public static readonly int PramID = Shader.PropertyToID("_Color");

        // 良い感じの白色
        static readonly Color WhiteColor = new(0.85f, 0.85f, 0.85f, 0.6f);
        [SerializeField, Tooltip("点滅時間")] float _duration;
        [SerializeField, Tooltip("点滅回数")] int _loopCount;
        Renderer _renderer;
        Material _copyMaterial;
        Sequence _sequence;
        Color _defaultColor;
        bool _hitFadeBlinking;

        public Material CopyMaterial => _copyMaterial;
        public Color DefaultColor
        {
            get => _defaultColor;
            set => _defaultColor = value;
        }
        public bool HitFadeBlinking => _hitFadeBlinking;

        /// <summary>
        /// マテリアルの複製を行う
        /// </summary>
        protected virtual void Awake()
        {
            if (!TryGetComponent(out _renderer))
            {
                Debug.LogWarning($"{gameObject.name} のレンダラーがアタッチされていません");
                return;
            }

            var material = new Material(_renderer.material);
            _copyMaterial = material;
            _defaultColor = material.GetColor(PramID);
            _renderer.material = _copyMaterial;

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
            HitFadeBlink(WhiteColor);
        }

        /// <summary>
        /// 色指定点滅メソッド
        /// </summary>
        /// <param name="color">点滅色</param>
        public void HitFadeBlink(Color color)
        {
            _copyMaterial.SetBool("_Boolean", true);
            _sequence?.Kill();
            _sequence = DOTween.Sequence();
            _sequence.Append(DOTween.To(() => _defaultColor, c => _copyMaterial.SetColor(PramID, c), color, _duration));
            _sequence.Append(DOTween.To(() => color, c => _copyMaterial.SetColor(PramID, c), _defaultColor, _duration));
            _sequence.AppendCallback(() => _copyMaterial.SetBool("_Boolean", false));
            _sequence.SetLoops(_loopCount, LoopType.Restart);
            _sequence.SetLink(gameObject);
            _sequence.Play();
            _hitFadeBlinking = true;
            _sequence.OnComplete(() => _hitFadeBlinking = false);
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