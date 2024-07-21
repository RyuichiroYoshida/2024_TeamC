using UniRx;

namespace SoulRunProject.Audio
{
    public interface ICriVolume
    {
        IReactiveProperty<float> Volume { get; }
        void SetVolume(float volume);
    }
}