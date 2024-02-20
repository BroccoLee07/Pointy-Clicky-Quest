using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace RPG.Control {
    public class PatrolPath : MonoBehaviour {

        private const float waypointGizmoRadius = 0.3f;
        private void OnDrawGizmos() {
            for (int i = 0; i < transform.childCount; i++) {
                Transform childTransform = transform.GetChild(i);
                Gizmos.DrawSphere(childTransform.position, waypointGizmoRadius);
            }
        }
    }
}

