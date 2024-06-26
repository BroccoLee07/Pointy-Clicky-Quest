using System;
using GameDevTV.Utils;
using RPG.Attributes;
using RPG.Combat;
using RPG.Core;
using RPG.Movement;
using RPG.Stats;
using UnityEngine;

namespace RPG.Control {
    [RequireComponent(typeof(CharacterMovement))]
    [RequireComponent(typeof(CharacterCombat))]
    [RequireComponent(typeof(ActionScheduler))]
    [RequireComponent(typeof(Health))]
    [RequireComponent(typeof(BaseStats))]
    public class AIController : MonoBehaviour {
        [SerializeField] private float chaseDistance = 5f;
        [SerializeField] private float suspicionTime = 5f;
        [SerializeField] private float aggroCooldownTime = 6.5f;
        [SerializeField] private PatrolPath patrolPath; // Can be null
        [SerializeField] private float waypointTolerance = 1f;
        [SerializeField] private float waypointDwellTime = 3f;
        [Range(0,1)]
        [SerializeField] private float patrolSpeedFraction = 0.5f;
        [Tooltip("AI calls for reinforcements in this max distance")]
        [SerializeField] private float shoutDistance = 5f;
        
        // Dependency
        private CharacterCombat characterCombat;
        private CharacterMovement characterMovement;
        private ActionScheduler actionScheduler;
        private Health health;
        private GameObject player;

        private LazyValue<Vector3> guardPosition;
        private float timeSinceLastDetectedPlayer = Mathf.Infinity;
        private float timeSinceLastAggravated = Mathf.Infinity;
        private float timeSinceWaypointArrival = Mathf.Infinity;
        private int currentWaypointIndex = 0;
        public bool wasAggravatedByOtherAi = false;
        public bool hasShoutedForHelp = false;
        

        void Awake() {
            player = GameObject.FindWithTag("Player");
            characterCombat = GetComponent<CharacterCombat>();
            characterMovement = GetComponent<CharacterMovement>();
            actionScheduler = GetComponent<ActionScheduler>();
            health = GetComponent<Health>();

            guardPosition = new LazyValue<Vector3>(GetGuardPosition);
        }

        void Start() {
            guardPosition.ForceInit();
        }

        void Update() {
            if (player == null) return;
            if (health.IsDead) return;

            if (IsAggravated() && characterCombat.CanAttack(player)) {
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

        private Vector3 GetGuardPosition() {
            return transform.position;
        }

        public void Aggravate() {
            timeSinceLastAggravated = 0;
        }

        private void UpdateTimers() {
            timeSinceLastDetectedPlayer += Time.deltaTime;
            timeSinceWaypointArrival += Time.deltaTime;
            timeSinceLastAggravated += Time.deltaTime;
        }

        private void PatrolBehaviour() {
            Vector3 nextPosition = guardPosition.value;

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
                characterMovement.StartMoveAction(nextPosition, patrolSpeedFraction);
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

            // In case AI was only attacking because another AI was attacked, set this back to false
            wasAggravatedByOtherAi = false;
            // In case other AI was called for help, set this back to false to reset
            hasShoutedForHelp = false;
        }

        private void AttackBehaviour() {
            timeSinceLastDetectedPlayer = 0;
            characterCombat.Attack(player);

            AggravateNearbyEnemies();
        }

        private void AggravateNearbyEnemies() {
            // Avoid chain aggravate and infinite aggravate; Can only shout for help once per aggro
            if (wasAggravatedByOtherAi || hasShoutedForHelp) return;

            // Debug.Log($"Aggravate nearby enemies");
            // Casts sphere from center and doesn't travel at a distance essentially detecting anything in range of radius
            RaycastHit[] hits = Physics.SphereCastAll(transform.position, shoutDistance, Vector3.up, 0);
            foreach (RaycastHit hit in hits) {
                AIController ai = hit.collider.GetComponent<AIController>();
                if (ai == null || ai == this || ai.wasAggravatedByOtherAi) continue;

                // Debug.Log($"Aggravate nearby {ai.gameObject.name}");
                ai.wasAggravatedByOtherAi = true;
                ai.Aggravate();
            }

            hasShoutedForHelp = true;
        }

        private bool IsAggravated() {            
            bool isPlayerInDetectRange = Vector3.Distance(transform.position, player.transform.position) <= chaseDistance;
            bool isAggravated = timeSinceLastAggravated < aggroCooldownTime;
            return isPlayerInDetectRange || isAggravated;
        }


        public void Reset() {
            timeSinceLastAggravated = Mathf.Infinity;
            timeSinceLastDetectedPlayer = Mathf.Infinity;

            wasAggravatedByOtherAi = false;
            hasShoutedForHelp = false;
        }

        // Called by Unity
        private void OnDrawGizmosSelected() {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, chaseDistance);
        }
    }
}