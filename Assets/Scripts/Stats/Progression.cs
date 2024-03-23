using System;
using UnityEngine;

namespace RPG.Stats {
    [CreateAssetMenu(fileName = "Progression", menuName = "Stats/New Progression", order = 0)]
    public class Progression : ScriptableObject {
        [SerializeField] ProgressionCharacterClass[] characterClasses;

        public float GetHealth(CharacterClass characterClass, int level) {
            foreach (ProgressionCharacterClass progCharClass in characterClasses) {
                if (progCharClass.characterClass != characterClass) continue;

                return progCharClass.health[level - 1];
            }

            return 0;
        }

        public float GetExperienceReward(CharacterClass characterClass, int level) {
            foreach (ProgressionCharacterClass progCharClass in characterClasses) {
                if (progCharClass.characterClass != characterClass) continue;

                return progCharClass.experienceReward[level - 1];
            }

            return 0;
        }

        [Serializable]
        class ProgressionCharacterClass {
            public CharacterClass characterClass;
            public float[] health;
            public float[] experienceReward;
        }
    }
}