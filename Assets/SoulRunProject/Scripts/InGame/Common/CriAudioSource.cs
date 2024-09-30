using SoulRunProject.Audio;
using UnityEngine;

namespace SoulRunProject
{
    public class CriAudioSource : MonoBehaviour
    {
        public void PlaySE(string cueName)
        {
            CriAudioManager.Instance.Play(CriAudioType.CueSheet_SE, cueName);
        }
    }
}