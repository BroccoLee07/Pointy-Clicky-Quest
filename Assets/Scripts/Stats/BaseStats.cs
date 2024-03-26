using RPG.Attributes;
using UnityEngine;

namespace RPG.Stats {
    public class BaseStats : MonoBehaviour {
        [Range(1, 99)]
        [SerializeField] private CharacterClass characterClass;
        [SerializeField] private Progression progression;

        public float GetStat(Stat stat) {
            return progression.GetStat(stat, characterClass, GetLevel());
        }

        public int GetLevel() {
            Experience experience = GetComponent<Experience>();
            // Debug.Log($"Character {characterClass} GetLevel experience: {experience}");

            int currentLevel = 1;
            if (experience == null) return currentLevel;
            
            float currentExp = experience.ExperiencePoints;
            // penultimate = second to last/max
            int penultimateLevel = progression.GetLevels(Stat.ExperienceToNextLevel, characterClass);
            for (int level = 1; level <= penultimateLevel; level++) {
                float expToNextLevel = progression.GetStat(Stat.ExperienceToNextLevel, characterClass, level);
                if (currentExp >= expToNextLevel) {
                    currentLevel++;
                    currentExp -= expToNextLevel;
                } else {
                    break;
                }
            }

            return currentLevel;
        }
    }
}