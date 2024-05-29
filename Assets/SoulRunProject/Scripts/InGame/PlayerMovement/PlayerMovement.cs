using System;
using SoulRunProject.Common;
using UniRx;
using UniRx.Triggers;
using UnityEditor;
using UnityEngine;

namespace SoulRunProject.InGame
{
    /// <summary>
    /// プレイヤー移動
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerMovement : MonoBehaviour, IPlayerPausable
    {
        [SerializeField] private Animator _playerAnimator;
        [SerializeField] private float _moveSpeed;
        [SerializeField] private float _jumpPower;
        [SerializeField] private float _stickingPowerOnGround;
        [SerializeField] private float _normalGrav;
        [SerializeField] private float _jumpGrav;
        [SerializeField, CustomLabel("Pivotと接地点との距離")] private float _distanceBetweenPivotAndGroundPoint;
        [SerializeField, HideInInspector] private float _xMoveRangeMin;
        [SerializeField, HideInInspector] private float _xMoveRangeMax;
        [SerializeField, HideInInspector] private bool _canZAxisMovement;
        [SerializeField, HideInInspector] private float _zAxisMoveSpeed;
        [SerializeField, HideInInspector] private float _zMoveRangeMin;
        [SerializeField, HideInInspector] private float _zMoveRangeMax;

        private Rigidbody _rb;
        private readonly BoolReactiveProperty _isGround = new BoolReactiveProperty(false);
        private bool _jumping;
        private float Grav => _jumping? _jumpGrav : _normalGrav;
        private Vector3 _playerVelocity;
        private float _onFieldVelocityY; // フィールドに沿った場合のｙ速度
        private float _yAxisGroundLine;
        private bool _inPause;
        private int _spinIndex;

        public BoolReactiveProperty IsGround => _isGround;
        public event Action OnJumped;
        public bool IsJumping => _jumping;
        /// <summary> プレイヤー地点の地面の高さ </summary>
        public float GroundHeight => _yAxisGroundLine;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _rb.useGravity = false;

            _isGround.AddTo(this);
            this.OnDestroyAsObservable().Subscribe(_ => OnJumped = null);
            _spinIndex = CriAudioManager.Instance.PlaySE("SE_Spin");
            CriAudioManager.Instance.PauseSE(_spinIndex);
        }

        private void Update()
        {
            if (_inPause) return;
            LimitPosition();
            GroundCheck();
            _rb.velocity = _playerVelocity;
        }

        private void FixedUpdate()
        {
            if (_inPause) return;
            
            if (_isGround.Value && !_jumping)
            {
                _playerVelocity.y = _onFieldVelocityY - _stickingPowerOnGround;
            }
            else
            {
                _playerVelocity.y -= Grav * Time.fixedDeltaTime;
            }
        }

        public void InputMove(Vector2 moveInput)
        {
            if (_inPause) return;
            _playerVelocity.x = moveInput.x * _moveSpeed;
            if (_canZAxisMovement) _playerVelocity.z = moveInput.y * _zAxisMoveSpeed;
        }

        public void Jump()
        {
            if (_inPause) return;

            if (_isGround.Value)
            {
                _playerVelocity.y = _jumpPower;
                _jumping = true;
                CriAudioManager.Instance.PlaySE("SE_Jump");
                OnJumped?.Invoke();
            }
        }

        /// <summary>
        /// 地面を検出してpositionと音を調整する
        /// </summary>
        private void GroundCheck()
        {
            // 地面の検出
            RaycastHit[] hits = Physics.RaycastAll(transform.position + Vector3.up * 10, Vector3.down);

            foreach (var hit in hits)
            {
                if (hit.transform.TryGetComponent(out FieldSegment field))
                {
                    _yAxisGroundLine = hit.point.y;
                    Vector3 dir = Vector3.ProjectOnPlane(Vector3.forward, hit.normal);
                    _onFieldVelocityY = dir.y * 30 / dir.z; // todo 30 => プレイヤーのスピードを参照する
                    break;
                }
            }
            
            if (transform.position.y <= _yAxisGroundLine + _distanceBetweenPivotAndGroundPoint)
            {
                Vector3 pos = transform.position;
                pos.y = _yAxisGroundLine + _distanceBetweenPivotAndGroundPoint;
                transform.position = pos;

                if (!_isGround.Value)
                {
                    CriAudioManager.Instance.PlaySE("SE_Landing");
                    CriAudioManager.Instance.PauseSE(_spinIndex);
                    _isGround.Value = true;
                    _jumping = false;
                }
            }
            else if (_isGround.Value)
            {
                CriAudioManager.Instance.ResumeSE(_spinIndex);
                _isGround.Value = false;
            }
        }

        public void Pause(bool isPause)
        {
            _inPause = isPause;

            if (isPause)
            {
                _rb.Sleep();
            }
            else
            {
                _rb.WakeUp();
            }
        }

        /// <summary>
        /// プレイヤーのポジションを一定範囲内に限定する
        /// </summary>
        void LimitPosition()
        {
            // x座標軸の制限
            if (transform.position.x <= _xMoveRangeMin) // x マイナス側の制限
            {
                // 位置の制限
                Vector3 pos = transform.position;
                pos.x = _xMoveRangeMin;
                transform.position = pos;
                // Velocityの制限
                _playerVelocity.x = Mathf.Clamp(_playerVelocity.x, 0, _moveSpeed);
            }
            else if (transform.position.x >= _xMoveRangeMax) // x プラス側の制限
            {
                // 位置の制限
                Vector3 pos = transform.position;
                pos.x = _xMoveRangeMax;
                transform.position = pos;
                // Velocityの制限
                _playerVelocity.x = Mathf.Clamp(_playerVelocity.x, -_moveSpeed, 0);
            }

            if (!_canZAxisMovement) return;

            // z座標軸の制限
            if (transform.position.z <= _zMoveRangeMin) // z マイナス側の制限
            {
                // 位置の制限
                Vector3 pos = transform.position;
                pos.z = _zMoveRangeMin;
                transform.position = pos;
                // Velocityの制限
                _playerVelocity.z = Mathf.Clamp(_playerVelocity.z, 0, _zAxisMoveSpeed);
            }
            else if (transform.position.z >= _zMoveRangeMax)
            {
                // 位置の制限
                Vector3 pos = transform.position;
                pos.z = _zMoveRangeMax;
                transform.position = pos;
                // Velocityの制限
                _playerVelocity.z = Mathf.Clamp(_playerVelocity.z, -_zAxisMoveSpeed, 0);
            }
        }

        public void RotatePlayer(Vector2 input)
        {
            if (input.x > 0)
            {
                _playerAnimator.SetBool("IsLeft", true);
                _playerAnimator.SetBool("IsRight", false);
            }
            else if (input.x < 0)
            {
                _playerAnimator.SetBool("IsRight", true);
                _playerAnimator.SetBool("IsLeft", false);
            }
            else
            {
                _playerAnimator.SetBool("IsRight", false);
                _playerAnimator.SetBool("IsLeft", false);
            }
        }



#if UNITY_EDITOR
        private void OnValidate()
        {
            SceneView.RepaintAll();
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            Vector3 leftPos = Vector3.right * _xMoveRangeMin;
            Vector3 rightPos = Vector3.right * _xMoveRangeMax;
            Gizmos.DrawLine(leftPos, rightPos);
            Gizmos.DrawLine(leftPos + Vector3.up, leftPos - Vector3.up);
            Gizmos.DrawLine(rightPos + Vector3.up, rightPos - Vector3.up);

            if (!_canZAxisMovement) return;

            Vector3 backPos = Vector3.forward * _zMoveRangeMin;
            Vector3 forwardPos = Vector3.forward * _zMoveRangeMax;
            Gizmos.DrawLine(backPos, forwardPos);
            Gizmos.DrawLine(backPos + Vector3.up, backPos - Vector3.up);
            Gizmos.DrawLine(forwardPos + Vector3.up, forwardPos - Vector3.up);
        }

        /// <summary>
        /// move rangeの拡張
        /// </summary>
        [CustomEditor(typeof(PlayerMovement))]
        public class PlayerMovementEditor : Editor
        {
            private PlayerMovement _playerMovement;

            private void Awake()
            {
                _playerMovement = target as PlayerMovement;
            }

            public override void OnInspectorGUI()
            {
                DrawDefaultInspector();

                EditorGUILayout.BeginHorizontal();
                float width = EditorGUIUtility.labelWidth;
                EditorGUIUtility.labelWidth = 32;
                GUILayoutOption[] fieldOptions = new GUILayoutOption[]
                {
                    GUILayout.MinWidth(0),
                    GUILayout.MaxWidth(98)
                };
                EditorGUILayout.LabelField("X軸座標の移動範囲", fieldOptions);

                GUILayout.FlexibleSpace();

                fieldOptions = new GUILayoutOption[]
                {
                    GUILayout.MinWidth(84),
                    GUILayout.MaxWidth(84 < EditorGUIUtility.currentViewWidth * 0.27f
                        ? EditorGUIUtility.currentViewWidth * 0.27f
                        : 84)
                };

                EditorGUI.BeginChangeCheck();
                _playerMovement._xMoveRangeMin =
                    EditorGUILayout.FloatField("Min", _playerMovement._xMoveRangeMin, fieldOptions);
                GUILayout.Space(EditorGUIUtility.currentViewWidth * 0.03f);
                _playerMovement._xMoveRangeMax =
                    EditorGUILayout.FloatField("Max", _playerMovement._xMoveRangeMax, fieldOptions);
                EditorGUILayout.EndHorizontal();

                if (_playerMovement._xMoveRangeMin > _playerMovement._xMoveRangeMax)
                {
                    _playerMovement._xMoveRangeMin = _playerMovement._xMoveRangeMax;
                }

                EditorGUIUtility.labelWidth = width;
                _playerMovement._canZAxisMovement =
                    EditorGUILayout.Toggle("前後に移動可能か", _playerMovement._canZAxisMovement);

                EditorGUI.BeginDisabledGroup(!_playerMovement._canZAxisMovement);
                _playerMovement._zAxisMoveSpeed =
                    EditorGUILayout.FloatField("前後移動速度", _playerMovement._zAxisMoveSpeed);
                EditorGUIUtility.labelWidth = 32;
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Z軸座標の移動範囲", fieldOptions);
                GUILayout.FlexibleSpace();

                _playerMovement._zMoveRangeMin =
                    EditorGUILayout.FloatField("Min", _playerMovement._zMoveRangeMin, fieldOptions);
                GUILayout.Space(EditorGUIUtility.currentViewWidth * 0.03f);
                _playerMovement._zMoveRangeMax =
                    EditorGUILayout.FloatField("Max", _playerMovement._zMoveRangeMax, fieldOptions);
                EditorGUILayout.EndHorizontal();
                EditorGUI.EndDisabledGroup();

                if (_playerMovement._zMoveRangeMin > _playerMovement._zMoveRangeMax)
                {
                    _playerMovement._zMoveRangeMin = _playerMovement._zMoveRangeMax;
                }

                if (EditorGUI.EndChangeCheck())
                {
                    SceneView.RepaintAll();
                }

                Undo.RecordObject(_playerMovement, "set playerMovement");
            }
        }
#endif
    }
}