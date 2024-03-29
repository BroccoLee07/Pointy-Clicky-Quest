using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Combat {
    
    public class WeaponPickup : MonoBehaviour {
        [SerializeField] private Weapon weapon;
        [Tooltip("Main child component of weapon pickup that contains the renderer but not the WeaponPickup script")]
        [SerializeField] private GameObject pickupChild;
        [SerializeField] private float respawnTime = 5f;        

        private Collider pickupCollider;

        void Awake() {
            pickupCollider = GetComponent<Collider>();
        }

        private void OnTriggerEnter(Collider other) {
            if (other.tag == "Player") {
                other.GetComponent<CharacterCombat>().EquipWeapon(weapon);
                StartCoroutine(HideForSeconds(respawnTime));
            }
        }

        private IEnumerator HideForSeconds(float seconds) {
            ShowPickup(false);
            yield return new WaitForSeconds(seconds);
            ShowPickup(true);
        }

        private void ShowPickup(bool isVisible) {
            if (pickupChild == null || pickupCollider == null) return;

            pickupCollider.enabled = isVisible;
            pickupChild.SetActive(isVisible);
        }
    }
}
