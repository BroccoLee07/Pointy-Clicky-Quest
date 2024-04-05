using System.Collections;
using System.Collections.Generic;
using RPG.Attributes;
using RPG.Control;
using UnityEngine;

namespace RPG.Combat {
    
    public class WeaponPickup : MonoBehaviour, IRaycastable {
        [SerializeField] private WeaponConfig weapon;
        [SerializeField] private float healthToRestore = 0;
        [Tooltip("Main child component of weapon pickup that contains the renderer but not the WeaponPickup script")]
        [SerializeField] private GameObject pickupChild;
        [SerializeField] private float respawnTime = 5f;        

        private Collider pickupCollider;

        void Awake() {
            pickupCollider = GetComponent<Collider>();
        }

        private void OnTriggerEnter(Collider other) {
            if (other.tag == "Player") {
                Pickup(other.gameObject);
            }
        }

        private void Pickup(GameObject pickup) {
            if (weapon != null) {
                pickup.GetComponent<CharacterCombat>().EquipWeapon(weapon);
            }

            if (healthToRestore > 0) {
                pickup.GetComponent<Health>().Heal(healthToRestore);
            }

            StartCoroutine(HideForSeconds(respawnTime));
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

        public bool HandleRaycast(PlayerController playerController) {
            if (Input.GetMouseButtonDown(0)) {
                Pickup(playerController.gameObject);
            }

            // As long as the mouse is hovering and raycast can be handled, return true
            return true;
        }

        public CursorType GetCursorType() {
            return CursorType.Pickup;
        }
    }
}
