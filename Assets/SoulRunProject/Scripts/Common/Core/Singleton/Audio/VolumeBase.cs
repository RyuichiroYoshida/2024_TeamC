using System.Globalization;
using UnityEngine;

namespace SoulRunProject.Audio
{
    public class VolumeBase : MonoBehaviour
    {
        private string _label;
        private float _volumeMinValue;
        private float _volumeMaxValue;
        private float _currentValue;


        public void Initialize(string label, float initialValue)
        {
            _label = label;
            _volumeMinValue = 0;
            _volumeMaxValue = 100;
            _currentValue = initialValue;
        }

        public void SetValue(float value)
        {
            _currentValue = value;
        }
    }
}