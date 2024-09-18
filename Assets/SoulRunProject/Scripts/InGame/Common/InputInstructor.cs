using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.InputSystem.Switch;
using UnityEngine.InputSystem.XInput;

namespace SoulRunProject
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class InputInstructor : MonoBehaviour
    {
        [SerializeField] private List<InputInstructionData> _inputInstructions;
        [SerializeField] private List<int> _ignoreIndex = new();
        private TextMeshProUGUI _textMeshPro;
        private static readonly string[] LeftStickDirections = { "leftStick/up", "leftStick/down", "leftStick/right", "leftStick/left" };
        private static readonly string[] RightStickDirections = { "rightStick/up", "rightStick/down", "rightStick/right", "rightStick/left" };
        private static readonly string[] ArrowDirections = { "upArrow", "downArrow", "rightArrow", "leftArrow" };

        private void Awake()
        {
            _textMeshPro = GetComponent<TextMeshProUGUI>();
            DeviceUpdate(null, default);
            InputSystem.onDeviceChange += DeviceUpdate;
        }

        private void OnDestroy()
        {
            InputSystem.onDeviceChange -= DeviceUpdate;
        }

        void DeviceUpdate(InputDevice device, InputDeviceChange change)
        {
            if (gameObject == null) return;
            var sb = new StringBuilder();
            foreach (var data in _inputInstructions)
            {
                sb.Append(data.AboveText);
                foreach (var kv in GetControlPath(data.ActionReference.action))
                {
                    foreach (var controlPath in kv.Value)
                    {
                        var iconName = $"{kv.Key}-{controlPath.Replace("/", "")}";
                        var spriteIndex = _textMeshPro.spriteAsset.GetSpriteIndexFromName(iconName);
                        if (_ignoreIndex.Contains(spriteIndex)) continue;
                        if (spriteIndex >= 0)
                        {
                            sb.Append("<sprite=").Append(spriteIndex).Append(">");
                        }
                    }
                }
                sb.Append(data.BelowText);
            }

            _textMeshPro.text = sb.ToString();
        }

        Dictionary<string, HashSet<string>> GetControlPath(InputAction action)
        {
            var foundControlPathDic = new Dictionary<string, HashSet<string>>();
            foreach (var binding in action.bindings)
            {
                var path = binding.effectivePath;
                var matchedControls = action.controls.Where(control => InputControlPath.Matches(path, control));
                foreach (var control in matchedControls)
                {
                    if(control is InputDevice) continue;
                    var deviceIconGroup = GetDeviceIconGroup(control.device);
                    if (string.IsNullOrEmpty(deviceIconGroup)) continue;
                    var controlPathContent = control.path.Substring(control.device.name.Length + 2);
                    
                    if (!foundControlPathDic.ContainsKey(deviceIconGroup))
                    {
                        foundControlPathDic[deviceIconGroup] = new();
                    }
                    foundControlPathDic[deviceIconGroup].Add(controlPathContent);
                }
            }

            foreach (var kv in foundControlPathDic)
            {
                if (kv.Value.RemoveWhere(path=>LeftStickDirections.Contains(path)) > 0)
                {
                    kv.Value.Add("leftStick");
                }
                if (kv.Value.RemoveWhere(path=>RightStickDirections.Contains(path)) > 0)
                {
                    kv.Value.Add("rightStick");
                }
                if (kv.Value.RemoveWhere(path=>ArrowDirections.Contains(path)) > 0)
                {
                    kv.Value.Add("arrow");
                }
            }

            return foundControlPathDic;
        }
        string GetDeviceIconGroup(InputDevice device)
        {
            return device switch
            {
                Keyboard => "Keyboard",
                Mouse => "Mouse",
                XInputController => "XInputController",
                DualShockGamepad => "DualShockGamepad",
                SwitchProControllerHID => "SwitchProController",
                _ => null
            };
        }
    }
    [Serializable]
    public class InputInstructionData
    {
        [SerializeField, TextArea] private string _aboveText;
        [SerializeField, TextArea] private string _belowText;
        [SerializeField] private InputActionReference _actionReference;
        public string AboveText => _aboveText;
        public string BelowText => _belowText;
        public InputActionReference ActionReference => _actionReference;
    }
}