using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Control {
    public class AIController : MonoBehaviour {
        [SerializeField] private float chaseDistance = 5f;

        private GameObject player;

        void Awake() {
            player = GameObject.FindWithTag("Player");
        }
        void Update() {
            if (player == null) return;
            
            if (IsPlayerInRange()) {
                Debug.Log($"{gameObject.name} will give chase to player");
            }
        }

        private bool IsPlayerInRange() {
            return Vector3.Distance(this.transform.position, player.transform.position) <= chaseDistance;
        }
    }
}