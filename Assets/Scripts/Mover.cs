using UnityEngine;
using UnityEngine.AI;

public class Mover : MonoBehaviour {

    [SerializeField] private Transform target;
    
    void Update() {
        if (Input.GetMouseButtonDown(0)) {
            MoveToCursor();            
        }
        
    }

    private void MoveToCursor() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit raycastHit;

        // Check if Physics Raycast has a hit
        if (Physics.Raycast(ray, out raycastHit)) {
            NavMeshAgent playerNavMeshAgent = gameObject.GetComponent<NavMeshAgent>();
            playerNavMeshAgent?.SetDestination(raycastHit.point);
        }
    }
}
