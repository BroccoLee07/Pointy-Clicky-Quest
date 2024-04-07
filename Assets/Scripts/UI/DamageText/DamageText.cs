using System;
using TMPro;
using UnityEngine;

namespace RPG.UI.DamageText {
    public class DamageText : MonoBehaviour {
        [SerializeField] private TextMeshProUGUI displayValue;

        public void SetDisplayValue(float damageValue) {
            displayValue.text =  String.Format("{0:0.0}", Mathf.Round(damageValue * 10) / 10);
        }

        public void DestroyText() {
            Destroy(gameObject);
        }
    }
}

