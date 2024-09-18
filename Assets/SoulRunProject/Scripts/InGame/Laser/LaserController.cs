using UnityEngine;

namespace SoulRunProject.InGame
{
    public class LaserController : MonoBehaviour
    {
        [SerializeField] private GameObject _hitEffect;
        
        private int _turnCount = 2;
        private ParticleSystem[] _particleSystems;
        public Vector3 StartDirection { get; set; }
        public Vector3 EndDirection { get; set; }
        public float Timer { get; set; }
        public bool TurnSide { get; set; }
        public int TurnCount
        {
            get => _turnCount;
            set => _turnCount = value;
        }
        public GameObject HitEffect => _hitEffect;
        public ParticleSystem[] ParticleSystems => _particleSystems;

        private void Awake()
        {
            _particleSystems = GetComponentsInChildren<ParticleSystem>();
        }
    }
}