using TMPro;
using UnityEngine;

namespace RPG.UI.DamageText {
    public class DamageText : MonoBehaviour {
        [SerializeField] private TextMeshProUGUI displayValue;

        public void SetDisplayValue(float damageValue) {
            displayValue.text = damageValue.ToString();
        }
    }
}

