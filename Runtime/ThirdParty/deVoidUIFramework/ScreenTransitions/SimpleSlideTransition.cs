using System;
using System.Collections;
using UnityEngine;

namespace deVoid.UIFramework
{
    [CreateAssetMenu(menuName = "deVoid UI/Transitions/Simple Slide")]
    public class SimpleSlideTransition : ATransitionComponent
    {
        public enum SlideDirection { Left, Right, Up, Down }

        [SerializeField] private float transitionDuration = 0.5f;
        [SerializeField] private AnimationCurve slideCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
        [SerializeField] private SlideDirection direction = SlideDirection.Right;
        [Space]
        [Tooltip("If true, slides out from center. If false, slides in to center.")]
        [SerializeField] private bool slideOut = false;
        [Tooltip("How far it should slide (in pixels). Example: parent width/height.")]
        [SerializeField] private float slideDistance = 1920f;

        public override void Animate(Transform target, Action callWhenFinished) {
            var runner = target.GetComponent<MonoBehaviour>();
            if (runner != null) {
                runner.StartCoroutine(SlideRoutine(target as RectTransform, callWhenFinished));
            } else {
                callWhenFinished?.Invoke();
            }
        }

        private IEnumerator SlideRoutine(RectTransform target, Action callWhenFinished) {
            if (target == null) {
                callWhenFinished?.Invoke();
                yield break;
            }

            Vector2 offset = Vector2.zero;
            switch (direction) {
                case SlideDirection.Left: offset = new Vector2(-slideDistance, 0); break;
                case SlideDirection.Right: offset = new Vector2(slideDistance, 0); break;
                case SlideDirection.Up: offset = new Vector2(0, slideDistance); break;
                case SlideDirection.Down: offset = new Vector2(0, -slideDistance); break;
            }

            Vector2 startPos = slideOut ? Vector2.zero : offset;
            Vector2 endPos = slideOut ? offset : Vector2.zero;
            float timer = 0f;

            target.anchoredPosition = startPos;

            while (timer < transitionDuration) {
                timer += Time.unscaledDeltaTime;
                float progress = Mathf.Clamp01(timer / transitionDuration);
                float curveValue = slideCurve.Evaluate(progress);
                
                target.anchoredPosition = Vector2.LerpUnclamped(startPos, endPos, curveValue);
                yield return null;
            }

            target.anchoredPosition = endPos;
            callWhenFinished?.Invoke();
        }
    }
}
