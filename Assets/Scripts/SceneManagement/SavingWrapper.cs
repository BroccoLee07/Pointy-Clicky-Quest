using System;
using System.Collections;
using RPG.Saving;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RPG.SceneManagement {
    [RequireComponent(typeof(JsonSavingSystem))]
    public class SavingWrapper : MonoBehaviour {

        [SerializeField] private float fadeInTime = 0.2f;
        private JsonSavingSystem savingSystem;

        const string defaultSaveFile = "gameSave";

        void Awake() {
            StartCoroutine(LoadLastScene());
        }
        
        private IEnumerator LoadLastScene() {
            savingSystem = GetComponent<JsonSavingSystem>();
            yield return savingSystem.LoadLastScene(defaultSaveFile);

            // Make sure Awakes have happened from yield return LoadLastScene before trying to find the object
            SceneFader fader = FindObjectOfType<SceneFader>();            
            fader.FadeOutImmediate();
            yield return fader.FadeIn(fadeInTime);
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

            if (Input.GetKeyDown(KeyCode.Delete)) {
                Delete();
            }

            if (Input.GetKeyDown(KeyCode.R)) {
                // Delete saved file and restart scene
                Restart();
            }

            // Check if Ctrl, Alt, and C keys are pressed simultaneously
            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftAlt) && Input.GetKey(KeyCode.C)) {
                Quit();
            }
        }

        public void Save() {
            savingSystem.Save(defaultSaveFile);
        }

        public void Load() {
            savingSystem.Load(defaultSaveFile);
        }

        public void Delete() {
            savingSystem.Delete(defaultSaveFile);
        }

        public void Restart() {
            Delete();
            // Restart the current scene
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public void Quit() {
            // Save before quitting
            Save();
            // Quit the application
            Application.Quit();
        }
    }
}
