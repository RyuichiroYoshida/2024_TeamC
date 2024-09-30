using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using HikanyanLaboratory.Fade;
using HikanyanLaboratory.SceneManagement;
using SoulRun.InGame;
using UnityEngine;
using UniRx;

namespace SoulRunProject
{
    public class SceneChangeTest : MonoBehaviour
    {
        [SerializeField] private string _sceneName;
        [SerializeField] private TweenButton _button;

        void Start()
        {
            _button.OnClick.Subscribe(_ => ChangeScene()).AddTo(this);
        }

        private async void ChangeScene()
        {
            await SceneManager.Instance.LoadSceneWithFade(_sceneName);
        }
    }
}