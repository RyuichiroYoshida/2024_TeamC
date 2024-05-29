using UnityEngine;

namespace SoulRunProject.InGame
{
    public class LaserData
    {
        public Vector3 StartDirection;
        public Vector3 EndDirection;
        public float Timer;
        public int TurnCount = 2;
        public bool TurnSide = false;
        
        public Hovl_Laser HovlLaser { get; }

        public LaserData(Hovl_Laser laser)
        {
            HovlLaser = laser;
        }
        public LaserData(Hovl_Laser laser, Vector3 startDir, Vector3 endDir)
        {
            HovlLaser = laser;
            StartDirection = startDir;
            EndDirection = endDir;
        }
    }
}