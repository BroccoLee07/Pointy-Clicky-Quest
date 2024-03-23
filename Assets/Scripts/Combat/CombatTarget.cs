using UnityEngine;
using RPG.Attributes;

namespace RPG.Combat {
    [RequireComponent(typeof(Health))]
    public class CombatTarget : MonoBehaviour {
        public Health GetHealth() {
            return GetComponent<Health>();
        }
    }
}

