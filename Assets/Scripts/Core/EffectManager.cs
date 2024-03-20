using UnityEngine;

namespace RPG.Core {
    public class EffectManager : MonoBehaviour {

        void Update() {
            if (!GetComponent<ParticleSystem>().IsAlive()) {
                Destroy(gameObject);
            }
        }
    }
}