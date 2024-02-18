using UnityEngine;
using RPG.Movement;
using RPG.Combat;

namespace RPG.Control {
    public class PlayerController : MonoBehaviour {
        private CharacterMovement characterMovement;
        private CharacterCombat characterCombat;

        // Initialize dependencies here
        // Note: Might be good to use Zenject or some dependency injection framework to handle these
        void Start() {
            characterMovement = gameObject.GetComponent<CharacterMovement>();
            characterCombat = gameObject.GetComponent<CharacterCombat>();

            characterMovement.Initialize();
            characterCombat.Initialize();
        }

        void Update() {
            if (ProcessCombat()) return;
            if (ProcessMovement()) return;

            Debug.Log("Nothing to do.");
        }

        private bool ProcessCombat() {
            RaycastHit[] raycastHits = Physics.RaycastAll(GetMouseInputRay());

            foreach (RaycastHit hit in raycastHits) {                
                CombatTarget target = hit.collider.GetComponent<CombatTarget>();

                if (target == null) continue;

                if (Input.GetMouseButtonDown(0)) {
                    characterCombat.Attack(target);                    
                }

                // Return here since we are still handling/executing the ProcessCombat
                // This includes hovering on a target and not clicking yet
                return true;
            }

            return false;
        }

        private static Ray GetMouseInputRay() {
            return Camera.main.ScreenPointToRay(Input.mousePosition);
        }

        private bool ProcessMovement() {
            RaycastHit raycastHit;

            // Check if Physics Raycast has a hit
            if (Physics.Raycast(GetMouseInputRay(), out raycastHit)) {
                if (Input.GetMouseButton(0)) {
                    characterMovement.StartMoveAction(raycastHit.point);
                }

                // Mouse could hover over terrain
                return true;
            }

            return false;
        }
    }
}