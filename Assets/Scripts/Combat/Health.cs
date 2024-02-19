using UnityEngine;

namespace RPG.Combat {
    public class Health : MonoBehaviour {
        [SerializeField] private float healthPoints = 100f;

        private bool isDead = false;
        // private Animator animator;

        private const string ANIMATOR_DIE_TRIGGER = "die";

        public bool IsDead { get => isDead; }

        // public void Initialize() {
        //     animator = GetComponent<Animator>();
        // }
        public void TakeDamage(float damage) {
            if (IsDead) return;

            // To avoid the health going below 0
            healthPoints = Mathf.Max(healthPoints - damage, 0);
            // Debug.Log($"Took damage! Health is now {health}");

            if (healthPoints <= 0) {
                isDead = true;
                Die();                
            }
        }

        private void Die() {
            GetComponent<Animator>().SetTrigger(ANIMATOR_DIE_TRIGGER);
        }
    }
}

