using HikanyanLaboratory.SceneManagement;
using SoulRunProject.Audio;
using SoulRunProject.Framework;
using UnityEngine;
using SoulRunProject.InGame;

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
        private void Start()
        {
            CriAudioManager.Instance.Play(CriAudioType.CueSheet_BGM, "BGM_Title", true);
        }

        public async void StartGame()
        {
            DebugClass.Instance.ShowLog($"ゲーム開始:{_transitionTime}秒後にインゲーム画面に遷移します");
            //ここで実行
            await SceneManager.Instance.LoadSceneWithFade(_tutorialScene);
            //  アクションマップをプレイヤーに変更する。
            PlayerInputManager.Instance.SwitchActionMap(PlayerInputManager.ActionMapType.Player);
        }

        public void OpenOption(CanvasGroup optionPanel, CanvasGroup basePanel)
        {
            basePanel.interactable = false;
            basePanel.blocksRaycasts = false;
            optionPanel.interactable = true;
            optionPanel.blocksRaycasts = true;
            optionPanel.alpha = 1f;
        }

        public void CloseOption(CanvasGroup optionPanel, CanvasGroup basePanel)
        {
            basePanel.interactable = true;
            basePanel.blocksRaycasts = true;
            optionPanel.interactable = false;
            optionPanel.blocksRaycasts = false;
            optionPanel.alpha = 0f;
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
    }
}