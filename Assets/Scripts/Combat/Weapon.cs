using RPG.Attributes;
using UnityEngine;

namespace RPG.Combat {
    [CreateAssetMenu(fileName = "Weapon", menuName = "Weapons/Make New Weapon", order = 0)]
    public class Weapon : ScriptableObject {        
        [SerializeField] private float damage = 5f;
        [SerializeField] private float range = 2f;
        // Can be null for unarmed
        [SerializeField] private GameObject equippedWeaponPrefab = null;
        [SerializeField] private AnimatorOverrideController animatorOverride = null;
        [SerializeField] private bool isRightHanded = true;
        // Can be null if weapon doesn't use Projectiles
        [SerializeField] private Projectile projectile = null;

        public const string weaponName = "Weapon";

        // Properties
        public float Damage { get => damage; }
        public float Range { get => range; }
        public bool HasProjectile { get => projectile != null; }

        public void Spawn(Transform leftHandTransform, Transform rightHandTransform, Animator animator) {
            Debug.Log($"Spawning weapon");
            DestroyOldWeapon(leftHandTransform, rightHandTransform);

            // Check if weapon to spawn has an equipped look (unarmed and fireball has no equipment)
            if (equippedWeaponPrefab != null) {
                Transform handTransform = GetHandTransform(leftHandTransform, rightHandTransform);
                GameObject weapon = Instantiate(equippedWeaponPrefab, handTransform);
                weapon.name = weaponName;
            }

            // Make sure characer's animator exists
            if (animator == null) return;
            
            AnimatorOverrideController animatorOverrideController = animator.runtimeAnimatorController as AnimatorOverrideController;
            // If animatorOverride is set, update animation with appropriate weapon's animation
            if (animatorOverride != null) {
                animator.runtimeAnimatorController = animatorOverride;
            // If animatorOverride is set (for unarmed or if default should be used)
            } else if (animatorOverrideController != null) {
                // Set current animator to the default stored in the runtimeAnimatorController
                animator.runtimeAnimatorController = animatorOverrideController.runtimeAnimatorController;                
            }
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

        public void LaunchProjectile(Transform leftHandTransform, Transform rightHandTransform, Health targetHealth) {
            Projectile newProjectile = Instantiate(projectile, GetHandTransform(leftHandTransform, rightHandTransform).position, Quaternion.identity);
            newProjectile.Initialize(targetHealth, damage);
        }
    }
}