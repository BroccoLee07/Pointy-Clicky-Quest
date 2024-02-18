using UnityEngine;
using RPG.Movement;
using RPG.Core;

namespace RPG.Combat {
    public class CharacterCombat : MonoBehaviour, IAction {

        [SerializeField] float weaponRange = 2f;

        // Dependency
        private CharacterMovement characterMovement;
        private ActionScheduler actionScheduler;

        private Transform combatTargetTransform;
        public void Initialize() {
            characterMovement = GetComponent<CharacterMovement>();
            actionScheduler = GetComponent<ActionScheduler>();
        }

        void Update() {
            if (combatTargetTransform == null) return;

            // Handle movement towards any existing combat target
            if (!IsInAttackRange()) {
                characterMovement.MoveTo(combatTargetTransform.transform.position);
            } else {
                characterMovement.Cancel();
                Cancel();
            }
        }

        private bool IsInAttackRange() {
            float charToTargetDistance = Vector3.Distance(transform.position, combatTargetTransform.transform.position);
            // Debug.Log($"charToTargetDistance: {charToTargetDistance}");

            if (charToTargetDistance > weaponRange) {
                return false;
            } else {
                return true;
            }
        }

        public void Attack(CombatTarget combatTarget) {
            Debug.Log($"Fighter is attacking!");
            actionScheduler.StartAction(this);
            combatTargetTransform = combatTarget.transform;            
        }

        public void Cancel() {
            combatTargetTransform = null;
        }
    }
}

