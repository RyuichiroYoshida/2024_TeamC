using System;
using System.Collections.Generic;
using System.Linq;
using SoulRunProject.Common;
using SoulRunProject.SoulMixScene;
using SoulRunProject.SoulRunProject.Scripts.Common.Core.Singleton;
using UniRx;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace SoulRunProject.InGame
{
    public class SkillManager : MonoBehaviour, IPlayerPausable
    {
        [SerializeField, Header("初期スキルリスト")] private List<PlayerSkill> _defaultPlayerSkills;
        [SerializeField, CustomLabel("初期のスキル保有可能数")] private int _initialNumberOfPossessions;
        [SerializeField, Header("スキルの保有可能数が増えるレベル")] private int[] _increaseSkillLevels;
        [SerializeField, HideInInspector] private Image[] _skillIconImage = new Image[5];
        private readonly List<SkillBase> _currentSkills = new(5);
        private List<SkillBase> _skillData;
        public List<SkillBase> SkillData => _skillData;
        public List<SkillBase> CurrentSkill => _currentSkills;
        /// <summary>
        /// 現在所持しているスキル名リスト
        /// </summary>
        public List<PlayerSkill> CurrentSkillTypes => _currentSkills.Select(x => x.SkillType).ToList();
        /// <summary> 現在のスキル保有可能数 </summary>
        public int CurrentNumberOfPossessions { get; private set; }
        
        private bool _isPause;
        
        private PlayerManager _playerManager;
        
        public void Start()
        {
            _playerManager = FindObjectOfType<PlayerManager>();
            if (MyRepository.Instance.TryGetDataList<SkillBase>(out var dataSet))
            {
                _skillData = dataSet ;
            }

            foreach (var skill in _defaultPlayerSkills)
            {
                AddSkill(skill);
            }

            CurrentNumberOfPossessions = _initialNumberOfPossessions;
            GetComponent<PlayerLevelManager>().OnLevelUp.Where(level => _increaseSkillLevels.Contains(level))
                .Subscribe(_ => CurrentNumberOfPossessions++).AddTo(this);
        }
        
        public void Update()
        {
            if (!_isPause)
            {
                foreach (var skill in _currentSkills)
                {
                    skill.UpdateSkill(Time.deltaTime);
                }
            }
        }
        
        /// <summary>
        /// スキルを追加する。追加時にStartSkill()を呼ぶ。
        /// </summary>
        /// <param name="skillType">スキル名</param>
        public void AddSkill(PlayerSkill skillType)
        {
            var skill = _skillData.FirstOrDefault(x => x.SkillType == skillType);
            if (skill != null)
            {
                _currentSkills.Add(skill);
                skill.InitialiseSkill(_playerManager , transform);
                _skillIconImage[_currentSkills.Count - 1].sprite = skill.SkillIcon; // スキルアイコンの表示
            }
            else
            {
                Debug.LogError("スキルリストに入っていないスキルが追加選択されました。");
            }
        }

        /// <summary>
        /// スキルのレベルアップ
        /// </summary>
        /// <param name="skillType">スキル名</param>
        public void LevelUpSkill(PlayerSkill skillType)
        {
            var skill = _currentSkills.FirstOrDefault(x => x.SkillType == skillType);
            if (skill != null)
            {
                skill.LevelUp();
            }
            else
            {
                Debug.LogError("スキルリストに入っていないスキルがレベルアップ選択されました。");
            }
        }
        
        public void Pause(bool isPause)
        {
            _isPause = isPause;
            foreach (var skill in _currentSkills)
            {
                skill.Pause(isPause);
            }
        }

        #if  UNITY_EDITOR
        [CustomEditor(typeof(SkillManager))]
        public class SkillManagerEditor : Editor
        {
            private SkillManager _skillManager;
            private bool _groupIsOpen;
            
            private void Awake()
            {
                _skillManager = target as SkillManager;
            }

            public override void OnInspectorGUI()
            {
                DrawDefaultInspector();

                _groupIsOpen = EditorGUILayout.BeginFoldoutHeaderGroup(_groupIsOpen, "スキルアイコン");

                if (_groupIsOpen)
                {
                    _skillManager._skillIconImage[0] = 
                        EditorGUILayout.ObjectField(_skillManager._skillIconImage[0], typeof(Image), true) as Image;
                    _skillManager._skillIconImage[1] = 
                        EditorGUILayout.ObjectField(_skillManager._skillIconImage[1], typeof(Image), true) as Image;
                    _skillManager._skillIconImage[2] = 
                        EditorGUILayout.ObjectField(_skillManager._skillIconImage[2], typeof(Image), true) as Image;
                    _skillManager._skillIconImage[3] = 
                        EditorGUILayout.ObjectField(_skillManager._skillIconImage[3], typeof(Image), true) as Image;
                    _skillManager._skillIconImage[4] = 
                        EditorGUILayout.ObjectField(_skillManager._skillIconImage[4], typeof(Image), true) as Image;
                }
                
                EditorGUILayout.EndFoldoutHeaderGroup();
            }
        }
        #endif
    }
}
