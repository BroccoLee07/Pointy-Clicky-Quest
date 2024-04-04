using RPG.Attributes;
using UnityEngine;

namespace RPG.UI {
    public class HealthBar : MonoBehaviour {

        [SerializeField] private Health health;
        [SerializeField] private RectTransform foregroundTransform;
        [SerializeField] private Canvas healthBarCanvas;

        void Start() {
            // Make sure health bar is hidden in the beginning
            Show(false);
        }

        void Update() {
            // If health bar already reflects the health in fraction, do nothing
            if (Mathf.Approximately(foregroundTransform.localScale.x, health.GetFraction())) return;

            // If taking damage or loading health that is not the same as the current health bar value
            // and health bar is not yet visible, show health bar
            if (!IsVisible()) {
                Show(true);
            }

            UpdateHealthBar();

            // If target is dead, hide health bar
            if (health.IsDead) {
                Show(false);
            }
        }

        private void UpdateHealthBar() {
            // Set x value of local scale to update foreground of health bar
            foregroundTransform.localScale = new Vector3(health.GetFraction(), foregroundTransform.localScale.y, foregroundTransform.localScale.z);
        }

        private void Show(bool isVisible) {
            healthBarCanvas.enabled = isVisible;
        }

        private bool IsVisible() {
            return healthBarCanvas.enabled;
        }
    }
}
