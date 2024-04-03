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
        [SerializeField] private float maxNavMeshProjectionDistance = 1f;
        [SerializeField] private float maxNavPathLength = 40f;
        
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
            RaycastHit[] raycastHits = RaycastAllSorted();
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

        RaycastHit[] RaycastAllSorted() {
            RaycastHit[] hits = Physics.RaycastAll(GetMouseInputRay());
            // Build array of hits by distance
            float[] distances = new float[hits.Length];

            for (int i = 0; i < hits.Length; i++) {
                distances[i] = hits[i].distance;
            }

            // Sort distances of hits in ascending order
            Array.Sort(distances);

            // Sort hits by distance from screen
            Array.Sort(distances, hits);
            return Physics.RaycastAll(GetMouseInputRay());
        }

        private static Ray GetMouseInputRay() {
            return Camera.main.ScreenPointToRay(Input.mousePosition);
        }

        private bool ProcessMovement() {
            Vector3 targetPos;
            if (RaycastNavMesh(out targetPos)) {
                if (Input.GetMouseButton(0)) {
                    characterMovement.StartMoveAction(targetPos, 1f);
                }

                // Handles mouse hovers
                SetCursor(CursorType.Movement);
                return true;
            }

            return false;
        }

        private bool RaycastNavMesh(out Vector3 targetPos) {
            targetPos = Vector3.zero;

            RaycastHit raycastHit;
            bool hasHit = Physics.Raycast(GetMouseInputRay(), out raycastHit);

            if (!hasHit) return false;

            NavMeshHit navMeshHit;
            bool hasCastToNavMesh = NavMesh.SamplePosition(raycastHit.point, out navMeshHit, maxNavMeshProjectionDistance, NavMesh.AllAreas);
            if (!hasCastToNavMesh) return false;

            // Set target pos to sampled position
            targetPos = navMeshHit.position;

            // Calculating path so as to not allow paths that are too far for the player
            NavMeshPath path = new NavMeshPath();
            bool hasPath = NavMesh.CalculatePath(transform.position, targetPos, NavMesh.AllAreas, path);
            // If there is no path or the path is incomplete/invalid, then moving to pos is not allowed
            if (!hasPath || (path.status != NavMeshPathStatus.PathComplete)) return false;
            if (GetPathLength(path) > maxNavPathLength) return false;

            return true;
        }

        private float GetPathLength(NavMeshPath path) {
            float total = 0;
            if (path.corners.Length < 2) return total;

            for (int i = 0; i < path.corners.Length - 1; i++) {
                total += Vector3.Distance(path.corners[i], path.corners[i + 1]);
            }
            
            return total;
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