using System;
using RPG.Attributes;
using RPG.Stats;
using RPG.Combat;
using TMPro;
using UnityEngine;

namespace RPG.UI {
    public class PlayerStatsDisplay : MonoBehaviour {
        [SerializeField] TextMeshProUGUI healthValue;
        [SerializeField] TextMeshProUGUI levelValue;
        [SerializeField] TextMeshProUGUI expValue;
        [SerializeField] TextMeshProUGUI equippedWeaponValue;
        private Health health;
        private Experience experience;
        private BaseStats baseStats;
        private CharacterCombat characterCombat;

        void Awake() {
            health = GameObject.FindWithTag("Player").GetComponent<Health>();
            experience = GameObject.FindWithTag("Player").GetComponent<Experience>();
            baseStats = GameObject.FindWithTag("Player").GetComponent<BaseStats>();
            characterCombat = GameObject.FindWithTag("Player").GetComponent<CharacterCombat>();
        }

        void Update() {
            UpdateHealthDisplay();
            UpdateExperienceDisplay();
            UpdateLevelDisplay();
            UpdateEquippedWeapon();         
        }

        private void UpdateHealthDisplay() {
            if (healthValue.text != String.Format("{0:0.0} / {1:0.0}", health.CurrentHealthPoints, health.MaxHealthPoints)) {
                healthValue.text = String.Format("{0:0.0} / {1:0.0}", health.CurrentHealthPoints, health.MaxHealthPoints);                
            }
        }

        private void UpdateExperienceDisplay() {
            if (expValue.text != experience.ExperiencePoints.ToString()){
                // expValue.text = experience.ExperiencePoints.ToString();
                expValue.text = String.Format("{0:0.0} / {1:0.0}", 
                    experience.ExperiencePoints.ToString(), 
                    baseStats.GetCurrentExperienceToNextLevel().ToString()
                );
            }
        }

        private void UpdateLevelDisplay() {
            if (Int32.Parse(levelValue.text) != baseStats.GetCurrentLevel()) {
                levelValue.text = baseStats.GetCurrentLevel().ToString();
            }
        }

        private void UpdateEquippedWeapon() {
            equippedWeaponValue.text = characterCombat.CurrentWeaponConfig.WeaponName;

            if (characterCombat.CurrentWeaponConfig.WeaponName == "") {
                equippedWeaponValue.text = "Unarmed";
            }
        }
    }
}
