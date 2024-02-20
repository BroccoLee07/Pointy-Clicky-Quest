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
        [SerializeField] private float waypointTolerance = 1f;
        [SerializeField] private float waypointDwellTime = 3f;

        // Dependency
        private CharacterCombat characterCombat;
        private CharacterMovement characterMovement;
        private ActionScheduler actionScheduler;
        private Health health;
        private GameObject player;

        private Vector3 guardPosition;
        private float timeSinceLastDetectedPlayer = Mathf.Infinity;
        private float timeSinceWaypointArrival = Mathf.Infinity;
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
                AttackBehaviour();
            } else if (timeSinceLastDetectedPlayer <= suspicionTime)  {
                // Linger on doing nothing as if thinking or suspicious of player's action
                SuspicionBehaviour();
            } else {
                // Return to patrol if player is out of range or cannot be attacked
                PatrolBehaviour();
            }

            UpdateTimers();
        }

        private void UpdateTimers() {
            timeSinceLastDetectedPlayer += Time.deltaTime;
            timeSinceWaypointArrival += Time.deltaTime;
        }

        private void PatrolBehaviour() {
            Vector3 nextPosition = guardPosition;

            // If AI has a patrol path, start patrol
            if (patrolPath != null)  {
                if (IsAtWaypoint()) {            
                    timeSinceWaypointArrival = 0;        
                    // If already at current waypoint, update the current waypoint to be the next waypoint
                    CycleWaypoint();
                }

                // Move to currently assigned waypoint
                nextPosition = GetCurrentWaypoint();
            }

            // Dwell at waypoint for time indicated then start moving again
            if (timeSinceWaypointArrival > waypointDwellTime) {
                characterMovement.StartMoveAction(nextPosition);
            }
        }

        private void CycleWaypoint() {
            // Update the current waypoint to be the next waypoint
            currentWaypointIndex = patrolPath.GetNextIndex(currentWaypointIndex);
        }

        private Vector3 GetCurrentWaypoint() {
            return patrolPath.GetWaypoint(currentWaypointIndex);
        }

        private bool IsAtWaypoint() {
            // Check if at the waypoint or near it
            return Vector3.Distance(transform.position, GetCurrentWaypoint()) <= waypointTolerance;
        }

        private void SuspicionBehaviour() {
            // Cancel any ongoing action to do nothing
            actionScheduler.CancelCurrentAction();
        }

        private void AttackBehaviour() {
            timeSinceLastDetectedPlayer = 0;
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