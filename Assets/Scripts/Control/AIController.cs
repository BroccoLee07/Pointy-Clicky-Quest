using RPG.Combat;
using RPG.Movement;
using UnityEngine;

namespace RPG.Control {
    public class AIController : MonoBehaviour {
        [SerializeField] private float chaseDistance = 5f;

        // Dependency
        private CharacterCombat characterCombat;
        private CharacterMovement characterMovement;

        private GameObject player;

        void Start() {
            player = GameObject.FindWithTag("Player");
            characterCombat = GetComponent<CharacterCombat>();
            characterMovement = GetComponent<CharacterMovement>();

            characterCombat.Initialize();
            characterMovement.Initialize();
        }

        void Update() {
            if (player == null) return;
            
            if (IsPlayerInDetectRange() && characterCombat.CanAttack(player)) {                
                characterCombat.Attack(player);
            } else {
                characterCombat.Cancel();
            }
        }

        private bool IsPlayerInDetectRange() {
            return Vector3.Distance(transform.position, player.transform.position) <= chaseDistance;
        }
    }
}