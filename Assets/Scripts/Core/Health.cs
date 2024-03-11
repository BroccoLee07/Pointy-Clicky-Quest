using Newtonsoft.Json.Linq;
using RPG.Saving;
using UnityEngine;

namespace RPG.Core {
    [RequireComponent(typeof(ActionScheduler))]
    [RequireComponent(typeof(Animator))]
    public class Health : MonoBehaviour, IJsonSaveable {
        [SerializeField] private float healthPoints = 100f;

        private bool isDead = false;

        private const string ANIMATOR_DIE_TRIGGER = "die";

        public bool IsDead { get => isDead; }

        public void TakeDamage(float damage) {
            if (IsDead) return;

            // To avoid the health going below 0
            healthPoints = Mathf.Max(healthPoints - damage, 0);
            // Debug.Log($"Took damage! Health is now {health}");

            UpdateHealthState();
        }

        private void UpdateHealthState() {
            if (healthPoints <= 0) {
                isDead = true;
                Die();                
            }
        }

        private void Die() {
            GetComponent<Animator>().SetTrigger(ANIMATOR_DIE_TRIGGER);
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }

        public object CaptureState() {
            return healthPoints;
        }

        public void RestoreState(object state) {
            this.healthPoints = (float)state;

            UpdateHealthState();
        }

        public JToken CaptureAsJToken() {
            return JToken.FromObject(healthPoints);
        }

        public void RestoreFromJToken(JToken state) {
            healthPoints = state.ToObject<float>();
            UpdateHealthState();
        }

    }
}

