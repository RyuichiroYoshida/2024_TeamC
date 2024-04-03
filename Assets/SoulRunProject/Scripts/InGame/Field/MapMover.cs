using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace SoulRunProject
{
    public class MapMover : MonoBehaviour
    {
        [SerializeField] private float _moveSpeed = 1.0f;
        [SerializeField] private float _diff = 0.01f;
        private List<Transform> objs = new List<Transform>();
        [SerializeField] private float _originX = 0.5f;

        private void Start()
        {
            foreach (Transform child in transform)
            {
                objs.Add(child);
            }
        }

        public void HorizontalInput(Vector2 input)
        {
            if (input.x > 0)
            {
                foreach (var obj in objs)
                {
                    obj.transform.DOMoveX(0.5f, 0.5f);
                }
            }
            else if (input.x < 0)
            {
                foreach (var obj in objs)
                {
                    obj.transform.DOMoveX(0.5f, 0.5f);
                }
            }
            else
            {
                foreach (var obj in objs)
                {
                    obj.transform.DOMoveX(0.5f, 0.5f);
                }
            }
        }

        private void FixedUpdate()
        {
            transform.position += Vector3.back * (Time.fixedDeltaTime * _moveSpeed);
        }
    }
}
