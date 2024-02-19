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

        // private Transform targetTransform;
        private CombatTarget target;
        private float timeSinceLastAttack = 0;

        private const string ANIMATOR_ATTACK_TRIGGER = "attack";
        private const string ANIMATOR_STOP_ATTACK_TRIGGER = "stopAttack";
        public void Initialize() {
            characterMovement = GetComponent<CharacterMovement>();
            actionScheduler = GetComponent<ActionScheduler>();
            animator = GetComponent<Animator>();
        }

        void Update() {
            // Will keep increasing when no attack
            timeSinceLastAttack += Time.deltaTime;

            if (target == null) return;
            if (target.GetHealth().IsDead) return;

            // Handle movement towards any existing combat target
            if (!IsInAttackRange()) {
                characterMovement.MoveTo(target.transform.position);
            } else {
                characterMovement.Cancel();

                AttackBehaviour();
            }
        }

        private void AttackBehaviour()
        {
            if (timeSinceLastAttack <= timeBetweenAttacks) return;

            transform.LookAt(target.transform);
            TriggerAttack();
            timeSinceLastAttack = 0;
        }

        private void TriggerAttack() {
            animator.ResetTrigger(ANIMATOR_STOP_ATTACK_TRIGGER);
            // This will trigger the Hit() animation event
            animator.SetTrigger(ANIMATOR_ATTACK_TRIGGER);
        }

        // Handle attack animation event Hit
        void Hit() {
            target?.GetHealth()?.TakeDamage(weaponDamage);
        }

        private bool IsInAttackRange() {
            float charToTargetDistance = Vector3.Distance(transform.position, target.transform.position);
            // Debug.Log($"charToTargetDistance: {charToTargetDistance}");

            if (charToTargetDistance > weaponRange) {
                return false;
            } else {
                return true;
            }
        }

        public bool CanAttack(CombatTarget target) {
            if (target == null) return false;

            Debug.Log($"target dead? {target.GetHealth().IsDead}");
            return target != null && !target.GetHealth().IsDead;
        }

        public void Attack(CombatTarget combatTarget) {
            Debug.Log($"Fighter is attacking!");
            actionScheduler.StartAction(this);
            target = combatTarget;            
        }

        public void Cancel() {
            StopAttack();
            target = null;
        }

        private void StopAttack() {
            animator.ResetTrigger(ANIMATOR_ATTACK_TRIGGER);
            animator.SetTrigger(ANIMATOR_STOP_ATTACK_TRIGGER);
        }
    }
}

