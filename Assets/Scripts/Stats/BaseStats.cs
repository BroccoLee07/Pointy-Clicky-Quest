using System;
using GameDevTV.Utils;
using RPG.Attributes;
using UnityEngine;

namespace RPG.Stats {
    public class BaseStats : MonoBehaviour {
        // [Range(1, 99)]
        [SerializeField] private CharacterClass characterClass;
        [SerializeField] private Progression progression;
        [SerializeField] private GameObject levelUpEffect;
        [SerializeField] private GameObject healUpEffect;
        [SerializeField] private bool shouldUseStatModifiers;

        [HideInInspector] public event Action onLevelUp;

        private Experience experience;
        private LazyValue<int> currentLevel;

        void Awake() {
            experience = GetComponent<Experience>();
            currentLevel = new LazyValue<int>(CalculateLevel);
        }

        void Start() {
            currentLevel.ForceInit();
        }

        // Gets called after Awake and before Start
        private void OnEnable() {
            if (experience != null) {
                experience.onExperienceGained += UpdateLevel;
            }            
        }

        private void OnDisable() {
            if (experience != null) {
                experience.onExperienceGained -= UpdateLevel;
            }
        }

        private void UpdateLevel() {
            int newLevel = CalculateLevel();
            if (newLevel > currentLevel.value) {
                currentLevel.value = newLevel;
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

        public void PlayHealEffect() {
            Instantiate(healUpEffect, transform);
        }

        public float GetStat(Stat stat) {
            // (Base stat + equips modifier stats) * (1 + (percentage modifier / 100))
            // percentage modifier divided by 100 to get decimal then added by to add percentage stat value to base with additive
            return (GetBaseStat(stat) + GetAdditiveModifier(stat)) * (1 + GetPercentageModifier(stat) / 100);
        }

        public float GetStat(Stat stat, int level) {
            // (Base stat + equips modifier stats) * (1 + (percentage modifier / 100))
            // percentage modifier divided by 100 to get decimal then added by to add percentage stat value to base with additive
            return (GetBaseStat(stat, level) + GetAdditiveModifier(stat)) * (1 + GetPercentageModifier(stat) / 100);
        }

        private float GetBaseStat(Stat stat) {
            return progression.GetStat(stat, characterClass, GetCurrentLevel());
        }

        private float GetBaseStat(Stat stat, int level) {
            return progression.GetStat(stat, characterClass, level);
        }

        public int GetCurrentLevel() {
            return currentLevel.value;
        }

        public float GetCurrentExperienceToNextLevel() {
            int currentLevel = GetCurrentLevel();
            float currentExpToNextLevel = GetBaseStat(Stat.ExperienceToNextLevel, 1);

            for (int level = 2; level <= currentLevel; level++) {
                currentExpToNextLevel += GetBaseStat(Stat.ExperienceToNextLevel, level);
            }

            return currentExpToNextLevel;
        }

        private int CalculateLevel() {
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
            if (!shouldUseStatModifiers) return 0;

            float totalAdditiveModifier = 0;
            foreach (IModifierProvider modifierProvider in GetComponents<IModifierProvider>()) {
                foreach (float modifier in modifierProvider.GetAdditiveModifiers(stat)) {
                    totalAdditiveModifier += modifier;
                }
            }

            return totalAdditiveModifier;
        }

        private float GetPercentageModifier(Stat stat) {
            if (!shouldUseStatModifiers) return 0;
            
            float totalPercentageModifier = 0;
            foreach (IModifierProvider modifierProvider in GetComponents<IModifierProvider>()) {
                foreach (float modifier in modifierProvider.GetPercentageModifiers(stat)) {
                    totalPercentageModifier += modifier;
                }
            }

            return totalPercentageModifier;
        }

        public void Reset() {
            currentLevel.value = CalculateLevel();
        }
    }
}