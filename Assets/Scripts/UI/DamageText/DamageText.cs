using System;
using TMPro;
using UnityEngine;

namespace RPG.UI.DamageText {
    public class DamageText : MonoBehaviour {
        [SerializeField] private TextMeshProUGUI displayValue;

        public void SetDisplayValue(float damageValue) {
            displayValue.text =  String.Format("{0:0.0}", damageValue.ToString());
        }

        public void DestroyText() {
            Destroy(gameObject);
        }
    }
}

