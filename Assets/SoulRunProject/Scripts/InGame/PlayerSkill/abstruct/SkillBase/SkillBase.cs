using System;
using SoulRunProject.InGame;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SoulRunProject.Common
{
    public enum PlayerSkill
    {
        SoulBullet = 0,
        HolyField = 1,
        SoulSword = 2,
        SoulShell = 3,
        SoulRay = 4,
        SoulOfHealing = 5,
        SoulFrame = 6
    }

    /// <summary>
    ///     スキルの基底クラス
    /// </summary>
    [Serializable]
    [Name("スキルの基底クラス")]
    public abstract class SkillBase : ScriptableObject, IPausable
    {
        [SerializeField] [HideInInspector] private PlayerSkill _skillType;
        [SerializeField] [HideInInspector] private string _skillName;
        [SerializeField] [HideInInspector] private string _explanatoryText;
        [SerializeField] [HideInInspector] private Sprite _skillIcon;
        [SerializeField] [HideInInspector] public int MaxSkillLevel = 5;
        [SerializeField] [HideInInspector] protected SkillLevelUpEvent SkillLevelUpEvent;

        [SerializeReference] [CustomLabel("スキルパラメーター")] protected ISkillParameter SkillParam;

        private int _elementCount;

        protected bool IsPause;
        protected PlayerManager PlayerManagerInstance;
        protected Transform PlayerTransform;

        public PlayerSkill SkillType => _skillType;
        public string SkillName => _skillName;
        public string ExplanatoryText => _explanatoryText;
        public Sprite SkillIcon => _skillIcon;
        public int CurrentLevel { get; private set; } = 1;


        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            _elementCount = SkillLevelUpEvent.LevelUpType.LevelUpEventListList.Count;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        /// <summary>
        ///     リストに要素を追加した際に、最後の要素がコピーされ同じ参照を保持するため、追加した要素を初期化している。
        /// </summary>
        private void OnValidate()
        {
            var currentElementCount = SkillLevelUpEvent.LevelUpType.LevelUpEventListList.Count;
            if (_elementCount != currentElementCount)
            {
                if (_elementCount < currentElementCount)
                {
                    Debug.Log("RefreshElement");
                    SkillLevelUpEvent.LevelUpType.RefreshElement();
                }

                _elementCount = currentElementCount;
            }
        }

        public void Pause(bool isPause)
        {
            this.IsPause = isPause;
            OnSwitchPause(isPause);
        }

        protected virtual void OnSwitchPause(bool isPause)
        {
            
        }
        private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {
            Register();
            InitializeParamOnSceneLoaded();
        }

        /// <summary>
        /// シーンロード時にパラメータを初期化するように登録する。
        /// </summary>
        protected virtual void InitializeParamOnSceneLoaded()
        {
            SkillParam.InitializeParamOnSceneLoaded();
            CurrentLevel = 1;
            IsPause = false;
        }

        /// <summary> スキルレベルアップ可能かどうか </summary>
        public bool CanLevelUp()
        {
            return CurrentLevel <= MaxSkillLevel;
        }


        public void InitialiseSkill(in PlayerManager playerManager , in Transform playerTransform)
        {
            PlayerManagerInstance = playerManager;
            PlayerTransform = playerTransform;
            SkillParam.SetPlayerStatus(playerManager.CurrentPlayerStatus);
            StartSkill();
        }

        protected virtual void StartSkill()
        {
            
        }

        public virtual void UpdateSkill(float deltaTime)
        {
        }

        /// <summary>レベルアップ時イベント</summary>
        public virtual void OnLevelUp()
        {
        }

        /// <summary>スキル進化</summary>
        public void LevelUp()
        {
            CurrentLevel++;
            if (CanLevelUp())
            {
                SkillLevelUpEvent.LevelUp(CurrentLevel, SkillParam);
                OnLevelUp();
            }
            else
            {
                Debug.LogError("レベル上限を超えています。");
            }
        }
        
        public void Register()
        {
            PauseManager.RegisterPausableObject(this);
        }

        public void UnRegister()
        {
            PauseManager.UnRegisterPausableObject(this);
        }
        
    }
#if UNITY_EDITOR
    /// <summary>
    ///     エラーを出さないためのエディタ拡張クラス、SKillBaseクラスの派生クラスにも適応される
    ///     https://forum.unity.com/threads/nullreferenceexception-serializedobject-of-serializedproperty-has-been-disposed.1431907/
    /// </summary>
    [CustomEditor(typeof(SkillBase), true)]
    public class SkillBaseEditor : Editor
    {
        private SerializedProperty _explanatoryProperty;
        private SerializedProperty _iconProperty;
        private SerializedProperty _levelUpEventListListProperty;

        private int _levelUpListLastIndex;
        private SerializedProperty _nameProperty;
        private SerializedProperty _skillTypeProperty;

        private void OnEnable()
        {
            _skillTypeProperty = serializedObject.FindProperty("_skillType");
            _nameProperty = serializedObject.FindProperty("_skillName");
            _explanatoryProperty = serializedObject.FindProperty("_explanatoryText");
            _iconProperty = serializedObject.FindProperty("_skillIcon");
            _levelUpEventListListProperty =
                serializedObject.FindProperty("SkillLevelUpEvent._levelUpType._levelUpEventListList");
            _levelUpListLastIndex = _levelUpEventListListProperty.arraySize - 1;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            base.OnInspectorGUI();
            //enumの入力
            _skillTypeProperty.enumValueIndex =
                (int)(PlayerSkill)EditorGUILayout.EnumPopup("スキルタイプ", (PlayerSkill)_skillTypeProperty.enumValueIndex);
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("-----スキル情報-----");
            EditorGUILayout.PropertyField(_nameProperty, new GUIContent("スキル名"), true);
            EditorGUILayout.LabelField("スキル説明文");
            _explanatoryProperty.stringValue =
                EditorGUILayout.TextArea(_explanatoryProperty.stringValue);
            EditorGUILayout.PropertyField(_iconProperty, new GUIContent("スキルアイコン"), true);
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("-----スキル性能-----");
            // ExampleCustomList list = new ExampleCustomList(_levelUpEventListListProperty);
            // list.DoLayoutList();
            EditorGUILayout.PropertyField(_levelUpEventListListProperty, new GUIContent("レベルアップデータ"), true);
            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}