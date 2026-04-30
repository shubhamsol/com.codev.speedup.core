using System;
using System.Collections;
using UnityEngine;

namespace deVoid.UIFramework.Examples
{
    [CreateAssetMenu(menuName = "deVoid UI/Transitions/Legacy Animation")]
    public class LegacyAnimationScreenTransition : ATransitionComponent
    {
        [SerializeField] private AnimationClip clip = null;
        [SerializeField] private bool playReverse = false;
        
        public override void Animate(Transform target, Action callWhenFinished) {
            var runner = target.GetComponent<MonoBehaviour>();
            if (runner == null) {
                callWhenFinished?.Invoke();
                return;
            }

            var targetAnimation = target.GetComponent<Animation>();
            if (targetAnimation == null) {
                Debug.LogError("[LegacyAnimationScreenTransition] No Animation component in " + target);
                callWhenFinished?.Invoke();
                return;
            }

            targetAnimation.clip = clip;
            runner.StartCoroutine(PlayAnimationRoutine(targetAnimation, callWhenFinished));
        }

        private IEnumerator PlayAnimationRoutine(Animation targetAnimation, Action callWhenFinished) {
            foreach (AnimationState state in targetAnimation) {
                state.time = playReverse ? state.clip.length : 0f;
                state.speed = playReverse ? -1f : 1f;
            }

            targetAnimation.Play(PlayMode.StopAll);
            yield return new WaitForSeconds(targetAnimation.clip.length);
            callWhenFinished?.Invoke();
        }
    }
}
