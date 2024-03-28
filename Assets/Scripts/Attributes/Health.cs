using System;
using Newtonsoft.Json.Linq;
using RPG.Core;
using RPG.Saving;
using RPG.Stats;
using UnityEngine;

namespace RPG.Attributes {
    [RequireComponent(typeof(ActionScheduler))]
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(BaseStats))]
    public class Health : MonoBehaviour, IJsonSaveable {

        [Tooltip("How much health is regenerated on level up")]
        [SerializeField] private float levelUpRegenerationPercentage = 15;
        [SerializeField] private float healthPoints = -1f;

        private bool isDead = false;
        private BaseStats baseStats;

        private const string ANIMATOR_DIE_TRIGGER = "die";

        public bool IsDead { get => isDead; }

        void Start() {
            baseStats = GetComponent<BaseStats>();

            if(baseStats != null) {
                baseStats.onLevelUp += LevelUpRegenerateHealth;
            }

            // Value not restored from save state if any
            if (healthPoints < 0) {
                healthPoints = baseStats.GetStat(Stat.Health);
            }            
        }

        public void TakeDamage(GameObject attackInitiator, float damage) {
            if (IsDead) return;

            // To avoid the health going below 0
            healthPoints = Mathf.Max(healthPoints - damage, 0);
            // Debug.Log($"Took damage! Health is now {health}");

            UpdateHealthState();

            if (isDead) {
                AwardExperience(attackInitiator);
            }
        }

        public float GetPercentage() {
            return 100 * (healthPoints / baseStats.GetStat(Stat.Health));
        }

        private void LevelUpRegenerateHealth() {
            // float regenHealthPoints = baseStats.GetStat(Stat.Health) * (levelUpRegenerationPercentage / 100);
            // healthPoints = Mathf.Max(healthPoints, regenHealthPoints);

            // Heal up a percentage of new max health on level up
            healthPoints += baseStats.GetStat(Stat.Health) * (levelUpRegenerationPercentage / 100);
        }

        private void UpdateHealthState() {
            if (healthPoints <= 0) {
                isDead = true;
                Die();                
            }
        }

        private void AwardExperience(GameObject attackInitiator) {
            Experience exp = attackInitiator.GetComponent<Experience>();
            if (exp == null) return;

            exp.GainExperience(baseStats.GetStat(Stat.ExperienceReward));
        }

        private void Die() {
            GetComponent<Animator>().SetTrigger(ANIMATOR_DIE_TRIGGER);
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }

        public JToken CaptureAsJToken() {
            return JToken.FromObject(healthPoints);
        }

        public void RestoreFromJToken(JToken state) {
            healthPoints = state.ToObject<float>();
            UpdateHealthState();
        }

    }
}

