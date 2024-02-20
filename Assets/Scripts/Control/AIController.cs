using RPG.Combat;
using RPG.Core;
using RPG.Movement;
using UnityEngine;

namespace RPG.Control {
    [RequireComponent(typeof(CharacterMovement))]
    [RequireComponent(typeof(CharacterCombat))]
    [RequireComponent(typeof(Health))]
    public class AIController : MonoBehaviour {
        [SerializeField] private float chaseDistance = 5f;

        // Dependency
        private CharacterCombat characterCombat;
        private CharacterMovement characterMovement;
        private Health health;

        private GameObject player;

        void Start() {
            player = GameObject.FindWithTag("Player");
            characterCombat = GetComponent<CharacterCombat>();
            characterMovement = GetComponent<CharacterMovement>();
            health = GetComponent<Health>();
        }

        void Update() {
            if (player == null) return;
            if (health.IsDead) return;
            
            if (IsPlayerInDetectRange() && characterCombat.CanAttack(player)) {                
                characterCombat.Attack(player);
            } else {
                characterCombat.Cancel();
            }
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