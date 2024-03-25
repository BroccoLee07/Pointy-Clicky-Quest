using System;
using RPG.Attributes;
using TMPro;
using UnityEngine;

namespace RPG.UI {
    public class PlayerStatsDisplay : MonoBehaviour {
        [SerializeField] TextMeshProUGUI healthValue;
        [SerializeField] TextMeshProUGUI expValue;
        private Health health;
        private Experience experience;

        void Awake() {
            health = GameObject.FindWithTag("Player").GetComponent<Health>();
            experience = GameObject.FindWithTag("Player").GetComponent<Experience>();
        }

        void Update() {
            if (healthValue.text != String.Format("{0:0.00}%", health.GetPercentage())) {
                // Debug.Log($"health percent: {health.GetPercentage()}");
                healthValue.text = String.Format("{0:0.00}%", health.GetPercentage());                
            }

            if (expValue.text != experience.ExperiencePoints.ToString()){
                expValue.text = experience.ExperiencePoints.ToString();
            }
        }
    }
}
