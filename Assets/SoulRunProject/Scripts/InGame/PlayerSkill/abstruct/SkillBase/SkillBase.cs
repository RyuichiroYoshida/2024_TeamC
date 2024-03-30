using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SoulRunProject.Common
{
    public enum PlayerSkill
    {
         SoulBullet = 0 ,
         HollyField = 1 ,
         SoulSword = 2 ,
         SoulShell = 3 ,
         SoulRay = 4 ,
         SoulOfHealing = 5 ,
         SoulFrame = 6 ,
    }
    
    /// <summary>
    /// スキルの基底クラス
    /// </summary>
    [Serializable , Name("基底クラス(名前をオーバーライドしてください)")]
    public class SkillBase 
    {
        [SerializeField , Header("スキルの最大レベル")] public int MaxSkillLevel = 5;
        [SerializeField, Header("レベルアップイベントデータ")] protected SkillLevelUpEvent SkillLevelUpEvent;
        [SerializeReference,SubclassSelector , Header("スキルのパラメーターデータ")] protected SkillParameterBase SkillBaseParam;

        /// <summary>
        /// シーンロード時にパラメータを初期化するように登録する。
        /// </summary>
        public virtual void InitializeParamOnSceneLoaded()
        {
            SkillBaseParam.InitializeParamOnSceneLoaded();
        }
        
        private int _currentLevel = 1;
        private float _currentCoolTime;
        
        public PlayerSkill SkillType => SkillBaseParam.SkillType;
        
        /// <summary> スキルレベルアップ可能かどうか </summary>
        public bool CanLevelUp()
        {
            return _currentLevel <= MaxSkillLevel;
        }

        public virtual void StartSkill(){}
        public virtual void UpdateSkill(float deltaTime)
        {
            if (_currentCoolTime < SkillBaseParam.CoolTime)
            {
                _currentCoolTime += deltaTime;
            }
            else
            {
                _currentCoolTime = 0;
                Fire();
            }
        }

        public virtual void Fire()
        {
            Debug.Log("発射");
        }
        
        /// <summary>スキル進化</summary>
        public void LevelUp()
        {
            _currentLevel++;
            if (CanLevelUp())
            {
                SkillLevelUpEvent.LevelUp(_currentLevel , SkillBaseParam);
            }
            else
            {
                Debug.LogError("レベル上限を超えています。");
            }
        }
    }
}