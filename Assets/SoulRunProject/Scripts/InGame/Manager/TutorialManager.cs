using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using SoulRunProject.Common;
using UnityEngine;

namespace SoulRunProject.InGame
{
    public class TutorialManager : MonoBehaviour
    {
        [SerializeField] private PlayerInput _playerInput;
        [SerializeReference, SubclassSelector] private List<TutorialContentBase> _tutorialContents;

        private void Start()
        {
            foreach (var tutorialContent in _tutorialContents)
            {
                tutorialContent.TutorialPanel.SetActive(false);
            }

            _ = TakeTutorial();
        }

        /// <summary>
        /// チュートリアル再生
        /// </summary>
        async UniTask TakeTutorial()
        {
            _playerInput.MoveInputActive = false;
            _playerInput.JumpInputActive = false;
            _playerInput.ShiftInputActive = false;
            
            foreach (var tutorialContent in _tutorialContents)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(tutorialContent.StandbyTime));
                tutorialContent.TutorialPanel.SetActive(true);
                await tutorialContent.WaitAction(_playerInput);
                tutorialContent.TutorialPanel.SetActive(false);
            }
        }
    }

    [Serializable, Name("移動")]
    public class MoveTutorial : TutorialContentBase
    {
        public override async UniTask WaitAction(PlayerInput input)
        {
            PauseManager.Pause(true);
            input.MoveInputActive = true;
            await UniTask.WaitUntil(() => input.MoveInput.Value != Vector2.zero);
            PauseManager.Pause(false);
        }
    }

    [Serializable, Name("ジャンプ")]
    public class JumpTutorial : TutorialContentBase
    {
        public override async UniTask WaitAction(PlayerInput input)
        {
            PauseManager.Pause(true);
            input.JumpInputActive = true;
            await UniTask.WaitUntil(() => input.JumpInput.Value);
            PauseManager.Pause(false);
        }
    }

    [Serializable, Name("ソウル技")]
    public class SoulSkillTutorial : TutorialContentBase
    {
        public override async UniTask WaitAction(PlayerInput input)
        {
            PauseManager.Pause(true);
            input.ShiftInputActive = true;
            await UniTask.WaitUntil(() => input.ShiftInput.Value);
            PauseManager.Pause(false);
        }
    }

    [Serializable, Name("基底クラス")]
    public class TutorialContentBase
    {
        [SerializeField] private float _standbyTime;
        [SerializeField] private GameObject _tutorialPanel;

        public float StandbyTime => _standbyTime;
        public GameObject TutorialPanel => _tutorialPanel;

        public virtual async UniTask WaitAction(PlayerInput input){}
    }
}
