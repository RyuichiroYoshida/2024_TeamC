using UniRx;
using UnityEngine;

namespace SoulRunProject.Title
{
    public class OptionPresenter : MonoBehaviour
    {
        [SerializeField] private TitleModel _titleModel;
        [SerializeField] private OptionView _optionView;

        private void Start()
        {
            _optionView.ReturnButton.OnClick.Subscribe(_ => _titleModel.Option(_optionView.OptionPanel));
        }
    }
}