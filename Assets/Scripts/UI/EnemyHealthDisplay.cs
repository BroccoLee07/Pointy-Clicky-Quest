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
                if (healthValue.text != String.Format("{0:0.0} / {1:0.0}", targetHealth.CurrentHealthPoints, targetHealth.MaxHealthPoints)) {
                    healthValue.text = String.Format("{0:0.0} / {1:0.0}", targetHealth.CurrentHealthPoints, targetHealth.MaxHealthPoints);
                }
            }
        }
    }
}
