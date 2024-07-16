using System;
using CriWare;
using UnityEngine;

namespace SoulRunProject.Audio
{
    public interface ICriAudioPlayerService : IDisposable
    {
        void Play(string cueName, float volume = 1f, bool isLoop = false);
        void Play3D(Transform transform, string cueName, float volume = 1f, bool isLoop = false);
        void Stop(string cueName);
        void Pause(string cueName);
        void Resume(string cueName);
        void SetVolume(float volume);
        void StopAll();
        void PauseAll();
        void ResumeAll();
    }
}