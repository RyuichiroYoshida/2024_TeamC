using CriWare;

namespace SoulRunProject.Audio
{
    public class BGMPlayer : CriAudioPlayerService
    {
        private CriPlayerData? _currentBGM;

        public BGMPlayer(string cueSheetName, CriAtomListener listener)
            : base(cueSheetName, listener)
        {
        }

        public override void Play(string cueName, float volume, bool isLoop)
        {
            // 既存のBGMを停止する
            StopAllBGM();
            // 新しいBGMを再生する
            base.Play(cueName, volume, isLoop);
        }

        private void StopAllBGM()
        {
            if (_currentBGM.HasValue && _currentBGM.Value.Playback.GetStatus() == CriAtomExPlayback.Status.Playing)
            {
                _currentBGM.Value.Player.Stop();
                _currentBGM.Value.Player.Dispose();
                _currentBGM = null;
            }
        }

        protected override void OnPlayerCreated(CriPlayerData playerData)
        {
            _currentBGM = playerData;
        }
    }
}