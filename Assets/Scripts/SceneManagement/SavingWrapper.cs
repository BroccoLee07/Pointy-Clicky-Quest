using System;
using RPG.Saving;
using UnityEngine;

namespace RPG.SceneManagement {
    [RequireComponent(typeof(SavingSystem))]
    public class SavingWrapper : MonoBehaviour {

        private SavingSystem savingSystem;

        const string defaultSaveFile = "gameSave";

        void Start() {
            savingSystem = GetComponent<SavingSystem>();
        }

        void Update() {
            // Load save state with L key
            if (Input.GetKeyDown(KeyCode.L)) {
                Load();
            }

            // Save game state with S key
            if (Input.GetKeyDown(KeyCode.S)) {
                Save();
            }
        }

        private void Save() {
            savingSystem.Save(defaultSaveFile);
        }

        private void Load() {
            savingSystem.Load(defaultSaveFile);
        }
    }
}
