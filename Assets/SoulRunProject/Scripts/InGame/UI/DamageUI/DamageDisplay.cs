using System;
using DG.Tweening;
using SoulRunProject.Common;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

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
        [SerializeField, CustomLabel("UIのランダム位置範囲")] private Vector3 _randomPosRange;
        [SerializeField, CustomLabel("通常色")] private Color _nomalColor;
        [SerializeField, CustomLabel("クリティカル時の色")] private Color _critColor;
        [SerializeField] private float _damageToSizeRate;
        [SerializeField] private int _minSize;
        [SerializeField] private int _maxSize;

        /// <summary> 表示するダメージ </summary>
        private float _displayTotalDamage;
        private Transform _parent;
        private Vector3 _position;

        public bool WaitingForNextDamage { get; private set; }
        public Transform Parent => _parent;
        
        public override void Initialize(){}

        private void FixedUpdate()
        {
            if (WaitingForNextDamage && _parent)
            {
                _damageText.rectTransform.position = Camera.main.WorldToScreenPoint(_parent.position + _position);
            }
        }

        public void ResetDisplay(Transform tf, Vector3 position, float damage, bool isCrit)
        {
            // 表示のリセット
            _displayTotalDamage = damage;
            _damageText.text = ((int)_displayTotalDamage).ToString();
            _damageText.color = isCrit? _critColor : _nomalColor;
            _damageText.fontSize = Mathf.Clamp((int)(_damageToSizeRate * damage), _minSize, _maxSize);
            _parent = tf;
            _position = position + new Vector3(Random.Range(-_randomPosRange.x, _randomPosRange.x), 
                Random.Range(-_randomPosRange.y, _randomPosRange.y), 0);
            _damageText.rectTransform.position = Camera.main.WorldToScreenPoint(_parent.position + _position);

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
