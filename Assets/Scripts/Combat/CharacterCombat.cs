using UnityEngine;
using RPG.Movement;
using RPG.Core;
using System;

namespace RPG.Combat {
    [RequireComponent(typeof(CharacterMovement))]
    [RequireComponent(typeof(ActionScheduler))]
    [RequireComponent(typeof(Animator))]
    public class CharacterCombat : MonoBehaviour, IAction {

        [SerializeField] private float weaponRange = 2f;
        [SerializeField] private float timeBetweenAttacks = 1f;
        [SerializeField] private float weaponDamage = 5f;
        // Set to null for both variables related to weapons for the case where character is unarmed
        [SerializeField] private GameObject weaponPrefab = null;
        [SerializeField] private Transform handTransform = null;
        [SerializeField] private AnimatorOverrideController weaponOverride = null;

        // Dependency
        private CharacterMovement characterMovement;
        private ActionScheduler actionScheduler;
        private Animator animator;
        private Health targetHealth;

        private float timeSinceLastAttack = Mathf.Infinity;

        private const string ANIMATOR_ATTACK_TRIGGER = "attack";
        private const string ANIMATOR_STOP_ATTACK_TRIGGER = "stopAttack";
        void Start() {
            characterMovement = GetComponent<CharacterMovement>();
            actionScheduler = GetComponent<ActionScheduler>();
            animator = GetComponent<Animator>();

            SpawnWeapon();
        }        

        void Update() {
            // Will keep increasing when no attack
            timeSinceLastAttack += Time.deltaTime;

            if (targetHealth == null) return;
            if (targetHealth.IsDead) return;

            // Handle movement towards any existing combat target
            if (!IsInAttackRange()) {
                characterMovement.MoveTo(targetHealth.transform.position, 1f);
            } else {
                characterMovement.Cancel();

                AttackBehaviour();
            }
        }

        private void SpawnWeapon() {
            if (weaponPrefab == null || handTransform == null) return;
            
            Instantiate(weaponPrefab, handTransform);
            // Override character animation with appropriate animation for the weapon
            animator.runtimeAnimatorController = weaponOverride;
        }

        private void AttackBehaviour() {
            if (timeSinceLastAttack <= timeBetweenAttacks) return;

            transform.LookAt(targetHealth.transform);
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
            targetHealth?.TakeDamage(weaponDamage);
        }

        private bool IsInAttackRange() {
            float charToTargetDistance = Vector3.Distance(transform.position, targetHealth.transform.position);
            // Debug.Log($"charToTargetDistance: {charToTargetDistance}");

            if (charToTargetDistance > weaponRange) {
                return false;
            } else {
                return true;
            }
        }

        public bool CanAttack(GameObject target) {
            if (target == null) return false;

            // Debug.Log($"target dead? {target.GetHealth().IsDead}");
            Health targetHealth = target.GetComponent<Health>();
            return targetHealth != null && !targetHealth.IsDead;
        }

        public void Attack(GameObject combatTarget) {
            Debug.Log($"Fighter is attacking!");
            actionScheduler.StartAction(this);
            targetHealth = combatTarget.GetComponent<Health>();            
        }

        public void Cancel() {
            StopAttack();
            targetHealth = null;
            GetComponent<CharacterMovement>().Cancel();
        }

        private void StopAttack() {
            animator.ResetTrigger(ANIMATOR_ATTACK_TRIGGER);
            animator.SetTrigger(ANIMATOR_STOP_ATTACK_TRIGGER);
        }
    }
}

