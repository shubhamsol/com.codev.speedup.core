using deVoid.UIFramework;
using UnityEngine;

namespace Speedup.UI
{
    [AddComponentMenu("CodeV/UI/UI Transition On Enable")]
    [DisallowMultipleComponent]
    public class UITransitionOnEnable : MonoBehaviour
    {
        [SerializeField] private ATransitionComponent _transition;
        [SerializeField] private RectTransform _target;
        [SerializeField] private bool _playOnEnable = true;
        [SerializeField] private bool _playOnlyOnce;

        private bool _hasPlayed;

        private void Reset()
        {
            _target = GetComponent<RectTransform>();
        }

        private void OnEnable()
        {
            if (_playOnEnable)
            {
                Play();
            }
        }

        public void Play()
        {
            if (_transition == null)
            {
                Debug.LogWarning("[UITransitionOnEnable] Transition is not assigned.", this);
                return;
            }

            if (_playOnlyOnce && _hasPlayed)
            {
                return;
            }

            Transform targetTransform = _target != null ? _target : transform;
            _transition.Animate(targetTransform, null);
            _hasPlayed = true;
        }

        public void ResetPlayedState()
        {
            _hasPlayed = false;
        }
    }
}
