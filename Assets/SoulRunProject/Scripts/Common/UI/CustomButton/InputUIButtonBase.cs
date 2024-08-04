using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SoulRun.InGame
{
    /// <summary> ボタンの表示状態を管理するボタンクラス </summary>
    public abstract class InputUIButtonBase : Selectable , ISubmitHandler
    {
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
        }

        /// <summary> ポインターがUIの上から離れるときに呼ばれる </summary>
        public override void OnDeselect(BaseEventData eventData)
        {
            base.OnDeselect(eventData);
        }

        public abstract void OnSubmit(BaseEventData eventData);
    }
}