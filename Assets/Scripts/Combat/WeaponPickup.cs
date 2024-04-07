using System.Collections;
using RPG.Attributes;
using RPG.Control;
using RPG.Movement;
using UnityEngine;

namespace RPG.Combat {
    
    public class WeaponPickup : MonoBehaviour, IRaycastable {
        [SerializeField] private WeaponConfig weapon;
        [SerializeField] private float healthToRestore = 0;
        [Tooltip("Main child component of weapon pickup that contains the renderer but not the WeaponPickup script")]
        [SerializeField] private GameObject pickupChild;
        [SerializeField] private float respawnTime = 5f;
        [SerializeField] private float pickupRange = 5f;

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

        public bool CanMoveTowardsPickup(PlayerController playerController) {
            return playerController.GetComponent<CharacterMovement>().CanMoveTo(transform.position);
        }

        private bool IsWithinPickupRange(PlayerController playerController) {
            float distToPickup = Vector3.Distance(playerController.transform.position, transform.position);
            // Debug.Log($"Player's dist to pickup: {distToPickup}; pickupRange: {pickupRange}");
            return distToPickup <= pickupRange;
        }

        public bool HandleRaycast(PlayerController playerController) {
            if (IsWithinPickupRange(playerController) && Input.GetMouseButtonDown(0)) {
                Pickup(playerController.gameObject);
            } else if (!IsWithinPickupRange(playerController) 
            && CanMoveTowardsPickup(playerController)
            && Input.GetMouseButtonDown(0)) {
                // Similar logic to combat, if not in range then walk towards target
                playerController.GetComponent<CharacterMovement>().MoveTo(transform.position, 1f);
            }
            
            // As long as the mouse is hovering and raycast can be handled, return true
            return true;
        }

        public CursorType GetCursorType() {
            return CursorType.Pickup;
        }
    }
}
