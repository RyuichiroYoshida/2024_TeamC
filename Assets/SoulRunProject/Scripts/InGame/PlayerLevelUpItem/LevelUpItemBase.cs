using System;
using UnityEngine;
using UnityEngine.UI;
using SoulRunProject.Common;

namespace SoulRunProject.InGame
{
    /// <summary>
    /// レベルアップ時に獲得可能なアイテムのBase
    /// </summary>
    /// <typeparam name="T"> プレイヤー強化処理で使用する参照の型 </typeparam>
    /// プレイヤーの強化処理で欲しいScriptが違う
    public abstract class LevelUpItemBase<T>
    {
        [SerializeField] protected string _itemName;
        [SerializeField] protected Image _itemImage;
        protected T _reference;

        public string ItemName => _itemName;
        public Image ItemImage => _itemImage;

        /// <summary> プレイヤー強化処理で使用するScript参照を取得する </summary>
        /// <param name="reference"> プレイヤー強化処理で使用する参照 </param>
        public void GetReference(T reference)
        {
            _reference = reference;
        }
        
        /// <summary> アイテムの効果処理 </summary>
        public abstract void ItemEffect();
    }

    /// <summary>
    /// 特定スキルのレベルを上げるアイテム
    /// </summary>
    [Serializable]
    public class SkillLevelUpItem : LevelUpItemBase<SkillManager>
    {
        [SerializeField, Tooltip("レベルアップさせるスキル")] private PlayerSkill _skillToLevelUp;
        
        public override void ItemEffect()
        {
            if (_reference.CurrentSkillTypes.Contains(_skillToLevelUp)) // そのスキルを持っている
            {
                _reference.LevelUpSkill(_skillToLevelUp);
            }
            else // 持っていない
            {
                _reference.AddSkill(_skillToLevelUp);
            }
        }
    }

    /// <summary>
    /// プレイヤーのステータスを複数上げるアイテム
    /// </summary>
    [Serializable]
    public class StatusUpItem : LevelUpItemBase<PlayerManager>
    {
        [SerializeField] private StatusEffect[] ItemEffects;
        
        public override void ItemEffect()
        {
            foreach (var statusEffect in ItemEffects)
            {
                statusEffect.UpStatus(_reference);
            }   
        }

        [Serializable]
        class StatusEffect
        {
            [SerializeField, Tooltip("上げるステータス")] private StatusType _statusToUp;
            [SerializeField, Tooltip("上昇量")] private float _upValue;

            /// <summary> 指定されたステータスを指定された値上昇させる </summary>
            /// <param name="playerManager"> 強化するPlayerManager </param>
            public void UpStatus(PlayerManager playerManager)
            {
                switch (_statusToUp)
                {
                    case StatusType.Hp:
                        playerManager.PlayerStatus.Hp += (int)_upValue;
                        return;
                    case StatusType.攻撃力:
                        playerManager.PlayerStatus.Attack += (int)_upValue;
                        return;
                    case StatusType.防御力:
                        playerManager.PlayerStatus.Defence += (int)_upValue;
                        return;
                    case StatusType.クールタイム減少率:
                        playerManager.PlayerStatus.CoolTime += _upValue;
                        return;
                    case StatusType.スキル範囲増加率:
                        playerManager.PlayerStatus.Range += _upValue;
                        return;
                    case StatusType.弾速増加率:
                        playerManager.PlayerStatus.BulletSpeed += _upValue;
                        return;
                    case StatusType.追加効果時間:
                        playerManager.PlayerStatus.EffectTime += _upValue;
                        return;
                    case StatusType.追加弾数:
                        playerManager.PlayerStatus.BulletNum += (int)_upValue;
                        return;
                    case StatusType.貫通力:
                        playerManager.PlayerStatus.Penetration += _upValue;
                        return;
                    case StatusType.移動スピード:
                        playerManager.PlayerStatus.MoveSpeed += _upValue;
                        return;
                    case StatusType.成長速度:
                        playerManager.PlayerStatus.GrowthSpeed += _upValue;
                        return;
                    case StatusType.金運:
                        playerManager.PlayerStatus.Luck += _upValue;
                        return;
                    case StatusType.クリティカル率:
                        playerManager.PlayerStatus.CriticalRate += _upValue;
                        return;
                    case StatusType.クリティカルダメージ倍率:
                        playerManager.PlayerStatus.CriticalDamageRate += _upValue;
                        return;
                    case StatusType.ソウル吸収力:
                        playerManager.PlayerStatus.SoulAbsorption += _upValue;
                        return;
                    case StatusType.ソウル獲得率:
                        playerManager.PlayerStatus.SoulAcquisition += _upValue;
                        return;
                }
            }
        }
    }

    public enum StatusType
    {
        Hp,
        攻撃力,
        防御力,
        クールタイム減少率,
        スキル範囲増加率,
        弾速増加率,
        追加効果時間,
        追加弾数,
        貫通力,
        移動スピード,
        成長速度,
        金運,
        クリティカル率,
        クリティカルダメージ倍率,
        ソウル吸収力,
        ソウル獲得率
    }
}
