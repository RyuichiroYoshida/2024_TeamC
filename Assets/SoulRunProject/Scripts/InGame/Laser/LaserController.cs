using UnityEngine;

namespace SoulRunProject.InGame
{
    public class LaserController : MonoBehaviour
    {
        private int _turnCount = 2;
        public Vector3 StartDirection { get; set; }
        public Vector3 EndDirection { get; set; }
        public float Timer { get; set; }
        public bool TurnSide { get; set; }
        public int TurnCount
        {
            get => _turnCount;
            set => _turnCount = value;
        }
    }
}