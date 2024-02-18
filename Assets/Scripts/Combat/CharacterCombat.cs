using UnityEngine;
using RPG.Movement;

namespace RPG.Combat {
    public class CharacterCombat : MonoBehaviour {

        [SerializeField] float weaponRange = 2f;

        // Dependency
        private CharacterMovement characterMovement;

        private Transform combatTargetTransform;
        public void Initialize() {
            characterMovement = GetComponent<CharacterMovement>();
        }

        void Update() {
            if (combatTargetTransform != null) {
                float charToTargetDistance = Vector3.Distance(transform.position, combatTargetTransform.transform.position);
                Debug.Log($"charToTargetDistance: {charToTargetDistance}");
                if (charToTargetDistance > weaponRange) {
                    characterMovement.MoveTo(combatTargetTransform.transform.position);
                } else {
                    characterMovement.Stop();
                    combatTargetTransform = null;
                }
            }
        }

        public void Attack(CombatTarget combatTarget) {
            Debug.Log($"Fighter is attacking!");
            combatTargetTransform = combatTarget.transform;
            
        }
    }
}

