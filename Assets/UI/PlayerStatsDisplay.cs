using System;
using RPG.Attributes;
using RPG.Stats;
using TMPro;
using UnityEngine;

namespace RPG.UI {
    public class PlayerStatsDisplay : MonoBehaviour {
        [SerializeField] TextMeshProUGUI healthValue;
        [SerializeField] TextMeshProUGUI levelValue;
        [SerializeField] TextMeshProUGUI expValue;
        private Health health;
        private Experience experience;
        private BaseStats baseStats;

        void Awake() {
            health = GameObject.FindWithTag("Player").GetComponent<Health>();
            experience = GameObject.FindWithTag("Player").GetComponent<Experience>();
            baseStats = GameObject.FindWithTag("Player").GetComponent<BaseStats>();
        }

        void Update() {
            UpdateHealthDisplay();
            UpdateExperienceDisplay();
            UpdateLevelDisplay();            
        }

        private void UpdateHealthDisplay() {
            if (healthValue.text != String.Format("{0:0.0} / {1:0.0}", health.CurrentHealthPoints, health.MaxHealthPoints)) {
                // Debug.Log($"health percent: {health.GetPercentage()}");
                healthValue.text = String.Format("{0:0.0} / {1:0.0}", health.CurrentHealthPoints, health.MaxHealthPoints);                
            }
        }

        private void UpdateExperienceDisplay() {
            if (expValue.text != experience.ExperiencePoints.ToString()){
                expValue.text = experience.ExperiencePoints.ToString();
            }
        }

        private void UpdateLevelDisplay() {
            if (Int32.Parse(levelValue.text) != baseStats.GetCurrentLevel()) {
                levelValue.text = baseStats.GetCurrentLevel().ToString();
            }
        }
    }
}
