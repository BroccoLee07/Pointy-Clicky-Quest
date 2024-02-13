using UnityEngine;
using UnityEngine.AI;

public class Mover : MonoBehaviour {

    [SerializeField] private Transform target;
    
    void Update() {
        NavMeshAgent playerNavMeshAgent = gameObject.GetComponent<NavMeshAgent>();
        playerNavMeshAgent?.SetDestination(target.position);
    }
}
