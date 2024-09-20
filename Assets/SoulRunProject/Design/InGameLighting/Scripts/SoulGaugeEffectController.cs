using DG.Tweening;
using SoulRunProject.InGame;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace SoulRunProject
{
    public class SoulGaugeEffectController : MonoBehaviour
    {
        [SerializeField, ColorUsage(true, true)]
        private Color _changedColor;

        [SerializeField] private Image _image;
        [SerializeField] private float _processingTime = 1f;

        private Renderer _renderer;
        private Material _material;
        private Sequence _sequence;
        private SoulSkillManager _soulSkillManager;
        private Color _initialColor;
        private int _baseColorParameterID = Shader.PropertyToID("_BaseColor");

        private void OnEnable()
        {
            _sequence?.Kill();
            _material = _image.material;

            if (!_material.HasColor(_baseColorParameterID)) // パラメータ名が存在しているか
            {
                _baseColorParameterID = Shader.PropertyToID("_BaseColor");
            }

            _soulSkillManager = FindAnyObjectByType<SoulSkillManager>();
            _soulSkillManager.CurrentSoul.Where(currentSoul => currentSoul >= _soulSkillManager.RequiredSoul)
                .Subscribe(_ => SoulGaugeEffect()).AddTo(this);
            _soulSkillManager.CurrentSoul.Where(currentSoul => currentSoul < _soulSkillManager.RequiredSoul)
                .Subscribe(_ => _material.color = _initialColor).AddTo(this);
            _initialColor = _material.GetColor(_baseColorParameterID);
        }

        private void OnDisable()
        {
            _material.color = _initialColor;
        }

        public void SoulGaugeEffect()
        {
            _sequence?.Kill();
            _sequence.Append(DOTween.To(() => _material.GetColor(_baseColorParameterID),
                    color => _material.SetColor(_baseColorParameterID, color), _changedColor, _processingTime))
                .SetEase(Ease.OutBack).SetLink(gameObject);
        }
    }
}
