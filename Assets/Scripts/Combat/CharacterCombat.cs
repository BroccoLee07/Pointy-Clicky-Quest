using UnityEngine;
using RPG.Movement;
using RPG.Core;

namespace RPG.Combat {
    public class CharacterCombat : MonoBehaviour, IAction {

        [SerializeField] private float weaponRange = 2f;
        [SerializeField] private float timeBetweenAttacks = 1f;

        // Dependency
        private CharacterMovement characterMovement;
        private ActionScheduler actionScheduler;
        private Animator animator;

        private Transform combatTargetTransform;
        private float timeSinceLastAttack = 0;
        public void Initialize() {
            characterMovement = GetComponent<CharacterMovement>();
            actionScheduler = GetComponent<ActionScheduler>();
            animator = GetComponent<Animator>();
        }

        void Update() {
            // Will keep increasing when no attack
            timeSinceLastAttack += Time.deltaTime;

            if (combatTargetTransform == null) return;

            // Handle movement towards any existing combat target
            if (!IsInAttackRange()) {
                characterMovement.MoveTo(combatTargetTransform.transform.position);
            } else {
                characterMovement.Cancel();

                AttackBehaviour();
            }
        }

        private void AttackBehaviour() {
            if (timeSinceLastAttack <= timeBetweenAttacks) return;

            animator.SetTrigger("attack");
            timeSinceLastAttack = 0;

            // TODO: Apply damage here
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

        // Handle attack animation event Hit
        void Hit() {

        }
    }
}

