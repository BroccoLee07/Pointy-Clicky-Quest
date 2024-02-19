using UnityEngine;
using RPG.Movement;
using RPG.Core;

namespace RPG.Combat {
    public class CharacterCombat : MonoBehaviour, IAction {

        [SerializeField] private float weaponRange = 2f;
        [SerializeField] private float timeBetweenAttacks = 1f;
        [SerializeField] private float weaponDamage = 5f;

        // Dependency
        private CharacterMovement characterMovement;
        private ActionScheduler actionScheduler;
        private Animator animator;

        private Transform targetTransform;
        private Health targetHealth;
        private float timeSinceLastAttack = 0;

        private const string ANIMATOR_ATTACK_TRIGGER = "attack";
        public void Initialize() {
            characterMovement = GetComponent<CharacterMovement>();
            actionScheduler = GetComponent<ActionScheduler>();
            animator = GetComponent<Animator>();
        }

        void Update() {
            // Will keep increasing when no attack
            timeSinceLastAttack += Time.deltaTime;

            if (targetTransform == null) return;

            // Handle movement towards any existing combat target
            if (!IsInAttackRange()) {
                characterMovement.MoveTo(targetTransform.transform.position);
            } else {
                characterMovement.Cancel();

                AttackBehaviour();
            }
        }

        private void AttackBehaviour() {
            if (timeSinceLastAttack <= timeBetweenAttacks) return;

            // This will trigger the Hit() animation event
            animator.SetTrigger(ANIMATOR_ATTACK_TRIGGER);
            timeSinceLastAttack = 0;            
        }

        // Handle attack animation event Hit
        void Hit() {
            GetTargetHealth()?.TakeDamage(weaponDamage);
        }

        private bool IsInAttackRange() {
            float charToTargetDistance = Vector3.Distance(transform.position, targetTransform.transform.position);
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
            targetTransform = combatTarget.transform;            
        }

        public void Cancel() {
            targetTransform = null;
        }

        public Health GetTargetHealth() {
            return targetTransform?.GetComponent<Health>();
        }
    }
}

