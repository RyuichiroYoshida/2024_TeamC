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

        private void Update()
        {
            _currentRotateZ += -_inputVector.x * _improveRate * Time.deltaTime;
            //回転制限
            _currentRotateZ = Mathf.Clamp(_currentRotateZ, -_crampRotateZ, _crampRotateZ);
            //減衰補完
            _currentRotateZ = Mathf.Lerp(_currentRotateZ, 0f, _decreaseRate * Time.deltaTime);

            var myRotation = _cameraLookAt.transform.rotation;
            _cameraLookAt.transform.rotation  = Quaternion.AngleAxis(_currentRotateZ, Vector3.forward);
            
        }
    }
}
