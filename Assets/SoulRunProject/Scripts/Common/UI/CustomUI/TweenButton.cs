using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SoulRun.InGame
{
    /// <summary> ボタンの表示状態を管理するボタンクラス View</summary>
    [RequireComponent(typeof(CanvasGroup))]
    public class TweenButton : CustomButtonBase
    {
        [SerializeField] private float _scaleMultiplier = 1.2f;
        private CanvasGroup _button;
        private Vector3 _originalScale;
        private Tween _tween;
        private const float FadeTime = 0.2f;
        protected override void Awake () 
        {
            _button = GetComponent<CanvasGroup>();
            _originalScale = transform.localScale;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            _tween?.Kill();
            transform.localScale = _originalScale;
            _button.alpha = 1f;
        }

        public override void OnSubmit(BaseEventData eventData)
        {
            base.OnSubmit(eventData);
            _tween?.Kill();
            // DOTweenを使ってスケールを小さくするアニメーションを実行
            var sequence = DOTween.Sequence();
            sequence.Append(transform.DOScale(_originalScale * 0.8f, FadeTime).SetLink(gameObject)
                .SetUpdate(UpdateType.Normal, true));
            sequence.AppendCallback(() => _button.alpha = 0.5f);
            sequence.Append(transform.DOScale(_originalScale, FadeTime).SetLink(gameObject)
                .SetUpdate(UpdateType.Normal, true));
            sequence.AppendCallback(() => _button.alpha = 1f);
            sequence.SetLink(gameObject);
            _tween = sequence;
        }

        public override void OnSelect(BaseEventData eventData)
        {
            base.OnSelect(eventData);
            _tween?.Kill();
            _tween = transform.DOScale(_originalScale * _scaleMultiplier, FadeTime).SetLink(gameObject).SetUpdate(UpdateType.Normal, true);
        }

        public override void OnDeselect(BaseEventData eventData)
        {
            base.OnDeselect(eventData);
            _tween?.Kill();
            _tween = transform.DOScale(_originalScale, FadeTime).SetLink(gameObject).SetUpdate(UpdateType.Normal, true);
            _button.alpha = 1f;
        }
    }
}