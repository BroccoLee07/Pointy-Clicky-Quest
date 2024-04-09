using GameDevTV.Utils;
using Newtonsoft.Json.Linq;
using RPG.Core;
using RPG.Saving;
using RPG.Stats;
using UnityEngine;
using UnityEngine.Events;

namespace RPG.Attributes {
    [RequireComponent(typeof(ActionScheduler))]
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(BaseStats))]
    public class Health : MonoBehaviour, IJsonSaveable {

        [Tooltip("How much health is regenerated on level up")]
        [SerializeField] private float levelUpRegenerationPercentage = 15;
        [SerializeField] private LazyValue<float> healthPoints;
        [SerializeField] private UnityEvent<float> takeDamageEvent;
        [Tooltip("Meant for the player only to show any UI or options after dying")]
        [SerializeField] private UnityEvent<bool> playerPostDeathAction;
        [SerializeField] private UnityEvent onDeath;
        [SerializeField] private UnityEvent onReset;

        private bool isDead = false;
        private BaseStats baseStats;
        private bool canBeRevived = false;

        private const string ANIMATOR_DIE_TRIGGER = "die";

        // Property
        public bool IsDead { get => isDead; }
        public float CurrentHealthPoints { get => healthPoints.value; }
        public float MaxHealthPoints { get => baseStats.GetStat(Stat.Health); }

        void Awake() {
            baseStats = GetComponent<BaseStats>();
            // Makes sure correct value is retrieved
            healthPoints = new LazyValue<float>(GetInitialHealth);
        }

        private float GetInitialHealth() {
            return baseStats.GetStat(Stat.Health);
        }

        void Start() {
            // If health was not initialized prior to this point, make sure it is initialized
            healthPoints.ForceInit();            
            Reset();           
        }

        private void OnEnable() {
            if (baseStats != null) {
                // Subscribe to onLevelUp event of BaseStats to health regen code
                baseStats.onLevelUp += LevelUpRegenerateHealth;
            }
        }

        private void OnDisable() {
            if(baseStats != null) {
                // Subscribe to onLevelUp event of BaseStats to health regen code
                baseStats.onLevelUp -= LevelUpRegenerateHealth;
            }
        }

        public void TakeDamage(GameObject attackInitiator, float damage) {
            if (IsDead) return;

            // To avoid the health going below 0
            healthPoints.value = Mathf.Max(healthPoints.value - damage, 0);

            takeDamageEvent.Invoke(damage);

            UpdateHealthState();

            if (isDead) {
                onDeath.Invoke();
                AwardExperience(attackInitiator);

                if (gameObject.tag == "Player") {
                    playerPostDeathAction.Invoke(true);
                }
                 
                canBeRevived = true;
            }
        }

        public void Heal(float healValue) {
            healthPoints.value = Mathf.Min(healthPoints.value + healValue, MaxHealthPoints);
        }

        public float GetPercentage() {
            return 100 * GetFraction();
        }

        public float GetFraction() {
            return healthPoints.value / baseStats.GetStat(Stat.Health);
        }

        private void LevelUpRegenerateHealth() {
            // Heal up a percentage of new max health on level up
            healthPoints.value += baseStats.GetStat(Stat.Health) * (levelUpRegenerationPercentage / 100);
        }

        private void UpdateHealthState() {
            if (healthPoints.value <= 0) {
                isDead = true;
                Die();

                if (gameObject.tag == "Player") {
                    playerPostDeathAction.Invoke(true);
                }
            } else {
                if (gameObject.tag == "Player") {
                    playerPostDeathAction.Invoke(false);
                }
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

        private void Reset() {
            // If not dead, just run reset event
            if (!isDead) {
                onReset.Invoke();
                return;
            }

            // If not allowed to revive, end here
            if (!canBeRevived) return;

            // At this point, should be dead and revivable
            isDead = false;
            canBeRevived = false;

            // Reset animator to look alive again
            Animator animator = GetComponent<Animator>();
            animator.Rebind();
            animator.Update(0f);

            onReset.Invoke();
        }

        public JToken CaptureAsJToken() {
            if (isDead) {
                canBeRevived = false;
            }
            return JToken.FromObject(healthPoints.value);
        }

        public void RestoreFromJToken(JToken state) {
            healthPoints.value = state.ToObject<float>();
            Debug.Log($"{gameObject.name} health on load: {healthPoints.value}");
            UpdateHealthState();
            Reset();
        }

    }
}

