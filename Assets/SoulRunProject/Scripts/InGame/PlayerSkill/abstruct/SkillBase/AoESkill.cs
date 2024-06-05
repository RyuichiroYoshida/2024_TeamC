using UnityEngine;

namespace SoulRunProject.Common
{
    /// <summary>
    /// 範囲攻撃スキル
    /// </summary>
    [CreateAssetMenu(menuName = "SoulRunProject/PlayerSkill/AoESkill")]
    public class AoESkill : SkillBase
    {
        [SerializeField, CustomLabel("生成するAoEプレハブ")] AoEController _original;
        [SerializeField, CustomLabel("展開する地面の高さ、y座標")] float _groundHeight;
        static Transform _playerTransform;
        AoEController _aoeController;

        AoESkill()
        {
            SkillParam = new AoESkillParameter();
            SkillLevelUpEvent = new SkillLevelUpEvent(new AoESkillLevelUpEventListList());
        }
        /// <summary>
        /// パラメーターを適用する
        /// </summary>
        void ApplyParameter()
        {
            if (SkillParam is AoESkillParameter param)
            {
                _aoeController.Initialize(param, PlayerManagerInstance);
            }
            else
            {
                Debug.LogError($"パラメータが{nameof(AoESkillParameter)}ではありません　");
            }
        }

        protected override void StartSkill()
        {
            _aoeController = Object.Instantiate(_original);
            ApplyParameter();
        }

        public override void UpdateSkill(float deltaTime)
        {
            //  AoEの座標更新
            var playerPosition = PlayerTransform.position;
            playerPosition.y = _groundHeight;
            _aoeController.transform.position = playerPosition;
        }

        public override void OnLevelUp()
        {
            ApplyParameter();
        }
    }
}