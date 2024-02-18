using UnityEngine;

namespace RPG.Combat {
    public class Health : MonoBehaviour {
        [SerializeField] private float health = 100f;

        public void TakeDamage(float damage) {
            if (health <= 0) return;

            // To avoid the health going below 0
            health = Mathf.Max(health - damage, 0);
            // Debug.Log($"Took damage! Health is now {health}");
        }
    }
}

