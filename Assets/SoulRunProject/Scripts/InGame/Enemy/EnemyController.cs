using Cysharp.Threading.Tasks;
using SoulRunProject.Audio;
using SoulRunProject.Common;
using UnityEngine;

namespace SoulRunProject.InGame
{
    [RequireComponent(typeof(DamageableEntity))]
    public class EnemyController : MonoBehaviour, IPausable
    {
        [SerializeReference] [SubclassSelector] [CustomLabel("攻撃処理")]
        protected EntityAttacker _attacker;
        [SerializeReference] [SubclassSelector] [CustomLabel("スタート時移動処理")]
        protected EntityMover _startMover;
        [SerializeReference] [SubclassSelector] [CustomLabel("移動処理")]
        protected EntityMover _mover;

        [SerializeField] [CustomLabel("Enemyの寿命")]
        private float _enemyLifeTime = 10;

        protected Transform _playerTransform;
        protected PlayerManager _playerManagerInstance;
        private Animator _animator;
        private DamageableEntity _damageableEntity;
        private bool _spawnFlag;
        private float _timer;

        private void Awake()
        {
            Register();
            _animator = GetComponent<Animator>();
            _damageableEntity = GetComponent<DamageableEntity>();
            _playerManagerInstance = FindObjectOfType<PlayerManager>();
            if (_playerManagerInstance) _playerTransform = _playerManagerInstance.transform;
            if(_attacker != null) _attacker.Animator = _animator;
            if (_mover is not null) _mover.Complete += _damageableEntity.Despawn;
        }

        public async UniTaskVoid Initialize()
        {
            _damageableEntity.OnDead += () => CriAudioManager.Instance.Play(CriAudioType.CueSheet_SE, "SE_Enemy_Dead");
            _timer = 0;
            var completionSource = new UniTaskCompletionSource();

            if (_startMover != null)
            {
                _startMover.OnStart(transform, _playerManagerInstance);
                _startMover.Complete += () => completionSource.TrySetResult();
                await completionSource.Task;
            }
            _attacker?.OnStart(transform);
            _mover?.OnStart(transform, _playerManagerInstance);
            _spawnFlag = true;
        }

        private void OnDisable()
        {
            _spawnFlag = false;
        }

        private void OnDestroy()
        {
            if (_mover is not null) _mover.Complete -= _damageableEntity.Despawn;
            UnRegister();
        }
        private void Update()
        {
            _startMover?.OnUpdateMove(transform, _playerTransform);
            if (!_spawnFlag) return;

            _timer += Time.deltaTime;
            _attacker?.OnUpdateAttack(transform, _playerTransform);
            if (_attacker is EntityLongRangeAttacker { CanMove: true } or not EntityLongRangeAttacker)
            {
                _mover?.OnUpdateMove(transform, _playerTransform);
            }

            if (_enemyLifeTime < _timer) _damageableEntity.Despawn();
            if (_playerTransform.position.z - 5 > gameObject.transform.position.z) _damageableEntity.Despawn();
        }

        public void Register()
        {
            PauseManager.RegisterPausableObject(this);
        }

        public void UnRegister()
        {
            PauseManager.UnRegisterPausableObject(this);
        }

        public void Pause(bool isPause)
        {
            if (isPause)
            {
                if (_animator) _animator.speed = 0;
                _attacker?.Pause();
            }
            else
            {
                if (_animator) _animator.speed = 1;
                _attacker?.Resume();
            }
        }
    }
}