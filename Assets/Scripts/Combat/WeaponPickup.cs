using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Combat {
    
    public class WeaponPickup : MonoBehaviour {
        [SerializeField] private Weapon weapon;
        private void OnTriggerEnter(Collider other) {
            if (other.tag == "Player") {
                other.GetComponent<CharacterCombat>().EquipWeapon(weapon);
                Destroy(gameObject);
            }
        }
    }
}
