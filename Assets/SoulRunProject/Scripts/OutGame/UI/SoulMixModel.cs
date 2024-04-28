﻿using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;

namespace SoulRunProject.SoulMixScene
{
    public class SoulMixModel : MonoBehaviour
    {
        [SerializeField] private SoulCombiner _soulCombiner;
        public ReactiveCollection<SoulCardMasterData> OwnedCards = new ReactiveCollection<SoulCardMasterData>();
        private SoulCardManager _soulCardManager;

        // ログメッセージを発行するためのReactiveProperty
        public ReactiveProperty<string> LogMessage = new ReactiveProperty<string>();

        private void Start()
        {
            _soulCardManager = SoulCardManager.Instance;
        }

        public async UniTaskVoid SoulMixAsync()
        {
            // // 選択されたソウルカードのリストが2つ以上であるか確認
            // if (_soulCombiner.SoulCardMasterDataList.Count < 2)
            // {
            //     LogMessage.Value = "ソウルカードを2つ以上選択してください。";
            //     return;
            // }
            //
            // // 選択された最初のソウルカードを取得
            // var selectedSoul1 = _soulCombiner.SoulCardMasterDataList[0];
            SoulCardMasterData selectedSoul1 = null;
            // 組み合わせ可能なソウルカードを探す
            var combinableSoul = _soulCombiner.SearchCombinableSoul(selectedSoul1);

            if (combinableSoul != null)
            {
                var resultSoul = _soulCombiner.combinations
                    .Find(c => c.IsValidCombination(selectedSoul1, combinableSoul))
                    .Result;

                LogMessage.Value = $"ソウルカード「{selectedSoul1.SoulName}」と" +
                                   $"組み合わせ可能なソウルカード「{combinableSoul.SoulName}」が見つかりました。";
                LogMessage.Value = $"組み合わせた後のソウルカード：{resultSoul.SoulName}";
                LogMessage.Value = "合成しますか？Y/N";

                if (await WaitForKeyDown(KeyCode.Y))
                {
                    LogMessage.Value = "合成します";
                    var newSoul = _soulCombiner.Combine(selectedSoul1, combinableSoul);
                    _soulCardManager.RemoveSoulCard(selectedSoul1);
                    _soulCardManager.RemoveSoulCard(combinableSoul);
                    _soulCardManager.AddSoulCard(newSoul);
                    LogMessage.Value = $"新しいソウルカード「{newSoul.SoulName}」を作成しました";
                }
                else if (await WaitForKeyDown(KeyCode.N))
                {
                    LogMessage.Value = "合成しません";
                }
            }
            else
            {
                LogMessage.Value = "組み合わせ可能なソウルカードが見つかりませんでした。";
            }

            //レベルアップ処理
            //経験値の取得
            //経験値の計算
        }

        private static async UniTask<bool> WaitForKeyDown(KeyCode keyCode)
        {
            while (!Input.GetKeyDown(keyCode))
            {
                await UniTask.Yield();
            }

            return true;
        }
    }
}