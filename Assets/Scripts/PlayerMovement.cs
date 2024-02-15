using UnityEngine;
using UnityEngine.AI;

public class PlayerMovement : MonoBehaviour {

    [SerializeField] private Transform target;

    private const string ANIMATOR_FORWARD_SPEED = "forwardSpeed";

    private NavMeshAgent playerNavMeshAgent;

    void Start() {
        playerNavMeshAgent = gameObject.GetComponent<NavMeshAgent>();
    }
    
    void Update() {
        if (Input.GetMouseButton(0)) {
            MoveToCursor();            
        }

        UpdateAnimator();
    }

    private void MoveToCursor() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit raycastHit;

        // Check if Physics Raycast has a hit
        if (Physics.Raycast(ray, out raycastHit)) {            
            playerNavMeshAgent?.SetDestination(raycastHit.point);
        }
    }

    private void UpdateAnimator() {
        Vector3 velocity = playerNavMeshAgent.velocity;
        // Convert from global velocity (from world space) to local velocity to be used by animator
        Vector3 localVelocity = transform.InverseTransformDirection(velocity);
        float speed = localVelocity.z;
        gameObject.GetComponent<Animator>().SetFloat(ANIMATOR_FORWARD_SPEED, speed);
    }
}
