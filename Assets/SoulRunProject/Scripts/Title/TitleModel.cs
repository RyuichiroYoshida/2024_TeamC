using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using HikanyanLaboratory.SceneManagement;
using SoulRunProject.Audio;
using SoulRunProject.Framework;
using UnityEngine;
using SoulRunProject.InGame;
using UniRx;
using UnityEngine.UI;

namespace SoulRunProject.Title
{
    /// <summary>
    /// タイトルのロジック処理を行うクラス
    /// </summary>
    public class TitleModel : MonoBehaviour
    {
        [SerializeField] float _transitionTime = 1.0f;
        [SerializeField] private string _tutorialScene = "TutorialScene";
        [SerializeField] private int _tutorialFadeTextureIndex;
        [SerializeField] private int _demoVideoDelayTime = 60;

        private Guid _bgmGuid;

        private void Start()
        {
            _bgmGuid = CriAudioManager.Instance.Play(CriAudioType.CueSheet_BGM, "BGM_Title", true);
        }

        public async void StartGame()
        {
            DebugClass.Instance.ShowLog($"ゲーム開始:{_transitionTime}秒後にインゲーム画面に遷移します");
            //ここで実行
            await SceneManager.Instance.LoadSceneWithFade(_tutorialScene);
            //  アクションマップをプレイヤーに変更する。
            PlayerInputManager.Instance.SwitchActionMap(PlayerInputManager.ActionMapType.Player);

            _cts?.Cancel();
        }

        public void OpenOption(CanvasGroup optionPanel, CanvasGroup basePanel)
        {
            basePanel.interactable = false;
            basePanel.blocksRaycasts = false;
            optionPanel.interactable = true;
            optionPanel.blocksRaycasts = true;
            optionPanel.alpha = 1f;
            optionPanel.transform.GetComponentInChildren<Selectable>().Select();
        }

        public void CloseOption(CanvasGroup optionPanel, CanvasGroup basePanel)
        {
            basePanel.interactable = true;
            basePanel.blocksRaycasts = true;
            optionPanel.interactable = false;
            optionPanel.blocksRaycasts = false;
            optionPanel.alpha = 0f;
            basePanel.transform.GetComponentInChildren<Selectable>().Select();
        }

        public void Exit()
        {
            DebugClass.Instance.ShowLog("ゲーム終了");
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false; //ゲームプレイ終了
#else
    Application.Quit();//ゲームプレイ終了
#endif
        }

        private readonly Subject<Unit> _onDemoMoviePlaySubject = new Subject<Unit>();
        private readonly ReactiveProperty<bool> _isDemoMoviePlayingProperty = new ReactiveProperty<bool>(false);
        private CancellationTokenSource _cts;
        public IObservable<Unit> OnDemoMoviePlay => _onDemoMoviePlaySubject;
        public IReactiveProperty<bool> IsDemoMoviePlaying => _isDemoMoviePlayingProperty;

        public void OpenMovie(CanvasGroup videoPanel)
        {
            videoPanel.alpha = 1f;
            CriAudioManager.Instance.Pause(CriAudioType.CueSheet_BGM, _bgmGuid);
            _ = StartCountDown();
        }

        public void CloseMovie(CanvasGroup videoPanel)
        {
            videoPanel.alpha = 0f;
            CriAudioManager.Instance.Resume(CriAudioType.CueSheet_BGM, _bgmGuid);
        }

        private async UniTask StartCountDown()
        {
            // 既存のカウントダウンがあればキャンセル
            _cts?.Cancel();
            _cts = new CancellationTokenSource();

            try
            {
                // 60秒カウントダウン
                await UniTask.Delay(TimeSpan.FromSeconds(_demoVideoDelayTime), cancellationToken: _cts.Token);

                // カウントダウン終了後にVideo再生のコールバックを呼び出す
                _onDemoMoviePlaySubject.OnNext(Unit.Default);

                // Videoが再生終了するまで待機
                await UniTask.WaitUntil(() => !_isDemoMoviePlayingProperty.Value, cancellationToken: _cts.Token);

                // 再度カウントダウンを開始
                await StartCountDown();
            }
            catch (OperationCanceledException)
            {
                // カウントダウンがキャンセルされた場合は何もしない
            }
        }

        public void ResetCountDown()
        {
            // 動画が再生中なら停止処理を行う
            if (_isDemoMoviePlayingProperty.Value)
            {
                _isDemoMoviePlayingProperty.Value = false; // 再生フラグを下ろす
                _cts?.Cancel(); // カウントダウンをキャンセルして、再生のリトリガーを防ぐ
                //Debug.Log("動画再生中にカウントダウンがリセットされました");
                StartCountDown().Forget();
                //Debug.Log("新しくカウントダウンを開始しました");
            }
            else
            {
                // 動画が再生されていない場合にのみカウントダウンを再開
                StartCountDown().Forget();
                //Debug.Log("カウントダウンがリセットされました");
            }
        }
    }
}