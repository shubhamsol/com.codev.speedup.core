using System;
using System.Collections;
using UnityEngine;

namespace deVoid.UIFramework
{
    [CreateAssetMenu(menuName = "deVoid UI/Transitions/Simple Scale")]
    public class SimpleScaleTransition : ATransitionComponent
    {
        [SerializeField] private float transitionDuration = 0.5f;
        [SerializeField] private AnimationCurve scaleCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
        [Space]
        [Tooltip("If true, scales from 1 to 0. If false, scales from 0 to 1.")]
        [SerializeField] private bool scaleOut = false;

        public override void Animate(Transform target, Action callWhenFinished) {
            var runner = target.GetComponent<MonoBehaviour>();
            if (runner != null) {
                runner.StartCoroutine(ScaleRoutine(target, callWhenFinished));
            } else {
                callWhenFinished?.Invoke();
            }
        }

        private IEnumerator ScaleRoutine(Transform target, Action callWhenFinished) {
            Vector3 startScale = scaleOut ? Vector3.one : Vector3.zero;
            Vector3 endScale = scaleOut ? Vector3.zero : Vector3.one;
            float timer = 0f;

            target.localScale = startScale;

            while (timer < transitionDuration) {
                timer += Time.unscaledDeltaTime;
                float progress = Mathf.Clamp01(timer / transitionDuration);
                float curveValue = scaleCurve.Evaluate(progress);
                
                target.localScale = Vector3.LerpUnclamped(startScale, endScale, curveValue);
                yield return null;
            }

            target.localScale = endScale;
            callWhenFinished?.Invoke();
        }
    }
}
