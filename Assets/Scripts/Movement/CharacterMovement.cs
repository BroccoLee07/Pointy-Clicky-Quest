using RPG.Core;
using UnityEngine;
using UnityEngine.AI;
using RPG.Saving;
using Newtonsoft.Json.Linq;
using RPG.Attributes;

namespace RPG.Movement {    
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(ActionScheduler))]
    [RequireComponent(typeof(Health))]
    public class CharacterMovement : MonoBehaviour, IAction, IJsonSaveable {
        [SerializeField] private float maxSpeed = 5.5f;
        [SerializeField] private float maxNavPathLength = 40f;

        // Dependency
        private ActionScheduler actionScheduler;
        private Health characterHealth;
        private NavMeshAgent characterNavMeshAgent;

        private const string ANIMATOR_FORWARD_SPEED = "forwardSpeed";

        void Awake() {
            characterNavMeshAgent = GetComponent<NavMeshAgent>();
            actionScheduler = GetComponent<ActionScheduler>();
            characterHealth = GetComponent<Health>();
        }
        
        void Update() {
            characterNavMeshAgent.enabled = !characterHealth.IsDead;

            if (characterHealth.IsDead) return;

            UpdateAnimator();
        }

        public void StartMoveAction(Vector3 destination, float speedFraction) {
            // Cancel any ongoing combat when new movement is initiated
            actionScheduler.StartAction(this);
            MoveTo(destination, speedFraction);
        }

        public bool CanMoveTo(Vector3 targetPos) {
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

        public void MoveTo(Vector3 destination, float speedFraction) {          
            characterNavMeshAgent?.SetDestination(destination);
            characterNavMeshAgent.speed = maxSpeed * Mathf.Clamp01(speedFraction);
            characterNavMeshAgent.isStopped = false;
        }

        public void Cancel() {
            characterNavMeshAgent.isStopped = true;
        }

        private void UpdateAnimator() {
            Vector3 velocity = characterNavMeshAgent.velocity;
            // Convert from global velocity (from world space) to local velocity to be used by animator
            Vector3 localVelocity = transform.InverseTransformDirection(velocity);
            float speed = localVelocity.z;
            GetComponent<Animator>().SetFloat(ANIMATOR_FORWARD_SPEED, speed);
        }

        public JToken CaptureAsJToken() {
            CharacterMovementSaveData data = new CharacterMovementSaveData();
            data.position = transform.position;
            data.rotation = transform.eulerAngles;
            return CharacterMovementSaveData.ToJToken(data);
        }

        public void RestoreFromJToken(JToken state) {
            characterNavMeshAgent.enabled = false;
            transform.position = CharacterMovementSaveData.FromJToken(state).position;
            transform.eulerAngles = CharacterMovementSaveData.FromJToken(state).rotation;
            characterNavMeshAgent.enabled = true;
            actionScheduler.CancelCurrentAction();
        }        
    } 
}