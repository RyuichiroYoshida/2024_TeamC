using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using SoulRunProject.Common;
using SoulRunProject.InGame;
using UniRx;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SoulRunProject
{
    [CreateAssetMenu(menuName = "SoulRunProject/PlayerSkill/MagicSquareSkill")]
    public class MagicSquareSkill : SkillBase
    {
        private static Transform _playerTransform;

        [SerializeField, CustomLabel("魔法陣の列数")] private int _count;


        [SerializeField, CustomLabel("発射する剣のプレハブ")]
        private PlayerBullet _sword;

        [SerializeField, CustomLabel("剣を召喚する魔法陣のプレハブ")]
        private GameObject _magicSquarePrefab;

        [SerializeField] private Vector3 _muzzleOffset;

        [SerializeField, CustomLabel("魔法陣起動から射出までの遅延")] private float _swordShotDelay;

        private CancellationTokenSource _cts;
        private CommonObjectPool _swordPool;
        private ProjectileSkillParameter _projectileSkillParameter;

        // 魔法陣生成位置の座標リスト
        private List<Vector3> _magicSquarePos = new();

        // 魔法陣ゲームオブジェクトリスト
        private List<GameObject> _magicSquaresRight = new();
        private List<GameObject> _magicSquaresLeft = new();

        private float _timer;
        private float _delayTimer;
        private bool _swordShotFlag;
        private bool _magicSquareFlag;
        
        private MagicSquareSkill()
        {
            SkillParam = new ProjectileSkillParameter();
            SkillLevelUpEvent = new SkillLevelUpEvent(new ProjectileSkillLevelUpEventListList());
        }

        protected override void InitializeParamOnSceneLoaded()
        {
            base.InitializeParamOnSceneLoaded();
            _timer = 0f;
            _delayTimer = 0f;

            _magicSquarePos.Clear();
            _magicSquaresRight.Clear();
            _magicSquaresLeft.Clear();

            _cts = new CancellationTokenSource();
        }

        protected override void StartSkill()
        {
            _swordPool = ObjectPoolManager.Instance.RequestPool(_sword);
            _projectileSkillParameter = (ProjectileSkillParameter)SkillParam;

            CreateMagicSquare();
        }

        public override void UpdateSkill(float deltaTime)
        {
            var coolTime = _projectileSkillParameter.CoolTime;
            _timer += deltaTime;

            // 魔法陣起動タイマー
            if (_timer > coolTime)
            {
                // 念のためフラグで一回のみ回している
                if (_magicSquareFlag == false)
                {
                    foreach (var item in _magicSquaresRight)
                    {
                        item.SetActive(true);
                        item.GetComponent<ParticleSystem>().Play();
                    }

                    foreach (var item in _magicSquaresLeft)
                    {
                        item.SetActive(true);
                        item.GetComponent<ParticleSystem>().Play();
                    }

                    _magicSquareFlag = true;
                }

                // 魔法陣起動から剣召喚までのタイマー
                _delayTimer += deltaTime;

                if (_delayTimer > _swordShotDelay)
                {
                    _magicSquareFlag = false;
                    _timer = 0;
                    _delayTimer = 0;

                    // 念のためフラグで一回のみ回している
                    if (_swordShotFlag)
                    {
                        return;
                    }

                    _swordShotFlag = true;

                    // 魔法陣ゲームオブジェクトを格納しているリストの位置から剣を射出
                    foreach (var obj in _magicSquaresRight)
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

                    foreach (var obj in _magicSquaresLeft)
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

        private void CreateMagicSquare()
        {
            // 最初の1列はここで追加
            _magicSquarePos.Add(new Vector3(5, 5, -1));

            // 2列目からの生成位置を求めるループ
            for (var i = 1f; i < _count; i++)
            {
                var yPos = 5 + i;
                for (var j = 0f; j <= i; j++)
                {
                    // 最初の箇所は基準になるので、処理を分ける
                    if (j <= 0)
                    {
                        _magicSquarePos.Add(new Vector3(i + 5, yPos -= j, -(1 + i / 5)));
                        continue;
                    }

                    _magicSquarePos.Add(new Vector3(i + 5, yPos -= 2, -(1 + i / 5)));
                }
            }

            // 魔法陣の生成処理
            for (var i = 0; i < _magicSquarePos.Count; i++)
            {
                // 魔法陣を左右それぞれリストに格納する
                _magicSquaresRight.Add(Instantiate(_magicSquarePrefab, _magicSquarePos[i],
                    Quaternion.Euler(0, -15, 0)));
                _magicSquaresRight[i].transform.parent = PlayerTransform;
                _magicSquaresRight[i].SetActive(false);

                _magicSquaresLeft.Add(Instantiate(_magicSquarePrefab,
                    new Vector3(-_magicSquarePos[i].x, _magicSquarePos[i].y, _magicSquarePos[i].z),
                    Quaternion.Euler(0, 15, 0)));
                _magicSquaresLeft[i].transform.parent = PlayerTransform;
                _magicSquaresLeft[i].SetActive(false);
            }
        }

        /// <summary>
        /// 剣射出メソッド、生成から射出までの間に数秒待機する
        /// </summary>
        private async void ShotSword(GameObject obj, Vector3 pos, CancellationTokenSource cts)
        {
            var sword = (PlayerBullet)_swordPool.Rent();
            // 剣の初期位置は魔法陣の座標 + マズル位置
            sword.transform.position = pos + _muzzleOffset;
            // 剣の初期角度は0
            sword.transform.rotation = Quaternion.Euler(0, 0, 0);

            var rand = Random.Range(0, 1f);
            await UniTask.Delay(TimeSpan.FromSeconds(rand), cancellationToken: cts.Token);

            obj.SetActive(false);

            // 待機後にランダムな角度で剣を飛ばす
            var randomRotationX = Random.Range(0f, 30);
            var randomRotationY = Random.Range(0f, 40);
            sword.transform.rotation = Quaternion.Euler(randomRotationX, -randomRotationY, 0);
            sword.ApplyParameter(_projectileSkillParameter);
            sword.Initialize();
            sword.OnFinishedAsync.Take(1).Subscribe(_ => _swordPool.Return(sword));
        }
    }
}