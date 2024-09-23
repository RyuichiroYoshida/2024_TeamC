using UnityEngine;
using UnityEngine.UI;

namespace SoulRunProject.InGame
{
    public class ItemIcon : MonoBehaviour
    {
        [SerializeField] private Image _iconImage;
        [SerializeField] private Text _itemLevelText;

        private int _level;

        public string ItemName { get; private set; } = "";

        public void GetItem(string itemName, Sprite icon)
        {
            if (_level == 0)
            {
                ItemName = itemName;
                _iconImage.sprite = icon;
            }

            _level++;
            _itemLevelText.text = $"{_level}";
        }
    }
}
