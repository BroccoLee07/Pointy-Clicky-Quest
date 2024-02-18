using UnityEngine;

public class PlayerController : MonoBehaviour {
    private CharacterMovement characterMovement;

    void Start() {
        characterMovement = gameObject.GetComponent<CharacterMovement>();
        characterMovement.Initialize();
    }

    void Update() {
        if (Input.GetMouseButton(0)) {
            MoveToCursor();            
        }
    }

    private void MoveToCursor() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit raycastHit;

        // Check if Physics Raycast has a hit
        if (Physics.Raycast(ray, out raycastHit)) {
            characterMovement.MoveTo(raycastHit.point);
        }
    }
}
