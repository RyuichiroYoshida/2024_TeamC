using DG.Tweening;
using SoulRunProject.Audio;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SoulRunProject
{
    public class TweenSlider : Slider
    {
        [SerializeField] private float _scaleMultiplier = 1.2f;
        private Vector3 _originalScale;
        private const float FadeTime = 0.2f;

        protected override void Awake()
        {
            base.Awake();
            _originalScale = transform.localScale;
        }

        public override void OnSelect(BaseEventData eventData)
        {
            base.OnSelect(eventData);
            CriAudioManager.Instance.Play(CriAudioType.CueSheet_SE, "SE_Select");
            transform.DOScale(_originalScale * _scaleMultiplier, FadeTime).SetLink(gameObject).SetUpdate(UpdateType.Normal, true);
        }

        public override void OnDeselect(BaseEventData eventData)
        {
            base.OnDeselect(eventData);
            transform.DOScale(_originalScale, FadeTime).SetLink(gameObject).SetUpdate(UpdateType.Normal, true);
        }
    }
}