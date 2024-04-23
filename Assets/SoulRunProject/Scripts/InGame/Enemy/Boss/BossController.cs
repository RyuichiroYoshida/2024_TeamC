using System.Collections.Generic;
using SoulRunProject.Common;
using SoulRunProject.SoulMixScene;
using UnityEngine;

namespace SoulRunProject.InGame
{
    /// <summary>
    /// ボスの行動を管理するクラス
    /// 行動様式
    /// 最初に入場ムーブ
    /// 一行動を起こすたびに次の行動からランダムで実行する、
    /// </summary>
    public class BossController : MonoBehaviour
    {
        [SerializeField, Header("スコア")]
        private int _score = 100;
        
        [SerializeField, Tooltip("敵のパラメータを設定する")]
        protected Status _status;
        
        [SerializeField] LootTable _lootTable;
        [Header("ボスの行動"), SerializeReference, SubclassSelector] List<IBossBehavior> _bossBehaviors;

        private BossState _currentState = BossState.Animation;

        private void Start()
        {
            _status = _status.Copy();

            foreach (var behavior in _bossBehaviors)
            {
                behavior.Initialize();
            }

            _bossBehaviors[0].BeginAction();
        }
        
        private void Update()
        {
            if (Input.GetMouseButtonDown(1))
            {
                _bossBehaviors[0].BeginAction();
            }
            
            _bossBehaviors[0].UpdateAction(Time.deltaTime);
        }

        private enum BossState
        {
            Animation, // 登場時などのAnimation中
            Standby, // 行動待機中
            InAction // IBossBehaviorのAction中
        }
    }
    
    /// <summary>
    /// ボスの行動が持つインターフェース
    /// </summary>
    public interface IBossBehavior
    {
        void Initialize();
        void BeginAction();
        void UpdateAction(float deltaTime);
    }
}
