using UnityEngine;
using RPG.Movement;
using RPG.Combat;
using RPG.Stats;
using UnityEngine.AI;
using RPG.Attributes;

namespace RPG.Control {
    [RequireComponent(typeof(CharacterMovement))]
    [RequireComponent(typeof(CharacterCombat))]
    [RequireComponent(typeof(Health))]
    public class PlayerController : MonoBehaviour {
        private CharacterMovement characterMovement;
        private CharacterCombat characterCombat;
        private Health health;

        // Initialize dependencies here
        // Note: Might be good to use Zenject or some dependency injection framework to handle these
        void Start() {
            characterMovement = gameObject.GetComponent<CharacterMovement>();
            characterCombat = gameObject.GetComponent<CharacterCombat>();
            health = GetComponent<Health>();
            
            GetComponent<NavMeshAgent>().enabled = true;
        }

        void Update() {
            if (health.IsDead) return;
            
            if (ProcessCombat()) return;
            if (ProcessMovement()) return;

            // Debug.Log("Nothing to do.");
        }

        private bool ProcessCombat() {
            RaycastHit[] raycastHits = Physics.RaycastAll(GetMouseInputRay());

            foreach (RaycastHit hit in raycastHits) {                
                CombatTarget target = hit.collider.GetComponent<CombatTarget>();

                if (target == null) continue;

                if (!characterCombat.CanAttack(target.gameObject)) continue;

                if (Input.GetMouseButton(0)) {
                    characterCombat.Attack(target.gameObject);                    
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
                    characterMovement.StartMoveAction(raycastHit.point, 1f);
                }

                // Mouse could hover over terrain
                return true;
            }

            return false;
        }
    }
}