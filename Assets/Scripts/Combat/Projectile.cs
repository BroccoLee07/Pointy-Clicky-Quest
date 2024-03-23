using System.Collections;
using System.Collections.Generic;
using RPG.Attributes;
using RPG.Core;
using UnityEngine;

namespace RPG.Combat {
    public class Projectile : MonoBehaviour {
        [Header("General Settings")]
        [SerializeField] private float speed = 5f;
        [SerializeField] private bool isHoming = false;
        [SerializeField] private GameObject hitEffect;
        [Space(3)]

        [Header("Cleanup Settings")]
        [SerializeField] private float maxLifetime = 10f;
        [SerializeField] private GameObject[] destroyOnHitObjects;
        [SerializeField] private float lifetimeAfterImpact = 0.2f;

        private Health target;
        private float weaponDamage = 0;        

        public void Initialize(Health targetHealth, float weaponDamage) {
            target = targetHealth;
            this.weaponDamage = weaponDamage;

            if (!isHoming) {
                // Set target once here for non-homing behaviour
                transform.LookAt(GetAimLocation(target));
            }

            Destroy(gameObject, maxLifetime);
        }

        void Update() {
            if (target == null) return;

            // Set projectile's target to update on every frame and have homing behaviour
            if (isHoming && !target.IsDead) {
                transform.LookAt(GetAimLocation(target));
            }

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

        private void OnTriggerEnter(Collider other) {
            if (other.GetComponent<Health>() != target) return;

            if (target.IsDead) {
                return;
            }

            target.TakeDamage(weaponDamage);
            // Set speed to 0 to keep it from moving forward
            speed = 0;

            if (hitEffect != null) {
                // Instantiate hit effect where projectile was last
                Instantiate(hitEffect, transform.position, transform.rotation);
            }

            foreach (GameObject toDestroyObj in destroyOnHitObjects) {
                Destroy(toDestroyObj);
            }

            Destroy(gameObject, lifetimeAfterImpact);
        }
    }
}

