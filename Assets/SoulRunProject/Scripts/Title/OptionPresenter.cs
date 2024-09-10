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
            // オプション画面を開く
            _optionView.DisplayOption();

            // リジュームボタンが押された時、ポーズ解除してオプション画面を閉じる
            _optionView.ResumeButton.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    _optionView.CloseOption(); // ポップアップを閉じる
                    _titleModel.Return(_optionView.gameObject); // タイトル画面に戻す
                })
                .AddTo(this);

            // エグジットボタンが押された時、ポップアップを閉じてタイトルに戻る
            _optionView.ExitButton.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    _optionView.CloseOption(); // ポップアップを閉じる
                    _titleModel.Return(_optionView.gameObject); // タイトル画面に戻す
                })
                .AddTo(this);
        }
    }
}