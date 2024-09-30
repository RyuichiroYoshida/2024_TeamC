using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using SoulRunProject.Common;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using ActionMapType = SoulRunProject.InGame.PlayerInputManager.ActionMapType;
using ActionType = SoulRunProject.InGame.PlayerInputManager.ActionType;
using SceneManager = HikanyanLaboratory.SceneManagement.SceneManager;

namespace SoulRunProject.InGame
{
    public class TutorialManager : MonoBehaviour
    {
        [SerializeField] private Image _fadePanel;
        [SerializeReference, SubclassSelector] private List<TutorialContentBase> _tutorialContents;
        [SerializeField] private EndTutorial _endTutorial;

        private void Start()
        {
            foreach (var tutorialContent in _tutorialContents)
            {
                tutorialContent.TutorialUI.SetActive(false);
            }

            _fadePanel.gameObject.SetActive(false);
            TakeTutorial(destroyCancellationToken).Forget();
        }

        /// <summary>
        /// チュートリアル再生
        /// </summary>
        async UniTaskVoid TakeTutorial(CancellationToken ct)
        {
            var inputManager = PlayerInputManager.Instance;
            inputManager.BindAction(ActionMapType.Player, ActionType.Move, false);
            inputManager.BindAction(ActionMapType.Player, ActionType.Jump, false);
            inputManager.BindAction(ActionMapType.Player, ActionType.UseSoulSkill, false);
            inputManager.BindAction(ActionMapType.Player, ActionType.Menu, false);
            
            foreach (var tutorialContent in _tutorialContents)
            {
                await UniTask.WaitForSeconds(tutorialContent.StandbyTime, cancellationToken: ct);
                await tutorialContent.WaitAction(inputManager, _fadePanel, ct);
            }
            _endTutorial.Initialize(_fadePanel);
            await _endTutorial.WaitAction(ct);
            inputManager.BindAction(ActionMapType.Player, ActionType.Menu, true);
        }
    }

    [Serializable, Name("移動")]
    public class MoveTutorial : TutorialContentBase
    {
        public override async UniTask WaitAction(PlayerInputManager inputManager, Image fadePanel, CancellationToken ct)
        {
            PauseManager.Pause(true);
            inputManager.BindAction(ActionMapType.Player, ActionType.Move, true);
            fadePanel.gameObject.SetActive(true);
            _tutorialUI.SetActive(true);
            await inputManager.MoveInput;
            PauseManager.Pause(false);
            fadePanel.gameObject.SetActive(false);
            _tutorialUI.SetActive(false);
        }
    }

    [Serializable, Name("ジャンプ")]
    public class JumpTutorial : TutorialContentBase
    {
        public override async UniTask WaitAction(PlayerInputManager inputManager, Image fadePanel, CancellationToken ct)
        {
            PauseManager.Pause(true);
            inputManager.BindAction(ActionMapType.Player, ActionType.Jump, true);
            fadePanel.gameObject.SetActive(true);
            _tutorialUI.SetActive(true);

            await inputManager.JumpInput.First().ToUniTask(cancellationToken:ct);
            PauseManager.Pause(false);
            fadePanel.gameObject.SetActive(false);
            _tutorialUI.SetActive(false);
        }
    }

    [Serializable, Name("ソウル技")]
    public class SoulSkillTutorial : TutorialContentBase
    {
        [SerializeField] private SoulSkillManager _soulSkillManager;
        
        public override async UniTask WaitAction(PlayerInputManager inputManager, Image fadePanel, CancellationToken ct)
        {
            PauseManager.Pause(true);
            inputManager.BindAction(ActionMapType.Player, ActionType.UseSoulSkill, true);
            fadePanel.gameObject.SetActive(true);
            _tutorialUI.SetActive(true);
            _soulSkillManager.AddSoul(1000);
            
            await inputManager.SoulSkillInput.First().ToUniTask(cancellationToken:ct);
            PauseManager.Pause(false);
            fadePanel.gameObject.SetActive(false);
            _tutorialUI.SetActive(false);
        }
    }

    [Serializable, Name("レベルアップ")]
    public class LevelUpTutorial : TutorialContentBase
    {
        [SerializeField, Tooltip("経験値スポーンからこのチュートリアル終了までの時間")] private float _tutorialDuration;
        [SerializeField] private LootTable _expLoot;
        [SerializeField] private Vector3 _expSpawnPosition;
        
        public override async UniTask WaitAction(PlayerInputManager inputManager, Image fadePanel, CancellationToken ct)
        {
            DropManager.Instance.RequestDrop(_expLoot, _expSpawnPosition);
            await UniTask.WaitForSeconds(_tutorialDuration, cancellationToken:ct);
        }
    }

    [Serializable, Name("チュートリアル終了")]
    public class EndTutorial
    {
        [SerializeField] private float _standbyTime;
        [SerializeField] private float _fadeTime;
        [SerializeField] private int _loadSceneIndex;
        
        private Image _fadePanel;
        
        public void Initialize(Image fadePanel)
        {
            _fadePanel = fadePanel;
        }
        
        public async UniTask WaitAction(CancellationToken ct)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(_standbyTime), cancellationToken:ct);
            _fadePanel.gameObject.SetActive(true);
            Color color = _fadePanel.color;
            color.a = 0;
            _fadePanel.color = color;
            await SceneManager.Instance.LoadSceneWithFade("StraightInGame");
        }
    }

    [Serializable, Name("基底クラス")]
    public class TutorialContentBase
    {
        [SerializeField] private float _standbyTime;
        [SerializeField] protected GameObject _tutorialUI;

        public float StandbyTime => _standbyTime;
        public GameObject TutorialUI => _tutorialUI;

        public virtual async UniTask WaitAction(PlayerInputManager inputManager, Image fadePanel, CancellationToken ct){}
    }
}
