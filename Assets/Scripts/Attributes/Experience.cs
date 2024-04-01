using System;
using Newtonsoft.Json.Linq;
using RPG.Saving;
using UnityEngine;

namespace RPG.Attributes {
    public class Experience : MonoBehaviour, IJsonSaveable {
        [SerializeField] float experiencePoints = 0;

        [HideInInspector] public event Action onExperienceGained;

        // Property
        public float ExperiencePoints { get => experiencePoints; }
        

        public void GainExperience(float experiencePoints) {
            this.experiencePoints += experiencePoints;
            onExperienceGained();
        }

        public JToken CaptureAsJToken() {
            return JToken.FromObject(experiencePoints);
        }

        public void RestoreFromJToken(JToken state) {
            experiencePoints = state.ToObject<float>();
        }
    }
}