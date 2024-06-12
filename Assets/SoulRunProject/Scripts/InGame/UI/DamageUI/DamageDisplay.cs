using System;
using DG.Tweening;
using SoulRunProject.Common;
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
        [SerializeField, CustomLabel("合計ダメージとして加算する待機時間")] private float _timeToWaitNextDamage;

        /// <summary> 表示するダメージ </summary>
        private float _displayTotalDamage;
        private Transform _parent;
        private Vector3 _position;

        public bool WaitingForNextDamage { get; private set; }
        
        public override void Initialize(){}

        private void FixedUpdate()
        {
            if (WaitingForNextDamage)
            {
                _damageText.rectTransform.position = Camera.main.WorldToScreenPoint(_parent.position + _position);
            }
        }

        public void ResetDisplay(Transform tf, Vector3 position, float damage, Color color)
        {
            // 表示のリセット
            _displayTotalDamage = damage;
            _damageText.text = ((int)_displayTotalDamage).ToString();
            _damageText.color = color;
            _parent = tf;
            _position = position;

            // ダメージ待機
            WaitingForNextDamage = true;
            Invoke(nameof(FadeDisplay), _timeToWaitNextDamage);
        }

        void FadeDisplay()
        {
            WaitingForNextDamage = false;
            _damageText.DOFade(0, _fadeDuration).OnComplete(Finish);
            _damageText.rectTransform.DOMoveY(_damageText.rectTransform.position.y + 30, _fadeDuration); // todo 30
        }
        
        /// <summary> すでに表示中のUIに対する追加ダメージ </summary>
        /// <param name="damage"></param>
        public void AddDisplayDamage(float damage)
        {
            // 待機時間のリセット
            CancelInvoke(nameof(FadeDisplay));
            Invoke(nameof(FadeDisplay), _timeToWaitNextDamage);

            // 表示更新
            _displayTotalDamage += damage;
            _damageText.text = ((int)_displayTotalDamage).ToString();
        }
    }
}
