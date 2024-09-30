using DG.Tweening;
using SoulRunProject.Audio;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SoulRunProject
{
    public class OutlineSlider : Slider
    {
        [SerializeField] private Image _outline;
        [SerializeField] private float _fadeDuration = 0.25f;
        private Tween _outlineTween;
        protected override void Awake()
        {
            base.Awake();
            if (_outline)
            {
                var color = _outline.color;
                color.a = 0f;
                _outline.color = color;
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            _outlineTween?.Kill();
            if (_outline)
            {
                var color = _outline.color;
                color.a = 0f;
                _outline.color = color;
            }
        }

        public override void OnSelect(BaseEventData eventData)
        {
            base.OnSelect(eventData);
            _outlineTween?.Kill();
            CriAudioManager.Instance.Play(CriAudioType.CueSheet_SE, "SE_Select");
            if(_outline) _outlineTween = _outline.DOFade(1, _fadeDuration).SetLink(gameObject).SetUpdate(true);
        }

        public override void OnDeselect(BaseEventData eventData)
        {
            base.OnDeselect(eventData);
            _outlineTween?.Kill();
            if(_outline) _outlineTween = _outline.DOFade(0, _fadeDuration).SetLink(gameObject).SetUpdate(true);
        }
    }
    #if UNITY_EDITOR
    /// <summary>
    /// InspectorでOutlineSliderの変数を表示するためのクラス。これが無いと表示されない。
    /// </summary>
    [CustomEditor(typeof(OutlineSlider))]
    public class OutlineSliderEditor : Editor
    {
    }
    #endif
}