using UnityEngine;
using RPG.Movement;
using RPG.Combat;
using RPG.Stats;
using UnityEngine.AI;
using RPG.Attributes;
using System;

namespace RPG.Control {
    [RequireComponent(typeof(CharacterMovement))]
    [RequireComponent(typeof(CharacterCombat))]
    [RequireComponent(typeof(Health))]
    public class PlayerController : MonoBehaviour {
        // Mouse cursor state
        public enum CursorType {
            None,
            Movement,
            Combat
        }

        [Serializable]
        public struct CursorMapping {
            public CursorType cursorType;
            public Texture2D texture;
            public Vector2 hotspot;
        }

        [SerializeField] private CursorMapping[] cursorMappings;
        
        private CharacterMovement characterMovement;
        private CharacterCombat characterCombat;
        private Health health;

        // Initialize dependencies here
        // Note: Might be good to use Zenject or some dependency injection framework to handle these
        void Awake() {
            characterMovement = gameObject.GetComponent<CharacterMovement>();
            characterCombat = gameObject.GetComponent<CharacterCombat>();
            health = GetComponent<Health>();
            
            GetComponent<NavMeshAgent>().enabled = true;
        }

        void Update() {
            if (health.IsDead) return;
            
            if (ProcessCombat()) return;
            if (ProcessMovement()) return;

            SetCursor(CursorType.None);
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

                // Handles mouse hovers
                SetCursor(CursorType.Combat);

                // Return here since we are still handling/executing the ProcessCombat
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

                // Handles mouse hovers
                SetCursor(CursorType.Movement);
                return true;
            }

            return false;
        }

        // Updates what cursor looks like based on what it hovers or the cursor type
        private void SetCursor(CursorType cursorType) {
            CursorMapping cursorMapping = GetCursorMapping(cursorType);
            Cursor.SetCursor(cursorMapping.texture, cursorMapping.hotspot, CursorMode.Auto);
        }

        private CursorMapping GetCursorMapping(CursorType cursorType) {
            foreach (CursorMapping mapping in cursorMappings) {
                if (mapping.cursorType == cursorType) {
                    return mapping;
                }
            }

            return cursorMappings[0];
        }
    }
}