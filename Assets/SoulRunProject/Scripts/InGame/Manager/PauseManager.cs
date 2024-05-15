using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using SoulRunProject.InGame;
using UnityEngine;

namespace SoulRunProject.Common
{
    public class PauseManager : AbstractSingletonMonoBehaviour<PauseManager>
    {
        protected override bool UseDontDestroyOnLoad { get; } = true;
        private static List<IPausable> _pausables = new();
        private bool _isPause;

        public void RegisterPausableObject(IPausable pausable)
        {
            _pausables.Add(pausable);
        }
        public void UnRegisterPausableObject(IPausable pausable)
        {
            _pausables.Remove(pausable);
        }

        public void Pause(bool isPause)
        {
            foreach (var pausable in _pausables)
            {
                pausable.Pause(isPause);
            }

            _isPause = isPause;
        }
        /// <summary>
        /// ポーズに対応した待機処理、キャンセルされるとfalseを返す
        /// </summary>
        /// <returns>キャンセルされるとfalse, 設定した時間を超えるとtrue</returns>
        public async UniTask<bool> TryWaitForSeconds(float seconds)
        {
            var timer = 0f;
            //  タイマーループ開始
            while (true)
                try
                {
                    //  Pauseフラグがfalseなら1フレーム待つ(ちゃんと1フレーム待ってるか分からない)、
                    //  trueならfalseになるまで通さない
                    await UniTask.WaitUntil(() => !_isPause, PlayerLoopTiming.Update, destroyCancellationToken);
                    timer += Time.deltaTime;
                    if (timer >= seconds)
                    {
                        return true;
                    }
                }
                catch
                {
                    Debug.Log("キャンセルされた");
                    return false;
                }
        }
    }
}