using System;
using UnityEngine;

namespace RPG.Stats {
    [CreateAssetMenu(fileName = "Progression", menuName = "Stats/New Progression", order = 0)]
    public class Progression : ScriptableObject {
        [SerializeField] ProgressionCharacterClass[] characterClasses;

        public float GetStat(Stat stat, CharacterClass characterClass, int level) {
            foreach (ProgressionCharacterClass progCharClass in characterClasses) {
                if (progCharClass.characterClass != characterClass) continue;
                foreach (ProgressionStat progStat in progCharClass.stats) {
                    if (progStat.stat != stat) continue;
                    // Handle index out of bounds
                    if (progStat.levels.Length < level) continue;
                    
                    return progStat.levels[level - 1];
                }                
            }

            return 0;
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