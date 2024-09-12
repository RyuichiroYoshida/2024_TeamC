using UniRx;
using UnityEngine;

namespace SoulRunProject.Title
{
    public class OptionModel : MonoBehaviour
    {
        private readonly ReactiveProperty<bool> _isPaused = new(false);

        public IReadOnlyReactiveProperty<bool> IsPaused => _isPaused;

        public void SetPausedState(bool isPaused)
        {
            _isPaused.Value = isPaused;
        }
    }
}