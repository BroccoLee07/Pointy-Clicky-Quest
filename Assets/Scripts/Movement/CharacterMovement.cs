using RPG.Core;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Movement {
    public class CharacterMovement : MonoBehaviour, IAction {
        private const string ANIMATOR_FORWARD_SPEED = "forwardSpeed";

        // Dependency
        private ActionScheduler actionScheduler;
        private NavMeshAgent characterNavMeshAgent;

        public void Initialize() {
            characterNavMeshAgent = GetComponent<NavMeshAgent>();
            actionScheduler = GetComponent<ActionScheduler>();
        }

        void Update() {
            UpdateAnimator();
        }

        public void StartMoveAction(Vector3 destination) {
            // Cancel any ongoing combat when new movement is initiated
            actionScheduler.StartAction(this);
            // TODO: Check if dead enemy or obstacle is on destination
            MoveTo(destination);
        }

        public void MoveTo(Vector3 destination) {          
            characterNavMeshAgent?.SetDestination(destination);
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
    }
}