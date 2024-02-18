using UnityEngine;

namespace RPG.Combat {
    public class CharacterCombat : MonoBehaviour {

        public void Initialize() {

        }
        public void Attack(CombatTarget target) {
            Debug.Log($"Fighter is attacking!");
        }
    }
}

