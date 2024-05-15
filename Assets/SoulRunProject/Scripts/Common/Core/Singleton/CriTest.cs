using System.Collections;
using System.Collections.Generic;
using SoulRunProject.Common;
using UnityEngine;

namespace SoulRunProject
{
    public class CriTest : MonoBehaviour
    {
        [SerializeField] private CriAudioManager.CueSheet cueSheet;
        [SerializeField] private string cueName;
        [SerializeField] private int _spinIndex = -1;

        void Start()
        {
            CriAudioManager.Instance.PlaySE(cueSheet, cueName);
            // _spinIndex = CriAudioManager.Instance.PlaySE(CriAudioManager.CueSheet.Se, "SE_Spin");
            // CriAudioManager.Instance.PauseSE(_spinIndex);
            // CriAudioManager.Instance.MasterVolume = 0.5f;
        }
    }
}