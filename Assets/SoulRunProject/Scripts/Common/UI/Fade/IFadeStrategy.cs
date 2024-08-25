using Cysharp.Threading.Tasks;
using UnityEngine;

namespace HikanyanLaboratory.Fade
{
    public interface IFadeStrategy
    {
        UniTask FadeOut(Material fadeMaterial);
        UniTask FadeIn(Material fadeMaterial);
    }
}