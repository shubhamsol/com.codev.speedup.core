using System;
using System.Collections;
using UnityEngine;

namespace deVoid.UIFramework
{
    [CreateAssetMenu(menuName = "deVoid UI/Transitions/Bounce Scale")]
    public class BounceScaleTransition : ATransitionComponent
    {
        public enum BounceType { Back, Elastic }

        [SerializeField] private float transitionDuration = 0.5f;
        [SerializeField] private BounceType bounceType = BounceType.Back;
        
        [Tooltip("Used for Back. How much it overshoots. 1.7 is a good default.")]
        [SerializeField] private float overshoot = 1.70158f;

        [Space]
        [Tooltip("If true, scales from 1 to 0 (anticipates then shrinks). If false, scales from 0 to 1 (overshoots then settles).")]
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
                
                float curveValue = EvaluateEase(progress);
                
                target.localScale = Vector3.LerpUnclamped(startScale, endScale, curveValue);
                yield return null;
            }

            target.localScale = endScale;
            callWhenFinished?.Invoke();
        }

        private float EvaluateEase(float t) {
            if (scaleOut) {
                // EaseIn (Anticipate then shrink)
                if (bounceType == BounceType.Back) {
                    float c1 = overshoot;
                    float c3 = c1 + 1f;
                    return c3 * t * t * t - c1 * t * t;
                } else { // Elastic
                    if (t == 0f) return 0f;
                    if (t == 1f) return 1f;
                    float c4 = (2f * Mathf.PI) / 3f;
                    return -Mathf.Pow(2f, 10f * t - 10f) * Mathf.Sin((t * 10f - 10.75f) * c4);
                }
            } else {
                // EaseOut (Overshoot then settle)
                if (bounceType == BounceType.Back) {
                    float c1 = overshoot;
                    float c3 = c1 + 1f;
                    return 1f + c3 * Mathf.Pow(t - 1f, 3) + c1 * Mathf.Pow(t - 1f, 2);
                } else { // Elastic
                    if (t == 0f) return 0f;
                    if (t == 1f) return 1f;
                    float c4 = (2f * Mathf.PI) / 3f;
                    return Mathf.Pow(2f, -10f * t) * Mathf.Sin((t * 10f - 0.75f) * c4) + 1f;
                }
            }
        }
    }
}
