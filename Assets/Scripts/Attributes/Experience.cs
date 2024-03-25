using Newtonsoft.Json.Linq;
using RPG.Saving;
using UnityEngine;

namespace RPG.Attributes {
    public class Experience : MonoBehaviour, IJsonSaveable {
        [SerializeField] float experiencePoints = 0;

        // Property
        public float ExperiencePoints { get => experiencePoints; }

        public void GainExperience(float experiencePoints) {
            this.experiencePoints += experiencePoints;
        }

        public JToken CaptureAsJToken() {
            return JToken.FromObject(experiencePoints);
        }

        public void RestoreFromJToken(JToken state) {
            experiencePoints = state.ToObject<float>();
        }
    }
}