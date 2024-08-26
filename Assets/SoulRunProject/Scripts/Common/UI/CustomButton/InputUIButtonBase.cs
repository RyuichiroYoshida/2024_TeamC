using System;
using SoulRunProject.Audio;
using UniRx;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SoulRun.InGame
{
    /// <summary> ボタンの表示状態を管理するボタンクラス </summary>
    public abstract class InputUIButtonBase : Selectable , ISubmitHandler
    {
        private readonly Subject<Unit> _onClickSubject = new Subject<Unit>();
        public IObservable<Unit> OnClick => _onClickSubject;
        public override void OnPointerEnter(PointerEventData eventData)
        {
            OnSelect(eventData);
        }
        
        public override void OnPointerExit(PointerEventData eventData)
        {
            OnDeselect(eventData);
        }
        
        public override void OnPointerDown(PointerEventData eventData)
        {
            OnSubmit(eventData);
        }
        
        /// <summary> ポインターがUIの上にあるときに呼ばれる </summary>
        public override void OnSelect(BaseEventData eventData)
        {
            base.OnSelect(eventData);
            CriAudioManager.Instance.Play(CriAudioType.CueSheet_SE, "SE_Select");
        }

        /// <summary> ポインターがUIの上から離れるときに呼ばれる </summary>
        public override void OnDeselect(BaseEventData eventData)
        {
            base.OnDeselect(eventData);
        }

        public virtual void OnSubmit(BaseEventData eventData)
        {
            CriAudioManager.Instance.Play(CriAudioType.CueSheet_SE, "SE_Decision");
            _onClickSubject.OnNext(Unit.Default);
        }
    }
}