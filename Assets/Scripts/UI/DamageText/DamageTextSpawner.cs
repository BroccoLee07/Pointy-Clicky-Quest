using UnityEngine;

namespace RPG.UI.DamageText {
    public class DamageTextSpawner : MonoBehaviour {
        [SerializeField] private DamageText damageTextPrefab;

        public void Spawn(float damageValue) {
            DamageText damageTextObject = Instantiate(damageTextPrefab, gameObject.transform);
            damageTextObject.SetDisplayValue(damageValue);
        }    
    }
}
