using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace SoulRunProject.Common
{
    [CreateAssetMenu(fileName = "ScoreData", menuName = "SoulRunProject/ScoreData")]
    public class ScoreData : ScriptableObject
    {
        [SerializeField] private List<ScoreDatum> _scoreList;

        public Sprite GetSprite(int scoreValue)
        {
            foreach (var data in _scoreList)
            {
                if (scoreValue >= data.LowerThreshold && scoreValue <= data.UpperThreshold)
                {
                    return data.RankSprite;
                }
            }

            return null;
        }
    }
    [Serializable]
    public struct ScoreDatum
    {
        [SerializeField, CustomLabel("上限値")] private int _upperThreshold;
        [SerializeField, CustomLabel("下限値")] private int _lowerThreshold;
        [SerializeField, CustomLabel("ランク画像")] private Sprite _rankSprite;
        public int UpperThreshold => _upperThreshold;
        public int LowerThreshold => _lowerThreshold;
        public Sprite RankSprite => _rankSprite;
    }
}