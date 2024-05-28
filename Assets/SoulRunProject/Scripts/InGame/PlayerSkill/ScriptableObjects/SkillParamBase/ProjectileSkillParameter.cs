using System;
using System.Text;
using SoulRunProject.SoulMixScene;
using UnityEngine;

namespace SoulRunProject.Common
{
    /// <summary>
    /// スキルのパラメーター
    /// </summary>
    [Serializable , Name("発射スキルパラメータ")]
    public class ProjectileSkillParameter : ISkillParameter
    {
        [SerializeField, CustomLabel("次にこのスキルを使えるまでの時間")] float _coolTime;
        [SerializeField, CustomLabel("同時発射するオブジェクトの数")] int _amount;
        [SerializeField, CustomLabel("敵にヒットしたときに与えるダメージ")] float _attackDamage;
        [SerializeField, CustomLabel("スキルのオブジェクトの大きさ")] float _size;
        [SerializeField, CustomLabel("スキルオブジェクトの移動速度")] float _speed;
        [SerializeField, CustomLabel("敵を何体まで貫通するか")] int _penetration;
        [SerializeField, CustomLabel("弾のライフタイム")] float _lifeTime;
        [SerializeField, CustomLabel("与ノックバック")]　GiveKnockBack _knockBack;
        [SerializeField, CustomLabel("ヒット時のソウル増加量")] private float _getSoulValueOnHit;
        
        /// <summary>
        /// ランタイム時に変更される基礎値
        /// </summary>
        [NonSerialized] public float BaseCoolTime;
        [NonSerialized] public float BaseLifeTime;
        [NonSerialized] public int BaseAmount;
        [NonSerialized] public float BaseAttackDamage;
        [NonSerialized] public GiveKnockBack BaseKnockBack;
        [NonSerialized] public float BaseSize;
        [NonSerialized] public float BaseSpeed;
        [NonSerialized] public int BasePenetration;
        [NonSerialized] public float GetSoulValueOnHit;
        
        private PlayerStatus _status;
        public void SetPlayerStatus(in PlayerStatus status)
        {
            _status = status;
        }

        public float AttackDamage => BaseAttackDamage + _status.AttackValue;
        public float CoolTime => BaseCoolTime * _status.CoolTimeReductionRate;
        public float Size => BaseSize * _status.SkillSizeUpRate;
        public int Amount => BaseAmount + _status.BulletAmountExtension;
        public float Speed => BaseSpeed * _status.BulletSpeedUpRate;
        public int Penetration => BasePenetration + _status.PenetrateAmountExtension;
        public float LifeTime => BaseLifeTime;
        public GiveKnockBack KnockBack => BaseKnockBack;
        

        
        public void InitializeParamOnSceneLoaded()
        {
            BaseCoolTime = _coolTime;
            BaseLifeTime = _lifeTime;
            BaseAmount = _amount;
            BaseAttackDamage = _attackDamage;
            BaseKnockBack = _knockBack;
            BaseSize = _size;
            BaseSpeed = _speed;
            BasePenetration = _penetration;
            GetSoulValueOnHit = _getSoulValueOnHit;
        }
        

    }
}
