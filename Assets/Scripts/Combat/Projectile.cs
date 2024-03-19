using System.Collections;
using System.Collections.Generic;
using RPG.Core;
using UnityEngine;

namespace RPG.Combat {
    public class Projectile : MonoBehaviour {
        [SerializeField] private float speed = 5f;
        private Health target;

        // Property
        public Health Target { get => target; set => target = value; }        

        void Update() {
            if (target == null) return;

            Vector3 targetPos = GetAimLocation(target);
            Debug.Log($"targetPos: {targetPos}");

            transform.LookAt(targetPos);
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }

        private Vector3 GetAimLocation(Health target) {
            CapsuleCollider targetCapsuleCollider = target.GetComponent<CapsuleCollider>();
            if (targetCapsuleCollider == null) {
                return target.transform.position;
            }

            // Get position of target and random y position of target's upper half and below top of head
            return target.transform.position + Vector3.up * ((targetCapsuleCollider.height / 2) + Random.Range(0, targetCapsuleCollider.height / 3));
        }
    }
}

