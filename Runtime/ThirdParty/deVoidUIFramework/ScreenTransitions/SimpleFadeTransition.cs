using System;
using System.Collections;
using UnityEngine;

namespace deVoid.UIFramework
{
    /// <summary>
    /// This is a simple fade transition implemented as a built-in example.
    /// I recommend using a free tweening library like DOTween (http://dotween.demigiant.com/)
    /// or rolling out your own.
    /// Check the Examples project for more robust and battle-tested options:
    /// https://github.com/yankooliveira/uiframework_examples
    /// </summary>
    [CreateAssetMenu(menuName = "deVoid UI/Transitions/Simple Fade")]
    public class SimpleFadeTransition : ATransitionComponent
    {
        [SerializeField] private float fadeDuration = 0.5f;
        [SerializeField] private bool fadeOut = false;

        public override void Animate(Transform target, Action callWhenFinished) {
            var runner = target.GetComponent<MonoBehaviour>();
            if (runner != null) {
                runner.StartCoroutine(FadeRoutine(target, callWhenFinished));
            } else {
                callWhenFinished?.Invoke();
            }
        }

        private IEnumerator FadeRoutine(Transform target, Action callWhenFinished) {
            CanvasGroup canvasGroup = target.GetComponent<CanvasGroup>();
            if (canvasGroup == null) {
                canvasGroup = target.gameObject.AddComponent<CanvasGroup>();
            }

            float startValue = fadeOut ? 1f : 0f;
            float endValue = fadeOut ? 0f : 1f;
            float timer = fadeDuration;

            canvasGroup.alpha = startValue;

            while (timer > 0f) {
                timer -= Time.unscaledDeltaTime;
                canvasGroup.alpha = Mathf.Lerp(endValue, startValue, timer / fadeDuration);
                yield return null;
            }

            canvasGroup.alpha = endValue;
            callWhenFinished?.Invoke();
        }
    }
}