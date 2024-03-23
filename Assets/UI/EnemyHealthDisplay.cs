using System;
using RPG.Attributes;
using RPG.Combat;
using TMPro;
using UnityEngine;

namespace RPG.UI {
    public class EnemyHealthDisplay : MonoBehaviour {
        [SerializeField] TextMeshProUGUI healthValue;
        private CharacterCombat playerCharCombat;

        void Awake() {
            playerCharCombat = GameObject.FindWithTag("Player").GetComponent<CharacterCombat>();
        }

        void Update() {
            if (playerCharCombat.GetTargetHealth() == null) {
                healthValue.text = "N/A";
                return;
            } else {
                Health targetHealth = playerCharCombat.GetTargetHealth();
                if (healthValue.text != String.Format("{0:0.00}%", targetHealth.GetPercentage())) {
                    healthValue.text = String.Format("{0:0.00}%", targetHealth.GetPercentage());
                }
            }
        }
    }
}
