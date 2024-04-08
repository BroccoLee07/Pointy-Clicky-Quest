using UnityEngine;
using RPG.Movement;
using RPG.Core;
using RPG.Saving;
using Newtonsoft.Json.Linq;
using RPG.Attributes;
using RPG.Stats;
using System.Collections.Generic;
using GameDevTV.Utils;

namespace RPG.Combat {
    [RequireComponent(typeof(CharacterMovement))]
    [RequireComponent(typeof(Health))]
    [RequireComponent(typeof(ActionScheduler))]
    [RequireComponent(typeof(Animator))]
    public class CharacterCombat : MonoBehaviour, IAction, IJsonSaveable, IModifierProvider {

        [SerializeField] private float timeBetweenAttacks = 1f;
        // Set to null for variables related to weapons for the case where character is unarmed
        [SerializeField] private Transform leftHandTransform = null;
        [SerializeField] private Transform rightHandTransform = null;
        [SerializeField] private WeaponConfig defaultWeapon = null;

        // Dependency
        private CharacterMovement characterMovement;
        private ActionScheduler actionScheduler;
        private Animator animator;
        private BaseStats baseStats;
        private Health targetHealth;
        private Health characterHealth;
        private WeaponConfig currentWeaponConfig;
        private LazyValue<Weapon> currentWeapon;


        private float timeSinceLastAttack = Mathf.Infinity;

        private const string ANIMATOR_ATTACK_TRIGGER = "attack";
        private const string ANIMATOR_STOP_ATTACK_TRIGGER = "stopAttack";

        // Properties
        public WeaponConfig CurrentWeaponConfig { get => currentWeaponConfig; }

        void Awake() {
            characterMovement = GetComponent<CharacterMovement>();
            characterHealth = GetComponent<Health>();
            actionScheduler = GetComponent<ActionScheduler>();
            animator = GetComponent<Animator>();
            baseStats = GetComponent<BaseStats>();
            currentWeaponConfig = defaultWeapon;
            currentWeapon = new LazyValue<Weapon>(SetupDefaultWeapon);
        }

        void Start() {
            // currentWeaponConfig.ForceInit();
            currentWeapon.ForceInit();
        }        

        void Update() {
            if (targetHealth == null) return;
            if (targetHealth.IsDead || characterHealth.IsDead) return;

            // Will keep increasing when no attack
            timeSinceLastAttack += Time.deltaTime;

            // Handle movement towards any existing combat target
            if (!IsInAttackRange(targetHealth.transform)) {
                characterMovement.MoveTo(targetHealth.transform.position, 1f);
            } else {
                characterMovement.Cancel();

                AttackBehaviour();
            }
        }

        private Weapon SetupDefaultWeapon() {
            Weapon weapon = AttachWeapon(defaultWeapon);
            return weapon;
        }

        public void EquipWeapon(WeaponConfig weapon) {
            if (weapon == null) return;

            Debug.Log($"Equipping new weapon: {weapon.WeaponName}");
            currentWeaponConfig = weapon;
            currentWeapon.value = AttachWeapon(weapon);
        }

        private Weapon AttachWeapon(WeaponConfig weapon) {
            return weapon.Spawn(leftHandTransform, rightHandTransform, animator);
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

            // Make sure there is an equipped weapon
            if (currentWeapon.value != null) {
                currentWeapon.value.OnHit();
            }

            if (currentWeaponConfig.HasProjectile) {
                currentWeaponConfig.LaunchProjectile(gameObject, leftHandTransform, rightHandTransform, targetHealth, damage);
            } else {
                targetHealth?.TakeDamage(gameObject, damage);
            }
        }

        // Handle attack animation event called Shoot (bow)
        void Shoot() {
            Hit();
        }

        private bool IsInAttackRange(Transform targetTransform) {
            float charToTargetDistance = Vector3.Distance(transform.position, targetTransform.transform.position);

            if (charToTargetDistance > currentWeaponConfig.Range) {
                return false;
            } else {
                return true;
            }
        }

        public bool CanAttack(GameObject target) {
            if (target == null) return false;
            // If there is no path or target is too far
            if (!characterMovement.CanMoveTo(target.transform.position)
            && !IsInAttackRange(target.transform)) {
                return false;
            }

            Health targetHealth = target.GetComponent<Health>();
            return targetHealth != null && !targetHealth.IsDead;
        }

        public void Attack(GameObject combatTarget) {
            actionScheduler.StartAction(this);
            targetHealth = combatTarget.GetComponent<Health>();            
        }

        public void Cancel() {
            StopAttack();
            targetHealth = null;
            GetComponent<CharacterMovement>().Cancel();
        }

        public JToken CaptureAsJToken() {
            return JToken.FromObject(currentWeaponConfig.name);
        }

        public void RestoreFromJToken(JToken state) {
            string weaponName = state.ToObject<string>();
            WeaponConfig weapon = Resources.Load<WeaponConfig>(weaponName);
            EquipWeapon(weapon);
        }

        public IEnumerable<float> GetAdditiveModifiers(Stat stat) {
            if (stat == Stat.Damage) {
                yield return currentWeaponConfig.Damage;
            }
        }

        public IEnumerable<float> GetPercentageModifiers(Stat stat) {
            if (stat == Stat.Damage) {
                yield return currentWeaponConfig.PercentageBonusDamage;
            }              
        }

        private void StopAttack() {
            animator.ResetTrigger(ANIMATOR_ATTACK_TRIGGER);
            animator.SetTrigger(ANIMATOR_STOP_ATTACK_TRIGGER);
        }
    }
}

