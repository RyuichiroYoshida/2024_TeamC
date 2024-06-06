using Cysharp.Threading.Tasks;
using SoulRunProject.Common;
using UniRx;
using UnityEngine;

namespace SoulRunProject.InGame
{
    public class LaserController : MonoBehaviour
    {
        [SerializeField] private int _initialCount = 5;
        [SerializeField] private float _offsetY = 10f;
        [SerializeField] private float _radius = 5f;
        [SerializeField, CustomLabel("生成するプレハブ")] private Hovl_Laser _original;
        [SerializeField] private float _width = 10f;
        [SerializeField] private float _height = 2f;
        [SerializeField] private float _speed = 1f;
        [SerializeField] private float _delay = 0.1f;
        [SerializeField] private int _turnCount = 15;
        [SerializeField] private float _coolTime = 3f;
        private readonly ReactiveCollection<LaserData> _laserList = new();
        private readonly BoolReactiveProperty _endFlag = new(false);
        private float _timer;
        private void Awake()
        {
            _laserList.ObserveCountChanged().Subscribe(Arrangement).AddTo(this);
            for (int i = 0; i < _initialCount; i++)
            {
                _laserList.Add(new LaserData(Instantiate(_original, transform)));
            }
            Initialize();
            _endFlag.Subscribe(flag =>
            {
                if (flag)
                {
                    CoolDown().Forget();
                }
            }).AddTo(this);
        }

        async UniTaskVoid CoolDown()
        {
            await UniTask.WaitForSeconds(_coolTime, cancellationToken: destroyCancellationToken);
            Initialize();
        }

        private void Initialize()
        {
            for (int i = 0; i < _laserList.Count; i++)
            {
                var startPos = transform.position;
                startPos.x -= _width / 2;
                startPos.z += _height;
                var targetPos = transform.position;
                targetPos.x += _width / 2;
                targetPos.z += _height * 2;
                var laserPos = transform.position + _laserList[i].HovlLaser.transform.localPosition;
                _laserList[i].StartDirection = startPos - laserPos;
                _laserList[i].EndDirection = targetPos - laserPos;
                _laserList[i].Timer = -_delay * i;
                _laserList[i].TurnCount = 2;
                _laserList[i].TurnSide = false;
                _laserList[i].HovlLaser.gameObject.SetActive(true);
            }

            _endFlag.Value = false;
        }

        private void Update()
        {
            int endCount = 0;
            foreach (var laser in _laserList)
            {
                if (laser.Timer > 1 && _turnCount > laser.TurnCount)
                {
                    laser.Timer = 0;
                    laser.TurnCount++;
                    laser.StartDirection = laser.EndDirection;
                    Vector3 endPos = transform.position;
                    if (laser.TurnSide)
                    {
                        endPos.x += _width / 2;
                    }
                    else
                    {
                        endPos.x -= _width / 2;
                    }
                    endPos.z += _height * laser.TurnCount;
                    var laserPos = transform.position + laser.HovlLaser.transform.localPosition;
                    laser.EndDirection = endPos - laserPos;
                    laser.TurnSide = !laser.TurnSide;
                }
                else if (_turnCount <= laser.TurnCount)
                {
                    laser.HovlLaser.gameObject.SetActive(false);
                    endCount++;
                    continue;
                }

                var matrix2 = Matrix4x4.TRS(Vector3.zero, transform.rotation, Vector3.one);
                //  プレイヤーが途中で回転してもいいようにベクトルに回転行列を掛ける。
                laser.HovlLaser.transform.rotation = 
                    Quaternion.LookRotation(Vector3.Lerp(matrix2.MultiplyVector(laser.StartDirection), matrix2.MultiplyVector(laser.EndDirection), Mathf.Clamp01(laser.Timer)));
                laser.Timer += Time.deltaTime * _speed;
                
                // if (Physics.Raycast(laser.HovlLaser.transform.position, laser.HovlLaser.transform.forward, out RaycastHit hit))
                // {
                //     if (hit.collider.TryGetComponent(out DamageableEntity entity))
                //     {
                //         entity.Damage(20 * Time.deltaTime);
                //     }
                // }
            }

            if (endCount >= _laserList.Count)
            {
                _endFlag.Value = true;
            }
        }
        [ContextMenu("Restart")]
        void Restart()
        {
            Initialize();
        }

        [ContextMenu("AddLaser")]
        void AddLaser()
        {
            var data = new LaserData(Instantiate(_original, transform));
            var startPos = transform.position;
            startPos.x -= _width / 2;
            startPos.z += _height;
            var targetPos = transform.position;
            targetPos.x += _width / 2;
            targetPos.z += _height * 2;
            data.StartDirection = startPos - data.HovlLaser.transform.position;
            data.EndDirection = targetPos - data.HovlLaser.transform.position;
            _laserList.Add(data);
        }
        
        void Arrangement(int count)
        {
            float angleDiff = 180f / count;
            for (int i = 0; i < count; i++)
            {
                Vector3 pos = transform.position;
                float angle = (180 + angleDiff * i) * Mathf.Deg2Rad;
                pos.x += _radius * Mathf.Cos(angle);
                pos.y = transform.position.y + _offsetY;
                pos.z += _radius * Mathf.Sin(angle);
                var matrix = Matrix4x4.TRS(Vector3.zero, transform.rotation, Vector3.one);
                pos = matrix.MultiplyPoint3x4(pos);
                //  整列前の座標を保存しておく
                var prevPos = _laserList[i].HovlLaser.transform.position;
                _laserList[i].HovlLaser.transform.position = pos;
                //  整列前と後の差を出す
                var moveDiff = prevPos - pos;
                //  レーザーのベクトルに差を足す
                _laserList[i].StartDirection += moveDiff;
                _laserList[i].EndDirection += moveDiff;
            }
        }
    }
}