using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

namespace SoulRunProject
{
    public class Billboard : MonoBehaviour
    {
        private CinemachineVirtualCamera _mainCamera;
        // Start is called before the first frame update
        void Start()
        {
            _mainCamera = FindObjectOfType<CinemachineVirtualCamera>();
        }

        // Update is called once per frame
        void Update()
        {
            // transform.LookAt(_mainCamera.transform.position, Vector3.up);
            // transform.rotation *= Quaternion.Euler(0, transform.rotation.y, 0);
            
            var direction = _mainCamera.transform.forward ;
            direction.y = 0;
 
            var lookRotation = Quaternion.LookRotation(direction, Vector3.up);
            transform.rotation = lookRotation;
        }
    }
}
