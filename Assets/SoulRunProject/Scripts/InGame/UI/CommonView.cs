using SoulRunProject.Common;
using UnityEngine;
using UnityEngine.UI;

namespace SoulRunProject.InGame
{
    /// <summary>
    /// インゲームで常時表示されるUIを管理するクラス
    /// ・体力、経験値、レベル、所持スキル、スコア、コイン、ソウルゲージの操作をする
    /// </summary>
    public class CommonView : MonoBehaviour
    {
        [SerializeField] private Image _hpGauge;
        [SerializeField] private Image _expGauge;
        [SerializeField] private Image _soulGauge;
        [SerializeField] private Text _levelText;
        [SerializeField] private Text _levelStackText;
        [SerializeField] private ItemIcon[] _skillIcons;
        [SerializeField] private ItemIcon[] _passiveItemIcons;
        [SerializeField] private Text _scoreText;
        [SerializeField] private Text _coinText;

        public void SetHpGauge(float value, float maxValue)
        {
            _hpGauge.fillAmount = value / maxValue;
        }
        
        public void SetExpGauge(float value, float maxValue)
        {
            _expGauge.fillAmount = value / maxValue;
        }
        
        public void SetSoulGauge(float value, float maxValue)
        {
            _soulGauge.fillAmount = value / maxValue;
        }
        
        public void SetLevelText(int level)
        {
            _levelText.text = $"{level}";
        }
        
        public void SetSkillIcon(AbstractSkillData getSkillData)
        {
            foreach (var itemIcon in _skillIcons)
            {
                // 前から空白か同じアイテムならそこの表示を更新する
                if (itemIcon.ItemName == "" || itemIcon.ItemName == getSkillData.SkillName)
                {
                    itemIcon.GetItem(getSkillData.SkillName, getSkillData.SkillIcon);
                    return;
                }
            }
        }

        public void SetPassiveItemIcon(StatusUpItem item)
        {
            foreach (var itemIcon in _passiveItemIcons)
            {
                // 前から空白か同じアイテムならそこの表示を更新する
                if (itemIcon.ItemName == "" || itemIcon.ItemName == item.ItemName)
                {
                    itemIcon.GetItem(item.ItemName, item.ItemIcon);
                    return;
                }
            }
        }
        
        public void SetScoreText(int score)
        {
            _scoreText.text = $"SCORE:{score}";
        }
        
        public void SetCoinText(int coin)
        {
            _coinText.text = $"{coin}";
        }
    }
}
