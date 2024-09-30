// using System;
// using Cysharp.Threading.Tasks;
// using DG.Tweening;
// using HikanyanLaboratory.Fade;
// using SoulRunProject.Common;
// using UnityEngine;
// using UnityEngine.UI;
//
// namespace GameJamProject.SceneManagement
// {
//     public class FadeView : AbstractSingletonMonoBehaviour<FadeView>
//     {
//         [SerializeField, Tooltip("フェード用のImageを設定")]
//         private Material _fadeMaterial;
//
//         [SerializeField, Tooltip("Fade用テクスチャ")]
//         private Texture _maskTexture;
//
//
//         [SerializeField, Tooltip("フェード時間を設定")] private float _fadeDuration = 1f;
//
//         [SerializeField, Tooltip("フェードのイージングを設定")]
//         private Ease _fadeEase = Ease.Linear;
//
//         private static readonly int MaskTex = Shader.PropertyToID("_MaskTex");
//         protected override bool UseDontDestroyOnLoad => true;
//
//         public override void OnAwake()
//         {
//             var fadeManager = FadeController.Instance;
//
//             if (_fadeMaterial == null) return;
//             fadeManager.SetFadeMaterial(_fadeMaterial);
//             if (_maskTexture != null)
//             {
//                 _fadeMaterial.SetTexture(MaskTex, _maskTexture);
//             }
//         }
//     }
// }