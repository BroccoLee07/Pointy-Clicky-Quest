using UnityEngine;
using RPG.Attributes;
using RPG.Control;

namespace RPG.Combat {
    [RequireComponent(typeof(Health))]
    public class CombatTarget : MonoBehaviour, IRaycastable {
        public Health GetHealth() {
            return GetComponent<Health>();
        }

        public bool HandleRaycast(PlayerController playerController) {
            CharacterCombat characterCombat = playerController.GetComponent<CharacterCombat>();

            if (!characterCombat.CanAttack(gameObject)) {
                return false;
            }

            if (Input.GetMouseButton(0)) {
                characterCombat.Attack(gameObject);
            }

            return true;

            // Handles mouse hovers
            // SetCursor(CursorType.Combat);
        }
    }
}

