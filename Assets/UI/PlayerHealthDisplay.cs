using System;
using RPG.Attributes;
using TMPro;
using UnityEngine;

namespace RPG.UI {
    public class PlayerHealthDisplay : MonoBehaviour {
        [SerializeField] TextMeshProUGUI healthValue;
        private Health health;

        void Awake() {
            health = GameObject.FindWithTag("Player").GetComponent<Health>();
        }

        void Update() {
            if (healthValue.text != String.Format("{0:0.00}%", health.GetPercentage())) {
                // Debug.Log($"health percent: {health.GetPercentage()}");
                healthValue.text = String.Format("{0:0.00}%", health.GetPercentage());
            }
        }
    }
}
