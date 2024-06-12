using System;
using SoulRunProject.Common;
using UnityEngine;

namespace SoulRunProject.InGame
{
    [RequireComponent(typeof(DamageableEntity))]
    public class EnemyController: MonoBehaviour, IPausable
    {
        [SerializeReference, SubclassSelector, CustomLabel("攻撃処理")]
        protected EntityAttacker _attacker;
        [SerializeReference, SubclassSelector, CustomLabel("移動処理")]
        protected EntityMover _mover;
        protected Transform _playerTransform;
        private Animator _animator;
        private DamageableEntity _damageableEntity;
        private bool _spawnFlag;
        private const float EnemyLifeTime = 20;
        private float _timer;
        

        private void Awake()
        {
            Register();
            _animator = GetComponent<Animator>();
            _damageableEntity = GetComponent<DamageableEntity>();
        }

        private void OnEnable()
        {
            _timer = 0;
            _spawnFlag = true;
        }
        
        private void OnDisable()
        {
            _spawnFlag = false;
        }

        private void OnDestroy()
        {
            UnRegister();
        }

        /// <summary>
        /// 各行動の初期化処理を行うメソッド
        /// </summary>
        void Start()
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player)
            {
                _playerTransform = player.transform;
            }
            Initialize();
        }

        public void Initialize()
        {
            _attacker?.OnStart();
            _mover?.OnStart(transform);
        }
        void Update()
        {
            if (!_spawnFlag)
            {
                return;
            }

            _timer += Time.deltaTime;
            _mover?.OnUpdateMove(transform, _playerTransform);
            _attacker?.OnUpdateAttack(transform, _playerTransform);

            if (EnemyLifeTime < _timer)
            {
                _damageableEntity.Death();
            }
            
            if (_playerTransform.position.z > gameObject.transform.position.z)
            {
                _damageableEntity.Death();
            }
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
                if(_animator) _animator.speed = 0;
                _attacker?.Pause();
                _mover?.Pause();
            }
            else
            {
                if(_animator) _animator.speed = 1;
                _attacker?.Resume();
                _mover?.Resume();
            }
        }
    }
}