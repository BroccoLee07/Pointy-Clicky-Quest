using RPG.Attributes;
using UnityEngine;

namespace RPG.Combat {
    [CreateAssetMenu(fileName = "Weapon", menuName = "Weapons/Make New Weapon", order = 0)]
    public class WeaponConfig : ScriptableObject {        
        [SerializeField] private float damage = 5f;
        [SerializeField] private float percentageBonusDamage = 0f;
        [SerializeField] private float range = 2f;
        // Can be null for unarmed
        [SerializeField] private Weapon equippedWeaponPrefab = null;
        [SerializeField] private AnimatorOverrideController animatorOverride = null;
        [SerializeField] private bool isRightHanded = true;
        // Can be null if weapon doesn't use Projectiles
        [SerializeField] private Projectile projectile = null;

        public const string weaponName = "Weapon";

        // Properties
        public float Damage { get => damage; }
        public float PercentageBonusDamage { get => percentageBonusDamage; }
        public float Range { get => range; }
        public bool HasProjectile { get => projectile != null; }

        public Weapon Spawn(Transform leftHandTransform, Transform rightHandTransform, Animator animator) {
            DestroyOldWeapon(leftHandTransform, rightHandTransform);

            Weapon weapon = null;
            // Check if weapon to spawn has an equipped look (unarmed and fireball has no equipment)
            if (equippedWeaponPrefab != null) {
                Transform handTransform = GetHandTransform(leftHandTransform, rightHandTransform);
                weapon = Instantiate(equippedWeaponPrefab, handTransform);
                weapon.gameObject.name = weaponName;
            }
            
            AnimatorOverrideController animatorOverrideController = animator.runtimeAnimatorController as AnimatorOverrideController;
            // If animatorOverride is set, update animation with appropriate weapon's animation
            if (animatorOverride != null) {
                animator.runtimeAnimatorController = animatorOverride;
            // If animatorOverride is set (for unarmed or if default should be used)
            } else if (animatorOverrideController != null) {
                // Set current animator to the default stored in the runtimeAnimatorController
                animator.runtimeAnimatorController = animatorOverrideController.runtimeAnimatorController;                
            }

            return weapon;
        }

        private void DestroyOldWeapon(Transform leftHandTransform, Transform rightHandTransform) {
            Transform oldWeapon = rightHandTransform.Find(weaponName);
            if (oldWeapon == null) {
                oldWeapon = leftHandTransform.Find(weaponName);
                // No weapon found so nothing to destroy
                if (oldWeapon == null) return;
            }

            // Rename first to make sure new weapon is not destroyed
            oldWeapon.name = "OldWeapon";
            Destroy(oldWeapon.gameObject);
        }

        private Transform GetHandTransform(Transform leftHandTransform, Transform rightHandTransform) {
            Transform handTransform;
            if (isRightHanded) {
                handTransform = rightHandTransform;
            } else {
                handTransform = leftHandTransform;
            }

            return handTransform;
        }

        public void LaunchProjectile(
            GameObject attackInitiator, 
            Transform leftHandTransform, 
            Transform rightHandTransform, 
            Health targetHealth, 
            float calculatedDamage) {

            Projectile newProjectile = Instantiate(projectile, GetHandTransform(leftHandTransform, rightHandTransform).position, Quaternion.identity);
            newProjectile.Initialize(attackInitiator, targetHealth, calculatedDamage);
        }
    }
}