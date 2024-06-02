using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace SoulRunProject.InGame
{
    /// <summary>
    /// ダメージUIの表記管理
    /// </summary>
    public class DamageDisplay : PooledObject
    {
        [SerializeField] private Text _damageText;
        [SerializeField] private float _fadeDuration;
        
        public override void Initialize()
        {
            
        }

        public void ResetDisplay(Vector3 pos, float damage, Color color)
        {
            _damageText.text = ((int)damage).ToString();
            _damageText.color = color;
            _damageText.DOFade(0, _fadeDuration).OnComplete(Finish);
            _damageText.rectTransform.position = Camera.main.WorldToScreenPoint(pos);
            _damageText.rectTransform.DOMoveY(_damageText.rectTransform.position.y + 30, _fadeDuration); // todo 30
        }
    }
}
