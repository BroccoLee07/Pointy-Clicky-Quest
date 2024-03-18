using UnityEngine;

namespace RPG.Combat {
    [CreateAssetMenu(fileName = "Weapon", menuName = "Weapons/Make New Weapon", order = 0)]
    public class Weapon : ScriptableObject {
        [SerializeField] private GameObject weaponPrefab = null;
        [SerializeField] private AnimatorOverrideController animatorOverride = null;

        public void Spawn(Transform handTransform, Animator animator) {
            if (weaponPrefab == null || handTransform == null || animator == null) return;
            
            Instantiate(weaponPrefab, handTransform);
            // Override character animation with appropriate animation for the weapon
            animator.runtimeAnimatorController = animatorOverride;
        }
        
    }
}