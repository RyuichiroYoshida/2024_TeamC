using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace SoulRunProject.Common
{
    [Serializable, Name("通常被ノックバック処理")]
    public class TakeKnockBack
    {
        [SerializeField, Range(0, 100), Tooltip("ノックバック抵抗(%)")] float _resistanceValue;
        [SerializeField, Tooltip("再ノックバックするまでの時間")] float _coolTime;
        bool _isKnockBack;
        private Sequence _sequence;
        /// <summary>
        /// ノックバック処理
        /// </summary>
        public async void KnockBack(Transform myTransform , float power, Vector3 direction)
        {
            // ノックバックが終わるまで次のノックバックは行わない
            if (_isKnockBack)　return;
            _isKnockBack = true;
            direction *= power * (100 - _resistanceValue);
            _sequence = DOTween.Sequence();
            _sequence.Append(myTransform.DOBlendableMoveBy(new Vector3(direction.x, 0f ,direction.z), _coolTime));
            
            _sequence.Insert(0f , myTransform.DOBlendableMoveBy(new Vector3(0f , direction.y ,0f), _coolTime / 2));
            _sequence.Insert(_coolTime / 2 , myTransform.DOBlendableMoveBy(new Vector3(0f , - direction.y ,0f), _coolTime / 2));
            await _sequence;
            _sequence = null;
            _isKnockBack = false;
        }

        public void Pause()
        {
            _sequence?.Pause();
        }

        public void Resume()
        {
            _sequence?.Restart();
        }
    }
}