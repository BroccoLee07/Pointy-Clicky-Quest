using UnityEngine;
using UnityEngine.AI;

namespace RPG.Movement {
    public class CharacterMovement : MonoBehaviour {
        private const string ANIMATOR_FORWARD_SPEED = "forwardSpeed";

        private NavMeshAgent characterNavMeshAgent;

        public void Initialize() {
            characterNavMeshAgent = gameObject.GetComponent<NavMeshAgent>();
        }

        void Update() {
            UpdateAnimator();
        }

        public void MoveTo(Vector3 destination) {
            characterNavMeshAgent?.SetDestination(destination);
        }

        private void UpdateAnimator() {
            Vector3 velocity = characterNavMeshAgent.velocity;
            // Convert from global velocity (from world space) to local velocity to be used by animator
            Vector3 localVelocity = transform.InverseTransformDirection(velocity);
            float speed = localVelocity.z;
            gameObject.GetComponent<Animator>().SetFloat(ANIMATOR_FORWARD_SPEED, speed);
        }
    }
}