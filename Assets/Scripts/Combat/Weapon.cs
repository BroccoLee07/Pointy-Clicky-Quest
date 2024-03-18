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

        public float Damage { get => damage; }
        public float Range { get => range; }

        public void Spawn(Transform leftHandTransform, Transform rightHandTransform, Animator animator) {
            // Check for animator first since weapon can be null if unarmed
            if (animator == null || animatorOverride == null) return;
            // Override character animation with appropriate animation for the weapon
            animator.runtimeAnimatorController = animatorOverride;

            // If unarmed, nothing to instantiate
            if (equippedWeaponPrefab == null || (rightHandTransform == null && leftHandTransform == null)) return;
            Transform handTransform;

            if (isRightHanded) {
                handTransform = rightHandTransform;
            } else {
                handTransform = leftHandTransform;
            }

            Instantiate(equippedWeaponPrefab, handTransform);   
        }
    }
}