using System;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SoulRunProject.Audio
{
    public class VolumeControl : VolumeBase
    {
        [SerializeField] private TextMeshProUGUI _volumeText;
        [SerializeField] private Slider _volumeSlider;
        [SerializeField] private TMP_InputField _volumeInputField;

        public void Initialize(string label, float initialValue, UnityAction<float> onSliderChanged,
            UnityAction<string> onInputChanged)
        {
            base.Initialize(label, initialValue);

            _volumeSlider.minValue = 0;
            _volumeSlider.maxValue = 100;
            _volumeSlider.value = initialValue * 100;
            _volumeSlider.onValueChanged.AddListener(onSliderChanged);

            _volumeInputField.text = (initialValue * 100).ToString(CultureInfo.CurrentCulture);
            _volumeInputField.onEndEdit.AddListener(onInputChanged);
        }

        public new void SetValue(float value)
        {
            base.SetValue(value);
            _volumeSlider.value = value * 100;
            _volumeInputField.text = (value * 100).ToString(CultureInfo.CurrentCulture);
        }
    }
}