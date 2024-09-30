using System;
using System.Collections.Generic;
using SoulRunProject.Common;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

namespace SoulRunProject.InGame
{
    /// <summary>
    /// プレイヤーのインプットを管理する
    /// </summary>
    public class PlayerInputManager : AbstractSingletonMonoBehaviour<PlayerInputManager>
    {
        [SerializeField] private PlayerInput _playerInput;
        private readonly Dictionary<(ActionMapType actionMapType, ActionType actionType), InputCallbackData> _callbackDictionary = new();
        private readonly Dictionary<ActionMapType, InputActionMap> _actionMapDictionary = new();
        private readonly ReactiveProperty<Vector2> _moveInput = new();
        private readonly Subject<Unit> _jumpInput = new();
        private readonly Subject<Unit> _pauseInput = new();
        private readonly Subject<Unit> _soulSkillInput = new();
        public ReactiveProperty<Vector2> MoveInput => _moveInput;
        public Subject<Unit> JumpInput => _jumpInput;
        public Subject<Unit> PauseInput => _pauseInput;
        public Subject<Unit> SoulSkillInput => _soulSkillInput;
        protected override bool UseDontDestroyOnLoad { get; } = true;
        public override void OnAwake()
        {
            _moveInput.AddTo(this);
            _jumpInput.AddTo(this);
            _pauseInput.AddTo(this);
            _soulSkillInput.AddTo(this);
            if (_playerInput && _playerInput.actions)
            {
                _actionMapDictionary.Add(ActionMapType.UI, _playerInput.actions.FindActionMap(ActionMapType.UI.ToString()));
                _actionMapDictionary.Add(ActionMapType.Player, _playerInput.actions.FindActionMap(ActionMapType.Player.ToString()));
            }
            _callbackDictionary.Add((ActionMapType.Player, ActionType.Jump), new InputCallbackData(OnJump));
            _callbackDictionary.Add((ActionMapType.Player, ActionType.Move), new InputCallbackData(OnMove));
            _callbackDictionary.Add((ActionMapType.Player, ActionType.UseSoulSkill), new InputCallbackData(OnUseSoulSkill));
            _callbackDictionary.Add((ActionMapType.Player, ActionType.Menu), new InputCallbackData(OnPause));
            _callbackDictionary.Add((ActionMapType.UI, ActionType.Menu), new InputCallbackData(OnPause));
            BindAction(ActionMapType.Player, ActionType.Jump, true);
            BindAction(ActionMapType.Player, ActionType.Move, true);
            BindAction(ActionMapType.Player, ActionType.UseSoulSkill, true);
            BindAction(ActionMapType.Player, ActionType.Menu, true);
            BindAction(ActionMapType.UI, ActionType.Menu, true);
            
            //  ユーザーのidに更新をかけることでInput.validプロパティでfalseが出力されるのを回避する。
            InputUser.CreateUserWithoutPairedDevices();
            InputSystem.onDeviceChange += DeviceUpdate;
            DeviceUpdate(null, default);
        }

        public override void OnDestroyed()
        {
            InputSystem.onDeviceChange -= DeviceUpdate;
            BindAction(ActionMapType.Player, ActionType.Jump, false);
            BindAction(ActionMapType.Player, ActionType.Move, false);
            BindAction(ActionMapType.Player, ActionType.UseSoulSkill, false);
            BindAction(ActionMapType.Player, ActionType.Menu, false);
            BindAction(ActionMapType.UI, ActionType.Menu, false);
        }

        public void BindAction(ActionMapType actionMapType, ActionType actionType, bool isActive)
        {
            var path = (actionMapType, actionType);
            if (!_actionMapDictionary.ContainsKey(actionMapType) || !_callbackDictionary.ContainsKey(path)) return;
            var actionMap = _actionMapDictionary[actionMapType];
            var actionName = actionType.ToString();
            switch (isActive)
            {
                case true when !_callbackDictionary[path].IsActive:
                    actionMap[actionName].started += _callbackDictionary[path].Callback;
                    actionMap[actionName].performed += _callbackDictionary[path].Callback;
                    actionMap[actionName].canceled += _callbackDictionary[path].Callback;
                    _callbackDictionary[path].IsActive = true;
                    break;
                case false when _callbackDictionary[path].IsActive:
                    actionMap[actionName].started -= _callbackDictionary[path].Callback;
                    actionMap[actionName].performed -= _callbackDictionary[path].Callback;
                    actionMap[actionName].canceled -= _callbackDictionary[path].Callback;
                    _callbackDictionary[path].IsActive = false;
                    break;
            }
        }
        /// <summary>
        /// 指定したActionMapをアクティブにする。それ以外は非アクティブになる。
        /// </summary>
        public void SwitchActionMap(ActionMapType actionMapType)
        {
            if (!_actionMapDictionary.ContainsKey(actionMapType)) return;
            foreach (var actionMap in _actionMapDictionary)
            {
                actionMap.Value.Disable();
            }
            _actionMapDictionary[actionMapType].Enable();
        }
        public void OnJump(InputAction.CallbackContext context)
        {
            if(context.performed) _jumpInput.OnNext(Unit.Default);
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            if(context.performed)
                _moveInput.Value = new Vector2(context.ReadValue<float>(), 0f);
            else if(context.canceled)
                _moveInput.Value = Vector2.zero;
        }
        public void OnUseSoulSkill(InputAction.CallbackContext context)
        {
            if(context.performed) _soulSkillInput.OnNext(Unit.Default);
        }

        public void OnPause(InputAction.CallbackContext context)
        {
            if(context.performed) _pauseInput.OnNext(Unit.Default);
        }
        void DeviceUpdate(InputDevice device, InputDeviceChange change)
        {
            if (Gamepad.current == null)
            {
                _playerInput.SwitchCurrentControlScheme(SchemeType.KeyboardMouse.ToString(), InputSystem.devices.ToArray());
            }
            else
            {
                _playerInput.SwitchCurrentControlScheme(SchemeType.Gamepad.ToString(), InputSystem.devices.ToArray());
            }
        }

        public enum ActionMapType
        {
            Player,
            UI
        }

        public enum ActionType
        {
            Jump,
            Move,
            UseSoulSkill,
            Menu
        }

        public enum SchemeType
        {
            KeyboardMouse,
            Gamepad
        }
    }

    public class InputCallbackData
    {
        public readonly Action<InputAction.CallbackContext> Callback;
        public bool IsActive;
        private InputCallbackData(){}
        public InputCallbackData(Action<InputAction.CallbackContext> callback)
        {
            Callback = callback;
            IsActive = false;
        }
    }
}
