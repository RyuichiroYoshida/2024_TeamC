using SoulRunProject.InGame;
using UnityEngine;
using UnityEngine.UI;

namespace SoulRunProject
{
    public class StageProgressView : MonoBehaviour
    {
        [SerializeField] private Image _progressBar;
        [SerializeField] private StageManager _stageManager;
        [SerializeField] private float progress;
        
        private int maxIndex = 0;
        private int currentIndex = 0;

        private void Start()
        {
            maxIndex = _stageManager.StageData[0].FieldPatterns.Count - 2;
            _stageManager.ToBossStage += () =>
            {
                _progressBar.fillAmount = 1;
                currentIndex = maxIndex;
                Destroy(this);
            };
        }

        // Update is called once per frame
        void Update()
        {
            currentIndex = _stageManager.FieldPatternIndex;
            if (maxIndex == currentIndex)
            {
                Destroy(this);
            }
            _progressBar.fillAmount = (float)(currentIndex - 1) / maxIndex;
        }
    }
}
