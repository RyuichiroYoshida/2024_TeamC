using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using SoulRunProject.Audio;
using SoulRunProject.Common;
using SoulRunProject.InGame;
using SoulRunProject.Skill;
using UniRx;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace SoulRunProject
{
    public class SummoningProjectileSkill : AbstractSkill
    {
        private CancellationTokenSource _cts;
        private CommonObjectPool _swordPool;

        // 魔法陣生成位置の座標リスト
        private readonly List<Vector3> _magicCirclePos = new();

        // 魔法陣ゲームオブジェクトリスト
        private readonly List<GameObject> _magicCirclesRight = new();
        private readonly List<GameObject> _magicCirclesLeft = new();

        private float _timer;
        private float _delayTimer;
        private bool _swordShotFlag;
        private bool _magicCircleFlag;
        private int _lineCount = 1;

        private const int AddXPos = 5;
        private const int AddYPos = 5;

        public SummoningProjectileSkill(AbstractSkillData skillData, in PlayerManager playerManager,
            in Transform playerTransform)
            : base(skillData, in playerManager, in playerTransform)
        {
        }

        private SummoningProjectileSkillData SkillData =>
            (SummoningProjectileSkillData)_skillData;

        private ProjectileSkillParameter RuntimeParameter =>
            (ProjectileSkillParameter)_runtimeParameter;

        public override void StartSkill()
        {
            _timer = 0f;
            _delayTimer = 0f;

            _magicCirclePos.Clear();
            _magicCirclesRight.Clear();
            _magicCirclesLeft.Clear();

            _cts = new CancellationTokenSource();

            _swordPool = ObjectPoolManager.Instance.RequestPool(SkillData.Sword);

            // 魔法陣の初期生成

            // 最初の1列はここで追加
            _magicCirclePos.Add(new Vector3(AddXPos, AddYPos, -1));
            SetMagicCirclePos();
        }

        public override void UpdateSkill(float deltaTime)
        {
            var coolTime = RuntimeParameter.CoolTime;
            _timer += deltaTime;

            // 魔法陣起動タイマー
            if (_timer > coolTime)
            {
                // 念のためフラグで一回のみ回している
                if (_magicCircleFlag == false)
                {
                    foreach (var item in _magicCirclesRight)
                    {
                        item.SetActive(true);
                        item.GetComponent<ParticleSystem>().Play();
                    }

                    foreach (var item in _magicCirclesLeft)
                    {
                        item.SetActive(true);
                        item.GetComponent<ParticleSystem>().Play();
                    }

                    _magicCircleFlag = true;
                    CriAudioManager.Instance.Play(CriAudioType.CueSheet_SE, "SE_SoulSwordMagicSquare");
                }

                // 魔法陣起動から剣召喚までのタイマー
                _delayTimer += deltaTime;

                if (_delayTimer > SkillData.SwordShotDelay)
                {
                    _magicCircleFlag = false;
                    _timer = 0;
                    _delayTimer = 0;

                    // 念のためフラグで一回のみ回している
                    if (_swordShotFlag)
                    {
                        return;
                    }

                    _swordShotFlag = true;

                    // 魔法陣ゲームオブジェクトを格納しているリストの位置から剣を射出
                    foreach (var obj in _magicCirclesRight)
                    {
                        try
                        {
                            ShotSword(obj, obj.GetComponent<Transform>().position, _cts);
                        }
                        catch (Exception)
                        {
                            // ignored
                        }
                    }

                    foreach (var obj in _magicCirclesLeft)
                    {
                        try
                        {
                            ShotSword(obj, obj.GetComponent<Transform>().position, _cts);
                        }
                        catch (Exception)
                        {
                            // ignored
                        }
                    }

                    _swordShotFlag = false;
                }
            }
        }

        public override void OnLevelUp()
        {
            SetMagicCirclePos();

            // レベルアップ時にタイマーリセット
            _timer = 0;
            _delayTimer = 0;
        }

        private void SetMagicCirclePos()
        {
            // 2列目からの生成位置を求めるループ
            for (var i = _lineCount; i < RuntimeParameter.Amount; i++, _lineCount++)
            {
                var yPos = AddYPos + i;
                for (var j = 0; j <= i; j++)
                {
                    // 最初の箇所は基準になるので、処理を分ける
                    if (j <= 0)
                    {
                        _magicCirclePos.Add(new Vector3(i + AddXPos, yPos -= j, -1));
                        continue;
                    }

                    _magicCirclePos.Add(new Vector3(i + AddXPos, yPos -= 2, -1));
                }
            }

            CreateMagicCircle();
        }

        private void CreateMagicCircle()
        {
            // 魔法陣の生成処理
            for (var i = 0; i < _magicCirclePos.Count; i++)
            {
                var rightObj = Object.Instantiate(SkillData.MagicCirclePrefab,
                    _playerTransform.position + _magicCirclePos[i],
                    Quaternion.Euler(0, -15, 0));
                rightObj.transform.parent = _playerTransform;
                rightObj.SetActive(false);
                _magicCirclesRight.Add(rightObj);

                var leftObj = Object.Instantiate(SkillData.MagicCirclePrefab,
                    _playerTransform.position +
                    new Vector3(-_magicCirclePos[i].x, _magicCirclePos[i].y, _magicCirclePos[i].z),
                    Quaternion.Euler(0, 15, 0));
                leftObj.transform.parent = _playerTransform;
                leftObj.SetActive(false);
                _magicCirclesLeft.Add(leftObj);
            }

            _magicCirclePos.Clear();
        }

        /// <summary>
        /// 剣射出メソッド、生成から射出までの間に数秒待機する
        /// </summary>
        private async void ShotSword(GameObject obj, Vector3 pos, CancellationTokenSource cts)
        {
            var sword = (PlayerBullet)_swordPool.Rent();
            var oldParent = sword.transform.parent;
            sword.transform.parent = obj.transform;
            // 剣の初期位置は魔法陣の座標 + マズル位置
            sword.transform.position = pos + SkillData.MuzzleOffset;
            // 剣の初期角度は0
            sword.transform.rotation = Quaternion.Euler(0, 0, 0);

            var rand = Random.Range(0, 1f);
            await UniTask.Delay(TimeSpan.FromSeconds(rand), cancellationToken: cts.Token);

            sword.transform.parent = oldParent;
            obj.SetActive(false);

            // 待機後に剣を飛ばす
            sword.ApplyParameter(RuntimeParameter);
            sword.Initialize();
            sword.GetReference(_playerManagerInstance);
            sword.OnFinishedAsync.Take(1).Subscribe(_ => _swordPool.Return(sword));
        }
    }
}