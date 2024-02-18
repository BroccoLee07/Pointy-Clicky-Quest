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
            ProcessCombat();
            ProcessMovement();
        }

        private void ProcessCombat() {
            RaycastHit[] raycastHits = Physics.RaycastAll(GetMouseInputRay());

            foreach (RaycastHit hit in raycastHits) {                
                CombatTarget target = hit.collider.GetComponent<CombatTarget>();

                if (target == null) continue;

                if (Input.GetMouseButtonDown(0)) {
                    characterCombat.Attack(target);
                }
            }
        }

        private static Ray GetMouseInputRay() {
            return Camera.main.ScreenPointToRay(Input.mousePosition);
        }

        private void ProcessMovement() {
            if (Input.GetMouseButton(0)) {
                MoveToCursor();
            }
        }

        private void MoveToCursor() {
            RaycastHit raycastHit;

            // Check if Physics Raycast has a hit
            if (Physics.Raycast(GetMouseInputRay(), out raycastHit)) {
                characterMovement.MoveTo(raycastHit.point);
            }
        }
    }
}