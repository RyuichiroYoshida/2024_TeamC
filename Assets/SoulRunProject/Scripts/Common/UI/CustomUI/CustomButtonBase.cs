using System;
using SoulRunProject.Audio;
using UniRx;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SoulRun.InGame
{
    /// <summary> ボタンの表示状態を管理するボタンクラス </summary>
    public abstract class CustomButtonBase : Selectable , ISubmitHandler
    {
        private readonly Subject<Unit> _onClickSubject = new Subject<Unit>();
        public IObservable<Unit> OnClick => _onClickSubject;

        /// <summary> ポインターがUIの上にあるときに呼ばれる </summary>
        public override void OnSelect(BaseEventData eventData)
        {
            base.OnSelect(eventData);
            CriAudioManager.Instance.Play(CriAudioType.CueSheet_SE, "SE_Select");
        }

        public virtual void OnSubmit(BaseEventData eventData)
        {
            CriAudioManager.Instance.Play(CriAudioType.CueSheet_SE, "SE_Decision");
            _onClickSubject.OnNext(Unit.Default);
        }
        public override void OnPointerDown(PointerEventData eventData)
        {
            CriAudioManager.Instance.Play(CriAudioType.CueSheet_SE, "SE_Decision");
            _onClickSubject.OnNext(Unit.Default);
        }
    }
}