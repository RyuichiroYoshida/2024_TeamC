using SoulRunProject.Common;
using UniRx;

namespace SoulRunProject
{
    public class ScoreManager : AbstractSingletonMonoBehaviour<ScoreManager>
    {
        protected override bool UseDontDestroyOnLoad { get; } = false;
        private readonly IntReactiveProperty _score = new (0);
        public IReadOnlyReactiveProperty<int> OnScoreChanged => _score;

        public void AddScore(int score)
        {
            _score.Value += score;
        }
    }
}
