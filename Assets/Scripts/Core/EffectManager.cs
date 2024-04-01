using UnityEngine;

namespace RPG.Core {
    public class EffectManager : MonoBehaviour {

        [SerializeField] GameObject targetToDestroy = null;

        void Update() {
            if (!GetComponent<ParticleSystem>().IsAlive()) {
                if (targetToDestroy != null) {
                    Destroy(targetToDestroy);
                } else {
                    Destroy(gameObject);
                }                
            }
        }
    }
}