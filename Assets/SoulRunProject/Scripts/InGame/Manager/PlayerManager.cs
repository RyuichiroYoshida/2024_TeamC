﻿using System;
using System.Collections.Generic;
using System.Linq;
using SoulRunProject.InGame;
using SoulRunProject.SoulMixScene;
using UniRx;
using UnityEngine;

namespace SoulRunProject.Common
{
    /// <summary>
    /// プレイヤーを管理するクラス
    /// </summary>
    [RequireComponent(typeof(HitDamageEffectManager))]
    public class PlayerManager : MonoBehaviour , IPausable
    {
        [SerializeField] private PlayerInput _playerInput;
        [SerializeField] private BaseStatus _baseStatus;
        [SerializeField] private PlayerCamera _playerCamera;
        
        private IPlayerPausable[] _inGameTimes;
        private PlayerLevelManager _pLevelManager;
        private SkillManager _skillManager;
        private SoulSkillManager _soulSkillManager;
        private PlayerMovement _playerMovement;
        private HitDamageEffectManager _hitDamageEffectManager;
        //private PlayerStatusManager _statusManager;
        private PlayerResourceContainer _resourceContainer;
        public ReadOnlyReactiveProperty<float> CurrentHp => CurrentPlayerStatus.CurrentHpProperty;
        public PlayerResourceContainer ResourceContainer => _resourceContainer;
        //public PlayerStatusManager PlayerStatusManager => _statusManager;
        
        [CustomLabel("現在のプレイヤーのステータス")]　public PlayerStatus CurrentPlayerStatus;
        
        /// <summary>ダメージを無効化出来るかどうかの条件を格納するリスト</summary>
        public List<Func<bool>> IgnoreDamagePredicates { get; } = new();

        private void Awake()
        {
            _inGameTimes = GetComponents<IPlayerPausable>();
            _pLevelManager = GetComponent<PlayerLevelManager>();
            _skillManager = GetComponent<SkillManager>();
            _soulSkillManager = GetComponent<SoulSkillManager>();
            _playerMovement = GetComponent<PlayerMovement>();
            _hitDamageEffectManager = GetComponent<HitDamageEffectManager>();
            _resourceContainer = new();
            //_statusManager = new PlayerStatusManager(_baseStatus.Status);
            CurrentPlayerStatus = new PlayerStatus(_baseStatus.PlayerStatus);
            
            InitializeInput();
        }
        
        /// <summary>
        /// 入力を受け付けるクラスに対して入力と紐づける
        /// </summary>
        private void InitializeInput()
        {
            _playerInput.MoveInput.Subscribe(input => _playerMovement.InputMove(input));
            _playerInput.MoveInput.Subscribe(input => _playerMovement.RotatePlayer(input));
            _playerInput.JumpInput.Where(x => x).Subscribe(_ => _playerMovement.Jump()).AddTo(this);
            _playerInput.ShiftInput.Where(x => x).Subscribe(_ => UseSoulSkill()).AddTo(this);
        }

        /// <summary>
        /// Pauseの切替
        /// </summary>
        /// <param name="toPause"></param>
        public void Pause(bool toPause)
        {
            foreach (var inGameTime in _inGameTimes)
            {
                inGameTime.Pause(toPause);
            }
        }

        /// <summary>
        /// 経験値を取得する
        /// </summary>
        /// <param name="exp">経験値量</param>
        public void GetExp(int exp)
        {
            _pLevelManager.AddExp(exp);
        }
        
        public void Damage(float damage)
        {
            foreach (var predicate in IgnoreDamagePredicates.Where(cond=> cond != null))
            {
                if (predicate())
                {
                    return;
                }
            }
            
            _playerCamera.DamageCam();
            CurrentPlayerStatus.CurrentHp -= Calculator.CalcDamage(damage, CurrentPlayerStatus.DefenceValue, 0, 1);
            
            // 白色点滅メソッド
            _hitDamageEffectManager.HitFadeBlinkWhite();
            //CriAudioManager.Instance.PlaySE(CriAudioManager.CueSheet.Se, "SE_Damage");
        }

        public void Heal(float value)
        {
            CurrentPlayerStatus.CurrentHp += value;
        }
        
        
        private void Death()
        {
            Debug.Log("GameOver");
            //SwitchPause(true);
        }

        #region SoulSkill関連
        /// <summary>
        /// SoulSkillを使用する
        /// </summary>
        public void UseSoulSkill()
        {
            _soulSkillManager.UseSoulSkill();
        }
        
        public void SetSoulSkill(SoulSkillType soulSkillType)
        {
            _soulSkillManager.SetSoulSkill(soulSkillType);
        }
        
        public void AddSoul(float soul)
        {
            _soulSkillManager.AddSoul(soul);
        }

        #endregion
        
    }
}