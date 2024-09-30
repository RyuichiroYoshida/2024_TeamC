using SoulRunProject.InGame;
using UniRx;
using UnityEngine;

namespace SoulRunProject
{
    public class CameraRotate : MonoBehaviour
    {
        [SerializeField] private Transform _cameraLookAt;
        [SerializeField, Header("回転制限")] private float _crampRotateZ;
        [SerializeField , Header("上昇率")] private float _improveRate;
        [SerializeField , Header("減衰率")] private float _decreaseRate;
        private float _currentRotateZ;
        private Vector2 _inputVector;
        private void Awake()
        {
            PlayerInputManager.Instance.MoveInput.Subscribe(input => _inputVector = input).AddTo(this);
        }

        private void FixedUpdate()
        {
            if (_inputVector.x > 0f)
            {
                _currentRotateZ = EaseIn(_currentRotateZ,_crampRotateZ, _improveRate * Time.fixedDeltaTime);
            }
            else if (_inputVector.x < 0f)
            {
                _currentRotateZ = EaseIn(_currentRotateZ, -_crampRotateZ, _improveRate * Time.fixedDeltaTime);
            }
            else
            {
                //減衰補完
                _currentRotateZ = EaseIn(_currentRotateZ, 0f, _decreaseRate * Time.fixedDeltaTime);
            }

            _cameraLookAt.transform.rotation  = Quaternion.AngleAxis(_currentRotateZ, Vector3.forward);
            
        }

        private float EaseIn(float start, float end, float t)
        {
            t = Mathf.Clamp01(t);  // t を 0 と 1 の間に制限
            t = t * t;  // 非線形補完のための関数
            return Mathf.Lerp(start, end, t);
        }
        
    }
}
