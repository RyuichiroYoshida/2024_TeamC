using UnityEngine;
using Dreamteck.Forever;

namespace SoulRunProject.Forever
{
    /// <summary>
    /// 横移動クラス
    /// </summary>
    public class RunnerMover : MonoBehaviour
    {
        Runner _runner;
        [SerializeField] float _moveSpeed = 5f;
        [SerializeField] float _clampWidth = 4f;

        void Start()
        {
            _runner = GetComponent<Runner>();
        }

        void Update()
        {
            _runner.motion.offset += new Vector2(Input.GetAxis("Horizontal"), 0f) * (_moveSpeed * Time.deltaTime);
            _runner.motion.offset = new Vector2(Mathf.Clamp(_runner.motion.offset.x, -_clampWidth, _clampWidth), _runner.motion.offset.y);
        }
    }
}
