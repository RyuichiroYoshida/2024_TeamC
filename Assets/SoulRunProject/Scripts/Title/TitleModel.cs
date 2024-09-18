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
        //[SerializeReference, SubclassSelector] IFadeStrategy _fadeStrategy;
        [SerializeField] private OptionModel _optionModel;

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

        public void Option(GameObject optionPanel)
        {
            bool isActive = optionPanel.activeSelf;
            optionPanel.SetActive(!isActive);
            _optionModel.SetPausedState(isActive);
            DebugClass.Instance.ShowLog("オプション画面表示");
        }

        public void Return(GameObject optionPanel)
        {
            optionPanel.SetActive(false);
            _optionModel.SetPausedState(false);
            DebugClass.Instance.ShowLog("タイトル画面に戻る");
            
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