using System;
using SoulRunProject.Common;
using UnityEditor;
using UnityEngine;

namespace SoulRunProject.Skill
{
    /// <summary>
    /// 一定時間間隔で回復
    /// </summary>
    [CreateAssetMenu(menuName = "SoulRunProject/PlayerSkill/HealingSkill")]
    public class HealingSkill : SkillBase
    {
        [SerializeField, Tooltip("スキルエフェクト")] private ParticleSystem _healParticle;

        private HealingSkillParameter _healingSkillParam;
        private float _coolTimer;
        
        HealingSkill()
        {
            SkillParam = new HealingSkillParameter();
            SkillLevelUpEvent = new SkillLevelUpEvent(new HealingSkillLevelUpEventListList());
        }

        protected override void StartSkill()
        {
            if (SkillParam is HealingSkillParameter param)
                _healingSkillParam = param;
            ActivateSkill();
            Instantiate(_healParticle, PlayerTransform);
        }

        public override void UpdateSkill(float deltaTime)
        {
            if (_isPause) return;
            
            if (_coolTimer > 0)
            {
                _coolTimer -= deltaTime;
            }
            else if (PlayerManagerInstance.CurrentPlayerStatus.CurrentHp < PlayerManagerInstance.CurrentPlayerStatus.MaxHp) // HPがMaxだと発動しない
            {
                ActivateSkill();
            }
        }

        /// <summary> スキル発動 </summary>
        void ActivateSkill()
        {
            _coolTimer = _healingSkillParam.CoolTime;
            PlayerManagerInstance.Heal(_healingSkillParam.HealAmount);
            _healParticle.Play();
        }
        
        #if UNITY_EDITOR
        [CustomEditor(typeof(HealingSkill))]
        public class HealingSkillEditor : SkillBaseEditor
        {
            private HealingSkill _healingSkill;
            
            private void Awake()
            {
                _healingSkill = target as HealingSkill;
            }

            public override void OnInspectorGUI()
            {
                base.OnInspectorGUI();

                _healingSkill._healParticle = 
                    EditorGUILayout.ObjectField("エフェクト", _healingSkill._healParticle, typeof(ParticleSystem), true) as ParticleSystem;
            }
        }
        #endif
    }
}
