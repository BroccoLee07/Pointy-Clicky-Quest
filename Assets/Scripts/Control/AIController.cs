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

        // Dependency
        private CharacterCombat characterCombat;
        private CharacterMovement characterMovement;
        private ActionScheduler actionScheduler;
        private Health health;
        private GameObject player;

        private Vector3 guardPosition;
        private float timeSinceLastDetectedPlayer = Mathf.Infinity;
        

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
                // Return to guard position if player is out of range or cannot be attacked
                GuardBehaviour();
            }

            timeSinceLastDetectedPlayer += Time.deltaTime;
        }

        private void GuardBehaviour() {
            characterMovement.StartMoveAction(guardPosition);
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