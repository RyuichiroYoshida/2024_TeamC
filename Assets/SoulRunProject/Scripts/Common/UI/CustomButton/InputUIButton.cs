using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SoulRun.InGame
{
    /// <summary> ボタンの表示状態を管理するボタンクラス View</summary>
    [RequireComponent(typeof(CanvasGroup))]
    public class InputUIButton : InputUIButtonBase
    {
        [SerializeField] private float _scaleMultiplier = 1.2f;
        private CanvasGroup _button;
        private Vector3 _originalScale;
        private const float FadeTime = 0.2f;

        protected override void Awake () 
        {
            _button = GetComponent<CanvasGroup>();
            _originalScale = transform.localScale;
        }
        public override void OnSubmit(BaseEventData eventData)
        {
            base.OnSubmit(eventData);
            // DOTweenを使ってスケールを小さくするアニメーションを実行
            transform.DOScale(_originalScale * 0.8f, FadeTime).SetLink(gameObject).SetUpdate(UpdateType.Normal, true);
            _button.alpha = 0.5f;
            transform.DOScale(_originalScale, FadeTime).SetLink(gameObject).SetUpdate(UpdateType.Normal, true);
            _button.alpha = 1f;
        }

        public override void OnSelect(BaseEventData eventData)
        {
            base.OnSelect(eventData);
            transform.DOScale(_originalScale * _scaleMultiplier, FadeTime).SetLink(gameObject).SetUpdate(UpdateType.Normal, true);
        }

        public override void OnDeselect(BaseEventData eventData)
        {
            base.OnDeselect(eventData);
            transform.DOScale(_originalScale, FadeTime).SetLink(gameObject).SetUpdate(UpdateType.Normal, true);
            _button.alpha = 1f;
        }
    }
}