using SoulRunProject.Common;
using SoulRunProject.InGame;
using UniRx;
using UnityEngine;

namespace SoulRunProject
{
    /// <summary>
    /// 飛翔物発射スキル
    /// <br/>表現を加える際にはさらにこれを継承する
    /// </summary>
    [CreateAssetMenu(menuName = "SoulRunProject/PlayerSkill/ProjectileSkill")]
    public class ProjectileSkill : SkillBase
    {
        static Transform _playerTransform;
        [SerializeField, Header("発射する弾のプレハブ")] PlayerBullet _bullet;

        [SerializeField, Header("複数弾を発射する際に与える回転基準")]
        float _baseRotateY = 5f;

        [SerializeField] Vector3 _muzzleOffset;
        CommonObjectPool _bulletPool;
        ProjectileSkillParameter _projectileSkillParameter;
        float _timer;

        ProjectileSkill()
        {
            SkillParam = new ProjectileSkillParameter();
            SkillLevelUpEvent = new SkillLevelUpEvent(new ProjectileSkillLevelUpEventListList());
        }

        protected override void InitializeParamOnSceneLoaded()
        {
            base.InitializeParamOnSceneLoaded();
            _timer = 0f;
        }

        protected override void StartSkill()
        {
            _bulletPool = ObjectPoolManager.Instance.RequestPool(_bullet);
            _projectileSkillParameter = (ProjectileSkillParameter)SkillParam;
        }

        public override void UpdateSkill(float deltaTime)
        {
            float currentCoolTime = _projectileSkillParameter.CoolTime;
            if (_timer < currentCoolTime)
            {
                _timer += deltaTime;
            }
            else
            {
                _timer = 0;
                if (_projectileSkillParameter != null)
                {
                    for (int i = 0; i < _projectileSkillParameter.Amount; i++)
                    {
                        // 弾の生成
                        float rotateY;
                        if (_projectileSkillParameter.Amount % 2 == 0)
                        {
                            rotateY = (i - _projectileSkillParameter.Amount / 2) * _baseRotateY + _baseRotateY / 2;
                        }
                        else
                        {
                            rotateY = (i - _projectileSkillParameter.Amount / 2) * _baseRotateY;
                        }


                        var bullet = (PlayerBullet)_bulletPool.Rent();
                        bullet.transform.position = PlayerTransform.position + _muzzleOffset;
                        bullet.transform.forward = PlayerTransform.forward;
                        bullet.transform.rotation *= Quaternion.Euler(0f, rotateY, 0f);
                        bullet.ApplyParameter(_projectileSkillParameter);
                        bullet.Initialize();
                        bullet.GetReference(PlayerManagerInstance);
                        bullet.OnFinishedAsync.Take(1).Subscribe(_ => _bulletPool.Return(bullet));
                    }

                    CriAudioManager.Instance.PlaySE("SE_Soulbullet");
                }
            }
        }
    }
}