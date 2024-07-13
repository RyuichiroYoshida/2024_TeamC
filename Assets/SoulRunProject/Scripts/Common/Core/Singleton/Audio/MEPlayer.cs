using CriWare;

namespace SoulRunProject.Audio
{
    public class MEPlayer : CriAudioPlayerService
    {
        private CriPlayerData? _currentMe;
        public MEPlayer(string cueSheetName, CriAtomListener listener)
            : base(cueSheetName, listener)
        {
        }

        protected override void OnPlayerCreated(CriPlayerData playerData)
        {
            _currentMe = playerData;
        }
    }
}