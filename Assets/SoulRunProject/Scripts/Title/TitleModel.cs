using System;
using Cysharp.Threading.Tasks;
using SoulRunProject.Audio;
using SoulRunProject.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using SoulRunProject.Common;

namespace SoulRunProject.Title
{
    /// <summary>
    /// タイトルのロジック処理を行うクラス
    /// </summary>
    public class TitleModel : MonoBehaviour
    {
        [SerializeField] float _transitionTime = 1.0f;

        private void Start()
        {
            CriAudioManager.Instance.Play(CriAudioType.CueSheet_BGM, "BGM_Title", true);
        }

        public async void StartGame()
        {
            DebugClass.Instance.ShowLog($"ゲーム開始:{_transitionTime}秒後にインゲーム画面に遷移します");
            //ここで実行
            SceneManager.LoadScene("TutorialScene");
            //LoadingScene.Instance.LoadNextScene("InGame").Forget();
        }

        public void Option()
        {
            DebugClass.Instance.ShowLog("オプション画面表示");
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