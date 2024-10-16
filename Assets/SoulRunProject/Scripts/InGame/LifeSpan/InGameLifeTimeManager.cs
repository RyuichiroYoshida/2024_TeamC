using SoulRunProject.InGame;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace SoulRunProject.Common
{
    /// <summary>
    /// インゲームの依存性注入を行うクラス
    /// </summary>
    public class InGameLifeTimeManager : LifetimeScope
    {
        [SerializeField] private PlayerCamera _camera;
        [SerializeField] private StageManager _stageManager;
        [SerializeField] private PlayerManager _playerManager;
        [SerializeField] private PlayerLevelManager _playerLevelManager;
        [SerializeField] private SoulSkillManager _soulSkillManager;
        [SerializeField] private SkillManager _skillManager;
        [SerializeField] private LevelUpItemData _levelUpItemData;
        [SerializeField] private ResultView _resultView;
        [SerializeField] private CommonView _commonView;
        [SerializeField] private StageNameView _stageNameView;
        [SerializeField] private LevelUpView _levelUpView;
        [SerializeField] private PauseView _pauseView;
        [SerializeField] private ScoreData _scoreData;
        protected override void Configure(IContainerBuilder builder)
        {
            //ドメイン層
            builder.RegisterInstance(_camera);
            builder.RegisterInstance(_stageManager);
            builder.RegisterInstance(_playerManager);
            builder.RegisterInstance(PlayerInputManager.Instance);
            builder.RegisterInstance(gameObject);
            builder.RegisterInstance(_stageNameView);
            
            //アプリケーション層
            builder.Register<EnterInGameState>(Lifetime.Singleton);
            builder.Register<EnterStageState>(Lifetime.Singleton);
            builder.Register<PlayingRunGameState>(Lifetime.Singleton);
            builder.Register<EnterBossStageState>(Lifetime.Singleton);
            builder.Register<PlayingBossStageState>(Lifetime.Singleton);
            builder.Register<ResultState>(Lifetime.Singleton);
            builder.Register<PauseState>(Lifetime.Singleton);
            builder.Register<LevelUpState>(Lifetime.Singleton);
            builder.Register<ExitInGameState>(Lifetime.Singleton);
            builder.Register<InGameManager>(Lifetime.Singleton);
            builder.RegisterInstance(_playerLevelManager);
            builder.RegisterInstance(_skillManager);
            builder.RegisterInstance(_soulSkillManager);
            builder.RegisterInstance(_levelUpItemData);
            builder.RegisterInstance(_scoreData);
            
            //プレゼンテーション層
            builder.RegisterComponent(_resultView);
            builder.Register<ResultPresenter>(Lifetime.Singleton);
            builder.RegisterComponent(_commonView);
            builder.Register<CommonUIPresenter>(Lifetime.Singleton);
            builder.Register<StageEnterPresenter>(Lifetime.Singleton);
            builder.RegisterComponent(_levelUpView);
            builder.Register<LevelUpUIPresenter>(Lifetime.Singleton);
            builder.RegisterComponent(_pauseView);
            builder.Register<PauseUIPresenter>(Lifetime.Singleton);
            
            //開始処理
            builder.RegisterEntryPoint<ResultPresenter>();
            builder.RegisterEntryPoint<StageEnterPresenter>();
            builder.RegisterEntryPoint<CommonUIPresenter>();
            builder.RegisterEntryPoint<LevelUpUIPresenter>();
            builder.RegisterEntryPoint<PauseUIPresenter>();
            builder.RegisterEntryPoint<InGameManager>();
        }
    }
}
