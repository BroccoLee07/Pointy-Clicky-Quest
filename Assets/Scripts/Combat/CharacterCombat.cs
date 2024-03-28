using UnityEngine;
using RPG.Movement;
using RPG.Core;
using RPG.Saving;
using Newtonsoft.Json.Linq;
using RPG.Attributes;
using RPG.Stats;
using System.Collections.Generic;

namespace RPG.Combat {
    [RequireComponent(typeof(CharacterMovement))]
    [RequireComponent(typeof(ActionScheduler))]
    [RequireComponent(typeof(Animator))]
    public class CharacterCombat : MonoBehaviour, IAction, IJsonSaveable, IModifierProvider {

        [SerializeField] private float timeBetweenAttacks = 1f;
        // Set to null for variables related to weapons for the case where character is unarmed
        [SerializeField] private Transform leftHandTransform = null;
        [SerializeField] private Transform rightHandTransform = null;
        [SerializeField] private Weapon defaultWeapon = null;

        // Dependency
        private CharacterMovement characterMovement;
        private ActionScheduler actionScheduler;
        private Animator animator;
        private BaseStats baseStats;
        private Health targetHealth;
        private Weapon currentWeapon;

        private float timeSinceLastAttack = Mathf.Infinity;

        private const string ANIMATOR_ATTACK_TRIGGER = "attack";
        private const string ANIMATOR_STOP_ATTACK_TRIGGER = "stopAttack";
        void Start() {
            characterMovement = GetComponent<CharacterMovement>();
            actionScheduler = GetComponent<ActionScheduler>();
            animator = GetComponent<Animator>();
            baseStats = GetComponent<BaseStats>();

            if (currentWeapon == null) {
                EquipWeapon(defaultWeapon);
            }
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

        public void EquipWeapon(Weapon weapon) {
            Debug.Log($"Equipping weapon {weapon.name}");
            if (weapon == null) return;

            currentWeapon = weapon;
            Debug.Log($"Set as current weapon");
            weapon.Spawn(leftHandTransform, rightHandTransform, animator);
        }

        public Health GetTargetHealth() {
            return targetHealth;
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
            if (targetHealth == null) return;

            float damage = baseStats.GetStat(Stat.Damage);
            if (currentWeapon.HasProjectile) {
                // Debug.Log($"Instantiate projectile");
                currentWeapon.LaunchProjectile(gameObject, leftHandTransform, rightHandTransform, targetHealth, damage);
            } else {
                // Debug.Log($"Melee On Hit, take damage");
                targetHealth?.TakeDamage(gameObject, damage);
            }
        }

        // Handle attack animation event called Shoot (bow)
        void Shoot() {
            Hit();
        }

        private bool IsInAttackRange() {
            float charToTargetDistance = Vector3.Distance(transform.position, targetHealth.transform.position);
            // Debug.Log($"charToTargetDistance: {charToTargetDistance}");

            if (charToTargetDistance > currentWeapon.Range) {
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
            // Debug.Log($"Fighter is attacking!");
            actionScheduler.StartAction(this);
            targetHealth = combatTarget.GetComponent<Health>();            
        }

        public void Cancel() {
            StopAttack();
            targetHealth = null;
            GetComponent<CharacterMovement>().Cancel();
        }

        public JToken CaptureAsJToken() {
            return JToken.FromObject(currentWeapon.name);
        }

        public void RestoreFromJToken(JToken state) {
            string weaponName = state.ToObject<string>();
            Weapon weapon = Resources.Load<Weapon>(weaponName);
            EquipWeapon(weapon);
        }

        public IEnumerable<float> GetAdditiveModifiers(Stat stat) {
            if (stat == Stat.Damage) {
                yield return currentWeapon.Damage;
            }
        }

        public IEnumerable<float> GetPercentageModifiers(Stat stat) {
            if (stat == Stat.Damage) {
                yield return currentWeapon.PercentageBonusDamage;
            }              
        }

        private void StopAttack() {
            animator.ResetTrigger(ANIMATOR_ATTACK_TRIGGER);
            animator.SetTrigger(ANIMATOR_STOP_ATTACK_TRIGGER);
        }
    }
}

