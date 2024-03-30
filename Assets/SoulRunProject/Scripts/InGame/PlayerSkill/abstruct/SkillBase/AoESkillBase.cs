using UnityEngine;

namespace SoulRunProject.Common
{
    /// <summary>
    /// 範囲攻撃スキル実行クラス
    /// </summary>
    public class AoESkillBase : SkillBase
    {
        [SerializeField, Tooltip("InstantiateするAoEプレハブ")] AoEController _original;
        [SerializeField, Tooltip("地面の高さ、y座標")] float _groundHeight;
        static Transform _playerTransform;
        AoEController _aoeController;
        public override void StartSkill()
        {
            _playerTransform = Object.FindObjectOfType<PlayerManager>().transform;
            _aoeController = Object.Instantiate(_original, _playerTransform);
            FieldProjection();
            if (SkillBaseParam is AoESkillParameter param)
            {
                _aoeController.Initialize(param.AttackDamage, param.Range);
            }
            else
            {
                Debug.LogError($"パラメータが{nameof(AoESkillParameter)}ではありません　");
            }
        }
        public override void Fire()
        {
            //  クールタイム0の発火メソッドで座標更新を行っている(Update)
            FieldProjection();
        }
        /// <summary>
        /// フィールドを地面に投影する
        /// </summary>
        void FieldProjection()
        {
            var playerPosition = _playerTransform.position;
            playerPosition.y = _groundHeight;
            _aoeController.transform.position = playerPosition;
        }
    }
}