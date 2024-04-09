using System.Collections;
using RPG.Attributes;
using RPG.Saving;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace RPG.SceneManagement {
    [RequireComponent(typeof(JsonSavingSystem))]
    public class SavingWrapper : MonoBehaviour {

        [SerializeField] private float fadeInTime = 0.2f;
        public UnityEvent<string> OnSaveStateChanged = new UnityEvent<string>();
        // public UnityEvent OnGameOverAction = new UnityEvent();
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
#if !UNITY_WEBGL
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
#endif

            if (Input.GetKeyDown(KeyCode.R)) {
                // Delete saved file and restart scene
                Restart();
            }
#if !UNITY_WEBGL
            // Check if Ctrl, Alt, and C keys are pressed simultaneously
            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftAlt) && Input.GetKey(KeyCode.C)) {
                Quit();
            }
#endif
        }

        public void Save() {
            savingSystem.Save(defaultSaveFile);
            OnSaveStateChanged.Invoke("Game saved!");
        }

        public void Load() {
            savingSystem.Load(defaultSaveFile);
            OnSaveStateChanged.Invoke("Game loaded!");
        }

        public void Delete() {
            savingSystem.Delete(defaultSaveFile);
            OnSaveStateChanged.Invoke("Save deleted!");
        }

        public void Restart() {
#if !UNITY_WEBGL
            Delete();
#endif
            // Restart to the first map/scene
            SceneManager.LoadScene(0);
            OnSaveStateChanged.Invoke("Game restarted!");
        }

        public void Quit() {
            // Save before quitting
            Save();
            // Quit the application
            Application.Quit();
        }
    }
}
