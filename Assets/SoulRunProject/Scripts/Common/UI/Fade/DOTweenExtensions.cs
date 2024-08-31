// using Cysharp.Threading.Tasks;
// using DG.Tweening;
//
// public static class DOTweenExtensions
// {
//     public static UniTask ToUniTask(this Tween tween, bool autoKill = true, TweenCallback onKilled = null)
//     {
//         var completionSource = new UniTaskCompletionSource();
//
//         tween.OnComplete(() => completionSource.TrySetResult());
//         tween.OnKill(() =>
//         {
//             if (autoKill)
//             {
//                 completionSource.TrySetResult();
//             }
//             onKilled?.Invoke();
//         });
//
//         return completionSource.Task;
//     }
// }