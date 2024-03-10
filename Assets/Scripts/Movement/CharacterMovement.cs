using RPG.Core;
using UnityEngine;
using UnityEngine.AI;
using RPG.Saving;

namespace RPG.Movement {
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(ActionScheduler))]
    [RequireComponent(typeof(Health))]
    public class CharacterMovement : MonoBehaviour, IAction, ISaveable {
        [SerializeField] private float maxSpeed = 5.5f;

        // Dependency
        private ActionScheduler actionScheduler;
        private Health characterHealth;
        private NavMeshAgent characterNavMeshAgent;

        private const string ANIMATOR_FORWARD_SPEED = "forwardSpeed";

        void Start() {
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
            // TODO: Check if dead enemy or obstacle is on destination
            MoveTo(destination, speedFraction);
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

        public object CaptureState() {
            return new SerializableVector3(transform.position);
        }

        public void RestoreState(object state) {
            SerializableVector3 lastCharacterPos = (SerializableVector3)state;
            GetComponent<NavMeshAgent>().enabled = false;
            transform.position = lastCharacterPos.ToVector();
            GetComponent<NavMeshAgent>().enabled = true;
        }
    }
}