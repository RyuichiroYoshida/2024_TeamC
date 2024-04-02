using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulRunProject
{
    public class CameraRotate : MonoBehaviour
    {
        [SerializeField] private Transform _cameraLookAt;
        [SerializeField, Header("回転制限")] private float _crampRotateY;
        [SerializeField , Header("上昇率")] private float _improveRate;
        [SerializeField , Header("減衰率")] private float _decreaseRate;

        private float _currentRotateY;
        private void Update()
        {
            float input = Input.GetAxis("Horizontal");
            Debug.Log(input);
            _currentRotateY += input * _improveRate * Time.deltaTime;
            //回転制限
            _currentRotateY = Mathf.Clamp(_currentRotateY, -_crampRotateY, _crampRotateY);
            //減衰補完
            _currentRotateY = Mathf.Lerp(_currentRotateY, 0f, _decreaseRate * Time.deltaTime);

            var myRotation = this.transform.rotation;
            this.transform.rotation = Quaternion.Euler(myRotation.x , _currentRotateY , myRotation.z);
            
        }
    }
}
