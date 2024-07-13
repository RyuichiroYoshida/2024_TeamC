using CriWare;

namespace SoulRunProject.Audio
{
    public struct CriPlayerData
    {
        public CriAtomExPlayer Player { get; set; }
        public CriAtomExPlayback Playback { get; set; }
        public CriAtomEx.CueInfo CueInfo { get; set; }
        public bool IsLoop { get; set; }

        public CriPlayerData(CriAtomExPlayer player, CriAtomExPlayback playback, CriAtomEx.CueInfo cueInfo, bool isLoop)
        {
            Player = player;
            Playback = playback;
            CueInfo = cueInfo;
            IsLoop = isLoop;
        }
    }
}