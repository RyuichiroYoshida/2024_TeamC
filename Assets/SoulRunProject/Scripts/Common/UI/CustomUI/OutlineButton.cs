using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SoulRun.InGame
{
    public class OutlineButton : CustomButtonBase
    {
        [SerializeField] private Image _outlineImage;
        [SerializeField] private float _fadeDuration = 0.25f;
        private Tween _tween;
        protected override void Awake()
        {
            base.Awake();
            if (_outlineImage)
            {
                var color = _outlineImage.color;
                color.a = 0f;
                _outlineImage.color = color;
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            if (_outlineImage)
            {
                _tween?.Kill();
                var color = _outlineImage.color;
                color.a = 0f;
                _outlineImage.color = color;
            }
        }

        public override void OnSelect(BaseEventData eventData)
        {
            base.OnSelect(eventData);
            if (_outlineImage)
            {
                _tween?.Kill();
                _tween = _outlineImage.DOFade(1, _fadeDuration).SetLink(gameObject).SetUpdate(true);
            }
        }

        public override void OnDeselect(BaseEventData eventData)
        {
            base.OnDeselect(eventData);
            if (_outlineImage)
            {
                _tween?.Kill();
                _tween = _outlineImage.DOFade(0, _fadeDuration).SetLink(gameObject).SetUpdate(true);
            }
        }
    }
}