using RPG.Attributes;
using UnityEngine;

namespace RPG.UI {
    public class HealthBar : MonoBehaviour {

        [SerializeField] private Health health;
        [SerializeField] private RectTransform foregroundTransform;

        void Update() {
            if (foregroundTransform.localScale.x == health.GetFraction()) return;

            UpdateHealthBar();
        }

        private void UpdateHealthBar() {
            // Set x value of local scale to update foreground of health bar
            foregroundTransform.localScale = new Vector3(health.GetFraction(), foregroundTransform.localScale.y, foregroundTransform.localScale.z);
        }
    }
}
