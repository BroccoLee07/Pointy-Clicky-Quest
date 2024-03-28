using System;
using RPG.Attributes;
using UnityEngine;

namespace RPG.Stats {
    public class BaseStats : MonoBehaviour {
        [Range(1, 99)]
        [SerializeField] private CharacterClass characterClass;
        [SerializeField] private Progression progression;
        [SerializeField] private GameObject levelUpEffect;
        [SerializeField] private GameObject healUpEffect;

        [HideInInspector] public event Action onLevelUp;

        private Experience experience;
        private int currentLevel = 0;

        void Start() {
            currentLevel = CalculateLevel();
            experience = GetComponent<Experience>();

            if (experience != null) {
                experience.onExperienceGained += UpdateLevel;
            }
        }

        private void UpdateLevel() {
            int newLevel = CalculateLevel();
            if (newLevel > currentLevel) {
                currentLevel = newLevel;
                print("Level up!");

                PlayLevelUpEffect();

                onLevelUp();
            }
        }

        private void PlayLevelUpEffect() {
            // In the current state of the game, levelling up also automatically heals the player
            // However, I would like to have a separate effect for just levelling and healing to reuse healing for potions
            Instantiate(levelUpEffect, transform);
            Instantiate(healUpEffect, transform);
        }

        public float GetStat(Stat stat) {
            return progression.GetStat(stat, characterClass, GetCurrentLevel()) + GetAdditiveModifier(stat);
        }

        public int GetCurrentLevel() {
            if (currentLevel < 1) {
                currentLevel = CalculateLevel();
            }

            return currentLevel;
        }

        private int CalculateLevel() {
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

        private float GetAdditiveModifier(Stat stat) {
            float totalAdditiveModifier = 0;
            foreach (IModifierProvider modifierProvider in GetComponents<IModifierProvider>()) {
                foreach (float modifier in modifierProvider.GetAdditiveModifier(stat)) {
                    totalAdditiveModifier += modifier;
                }
            }

            return totalAdditiveModifier;
        }
    }
}