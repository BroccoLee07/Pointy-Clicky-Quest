using UnityEngine;
using RPG.Movement;
using RPG.Combat;
using RPG.Stats;
using UnityEngine.AI;
using RPG.Attributes;
using System;
using UnityEngine.EventSystems;

namespace RPG.Control {
    [RequireComponent(typeof(CharacterMovement))]
    [RequireComponent(typeof(CharacterCombat))]
    [RequireComponent(typeof(Health))]
    public class PlayerController : MonoBehaviour {
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
            if (ProcessUI()) return;

            // If player is dead, cursor should seem like it cannot interact with anything but the UI
            if (health.IsDead) {
                SetCursor(CursorType.None);
                return;
            }

            if (ProcessInteractable()) return;
            if (ProcessMovement()) return;

            SetCursor(CursorType.None);
        }

        private bool ProcessInteractable() {
            RaycastHit[] raycastHits = Physics.RaycastAll(GetMouseInputRay());
            foreach (RaycastHit hit in raycastHits) {
                IRaycastable[] raycastables = hit.transform.GetComponents<IRaycastable>();
                foreach (IRaycastable raycastable in raycastables) {
                    if (raycastable.HandleRaycast(this)) {
                        SetCursor(raycastable.GetCursorType());
                        return true;
                    }
                }
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

        private bool ProcessUI() {
            // Check if mouse is over UI defined by EventSystem
            if (EventSystem.current.IsPointerOverGameObject()) {
                SetCursor(CursorType.UI);
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