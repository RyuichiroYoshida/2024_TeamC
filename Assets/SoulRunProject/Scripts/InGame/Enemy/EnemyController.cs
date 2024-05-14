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

        private void Awake()
        {
            Register();
            _animator = GetComponent<Animator>();
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
            _playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
            Initialize();
        }

        public void Initialize()
        {
            _attacker?.OnStart();
            _mover?.OnStart(transform);
        }
        void Update()
        {
            _mover?.OnUpdateMove(transform, _playerTransform);
            _attacker?.OnUpdateAttack(transform, _playerTransform);
        }

        public void Register()
        {
            PauseManager.Instance.RegisterPausableObject(this);
        }

        public void UnRegister()
        {
            PauseManager.Instance.UnRegisterPausableObject(this);
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