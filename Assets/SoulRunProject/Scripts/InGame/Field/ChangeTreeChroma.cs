using SoulRunProject.Common;
using UnityEditor;
using UnityEngine;

namespace SoulRunProject.InGame
{
    /// <summary>
    /// 木の色彩をプレイヤーからの距離に応じて変える
    /// </summary>
    public class ChangeTreeChroma : HitDamageEffectManager
    {
        [SerializeField, Tooltip("最も明るい色")] private Color _lightestColor;
        [SerializeField, Tooltip("最も暗い色")] private Color _darkestColor;
        [SerializeField, HideInInspector] private float _minVariableDistance;
        [SerializeField, HideInInspector] private float _maxVariableDistance;

        private Transform _playerTransform;

        protected override void Awake()
        {
            base.Awake();

            _playerTransform = FindObjectOfType<PlayerManager>().transform;
        }

        private void Update()
        {
            if (!_hitFadeBlinking) // HitEffect中は色を変えない
            {
                float xDistance = Vector3.Distance(transform.position, _playerTransform.position);
                float value = Mathf.Clamp((xDistance - _minVariableDistance) / (_maxVariableDistance - _minVariableDistance), 0, 1);
                _defaultColor = Color.Lerp(_lightestColor, _darkestColor, value);
                _copyMaterial.SetColor(PramID, _defaultColor);
            }
        }

#if UNITY_EDITOR
        [CustomEditor(typeof(ChangeTreeChroma))]
        public class ChangeTreeChromaEditor : Editor
        {
            private ChangeTreeChroma _target;

            private void Awake()
            {
                _target = target as ChangeTreeChroma;
            }

            public override void OnInspectorGUI()
            {
                DrawDefaultInspector();
                
                EditorGUILayout.BeginHorizontal();
                EditorGUIUtility.labelWidth = 32;
                GUILayoutOption[] fieldOptions = new GUILayoutOption[]
                {
                    GUILayout.MinWidth(0),
                    GUILayout.MaxWidth(128)
                };
                EditorGUILayout.LabelField("色が変動する距離の範囲", fieldOptions);
                
                GUILayout.FlexibleSpace();
                
                fieldOptions = new GUILayoutOption[]
                {
                    GUILayout.MinWidth(84),
                    GUILayout.MaxWidth(84 < EditorGUIUtility.currentViewWidth * 0.27f? EditorGUIUtility.currentViewWidth * 0.27f : 84)
                };
                
                EditorGUI.BeginChangeCheck();
                _target._minVariableDistance =
                    EditorGUILayout.FloatField("Min", _target._minVariableDistance, fieldOptions);
                GUILayout.Space(EditorGUIUtility.currentViewWidth * 0.03f);
                _target._maxVariableDistance =
                    EditorGUILayout.FloatField("Max", _target._maxVariableDistance, fieldOptions);
                EditorGUILayout.EndHorizontal();
                
                if (EditorGUI.EndChangeCheck())
                {
                    if (_target._minVariableDistance > _target._maxVariableDistance)
                    {
                        _target._minVariableDistance = _target._maxVariableDistance;
                    }
                    
                    EditorUtility.SetDirty(_target);
                }
            }
        }
#endif
    }
}
