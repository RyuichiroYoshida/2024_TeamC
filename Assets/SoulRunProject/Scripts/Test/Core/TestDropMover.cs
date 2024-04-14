using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulRunProject
{
    public class TestDropMover : MonoBehaviour
    {
        private float _speed = 20.0f;

        // Update is called once per frame
        void Update()
        {
            transform.position += _speed * Time.deltaTime * -Vector3.forward;
        }
    }
}
