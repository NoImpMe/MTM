using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class HpBarView : MonoBehaviour
    {
        [SerializeField] private Image fillImage;
        [SerializeField] private RectTransform rectTransform;

        private Transform followTarget;
        private Vector3 worldOffset = new Vector3(0f, 1.2f, 0f);
        private Camera mainCamera;

        private void Awake()
        {
            mainCamera = Camera.main;
        }

        public void SetFollowTarget(Transform target)
        {
            followTarget = target;
        }

        public void Bind(Unit unit)
        {
            unit.HpChanged += HandleHpChanged;
        }

        public void Unbind(Unit unit)
        {
            unit.HpChanged -= HandleHpChanged;
        }

        private void HandleHpChanged(float current, float max)
        {
            fillImage.fillAmount = Mathf.Clamp01(current / max);
        }

        private void LateUpdate()
        {
            if (followTarget == null) return;

            Vector3 screenPos = mainCamera.WorldToScreenPoint(followTarget.position + worldOffset);
            rectTransform.position = screenPos;
        }
    }
}