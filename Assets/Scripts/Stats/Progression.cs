using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

namespace RPG.Stats {
    [CreateAssetMenu(fileName = "Progression", menuName = "Stats/New Progression", order = 0)]
    public class Progression : ScriptableObject {
        [SerializeField] ProgressionCharacterClass[] characterClasses;

        private Dictionary<CharacterClass, Dictionary<Stat, float[]>> lookupTable;

        public float GetStat(Stat stat, CharacterClass characterClass, int level) {
            BuildLookup();

            float[] levels = lookupTable[characterClass][stat];
            // Handle index out of bounds and give default value
            if (levels.Length < level) {
                return 0;
            }

            return levels[level - 1];
        }

        public int GetLevels(Stat stat, CharacterClass characterClass) {
            BuildLookup();

            float[] levels;
            lookupTable[characterClass].TryGetValue(stat, out levels);
            if (levels == null) {
                return 0;
            }
            
            levels = lookupTable[characterClass][stat];
            return levels.Length;
        }

        private void BuildLookup() {
            if (lookupTable != null) return;

            lookupTable = new Dictionary<CharacterClass, Dictionary<Stat, float[]>>();

            foreach (ProgressionCharacterClass progCharClass in characterClasses) {
                Dictionary<Stat, float[]> statLookupTable = new Dictionary<Stat, float[]>();
                foreach (ProgressionStat progressionStat in progCharClass.stats) {
                    // Debug.Log($"Class: {progCharClass.characterClass} Stat: {progressionStat.stat}");
                    statLookupTable[progressionStat.stat] = progressionStat.levels;
                }
                lookupTable[progCharClass.characterClass] = statLookupTable;
            }
        }

        [Serializable]
        class ProgressionCharacterClass {
            public CharacterClass characterClass;
            public ProgressionStat[] stats;
        }

        [Serializable]
        class ProgressionStat {
            public Stat stat;
            public float[] levels;
        }
    }
}