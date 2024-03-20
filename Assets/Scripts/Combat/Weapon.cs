using System.Collections;
using RPG.Core;
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


        // Properties
        public float Damage { get => damage; }
        public float Range { get => range; }
        public bool HasProjectile { get => projectile != null; }

        public void Spawn(Transform leftHandTransform, Transform rightHandTransform, Animator animator) {
            // Check for animator first since weapon can be null if unarmed
            if (animator == null || animatorOverride == null) return;
            // Override character animation with appropriate animation for the weapon
            animator.runtimeAnimatorController = animatorOverride;

            // If unarmed, nothing to instantiate
            if (equippedWeaponPrefab == null || (rightHandTransform == null && leftHandTransform == null)) return;
            Transform handTransform = GetHandTransform(leftHandTransform, rightHandTransform);

            Instantiate(equippedWeaponPrefab, handTransform);
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