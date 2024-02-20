using RPG.Combat;
using RPG.Core;
using RPG.Movement;
using UnityEngine;

namespace RPG.Control {
    [RequireComponent(typeof(CharacterMovement))]
    [RequireComponent(typeof(CharacterCombat))]
    [RequireComponent(typeof(ActionScheduler))]
    [RequireComponent(typeof(Health))]    
    public class AIController : MonoBehaviour {
        [SerializeField] private float chaseDistance = 5f;
        [SerializeField] private float suspicionTime = 5f;
        [SerializeField] private PatrolPath patrolPath; // Can be null
        [SerializeField] private float wayPointTolerance = 1f;

        // Dependency
        private CharacterCombat characterCombat;
        private CharacterMovement characterMovement;
        private ActionScheduler actionScheduler;
        private Health health;
        private GameObject player;

        private Vector3 guardPosition;
        private float timeSinceLastDetectedPlayer = Mathf.Infinity;
        private int currentWaypointIndex = 0;
        

        void Start() {
            player = GameObject.FindWithTag("Player");
            characterCombat = GetComponent<CharacterCombat>();
            characterMovement = GetComponent<CharacterMovement>();
            actionScheduler = GetComponent<ActionScheduler>();
            health = GetComponent<Health>();

            guardPosition = transform.position;
        }

        void Update() {
            if (player == null) return;
            if (health.IsDead) return;
            
            if (IsPlayerInDetectRange() && characterCombat.CanAttack(player)) {
                timeSinceLastDetectedPlayer = 0;
                AttackBehaviour();
            } else if (timeSinceLastDetectedPlayer <= suspicionTime) {
                // Linger on doing nothing as if thinking or suspicious of player's action
                SuspicionBehaviour();
            } else {
                // Return to patrol if player is out of range or cannot be attacked
                PatrolBehaviour();
            }

            timeSinceLastDetectedPlayer += Time.deltaTime;
        }

        private void PatrolBehaviour() {
            Vector3 nextPosition = guardPosition;

            // If AI has a patrol path, start patrol
            if (patrolPath != null)  {
                if (IsAtWaypoint()) {
                    // If already at current waypoint, update the current waypoint to be the next waypoint
                    CycleWaypoint();
                }

                // Move to currently assigned waypoint
                nextPosition = GetCurrentWaypoint();
            }

            characterMovement.StartMoveAction(nextPosition);
        }

        private void CycleWaypoint() {
            currentWaypointIndex = patrolPath.GetNextIndex(currentWaypointIndex);
        }

        private Vector3 GetCurrentWaypoint() {
            return patrolPath.GetWaypoint(currentWaypointIndex);
        }

        private bool IsAtWaypoint() {
            // Check if at the waypoint or near it
            return Vector3.Distance(transform.position, GetCurrentWaypoint()) <= wayPointTolerance;
        }

        private void SuspicionBehaviour() {
            actionScheduler.CancelCurrentAction();
        }

        private void AttackBehaviour() {
            characterCombat.Attack(player);
        }

        private bool IsPlayerInDetectRange() {
            return Vector3.Distance(transform.position, player.transform.position) <= chaseDistance;
        }

        // Called by Unity
        private void OnDrawGizmosSelected() {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, chaseDistance);
        }
    }
}