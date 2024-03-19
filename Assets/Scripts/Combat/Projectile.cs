using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Combat {
    public class Projectile : MonoBehaviour {
        [SerializeField] private float speed = 5f;
        [SerializeField] private Transform target;

        void Start() {
            // ! For testing projectile movement
            StartCoroutine(MoveTo(target));
        }
        public IEnumerator MoveTo(Transform target) {
            if (target == null) yield break;

            Vector3 targetPos = GetAimLocation(target);
            // Debug.Log($"targetPos: {targetPos}");

            transform.LookAt(targetPos);
            while (Vector3.Distance(transform.position, targetPos) > 0.1f) {
                transform.Translate(Vector3.forward * speed * Time.deltaTime);
                yield return null;
            }            
        }

        private Vector3 GetAimLocation(Transform target) {
            CapsuleCollider targetCapsuleCollider = target.GetComponent<CapsuleCollider>();
            if (targetCapsuleCollider == null) {
                return target.position;
            }
            
            // Get position of target and random y position of target's upper half and below top of head
            return target.position + Vector3.up * ((targetCapsuleCollider.height / 2) + Random.Range(0, targetCapsuleCollider.height / 3));
        }
    }
}

