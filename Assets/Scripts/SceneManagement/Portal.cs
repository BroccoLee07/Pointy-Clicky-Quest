using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;
using RPG.Control;
using RPG.Movement;

namespace RPG.SceneManagement {
    [RequireComponent(typeof(PortalControlRemover))]
    public class Portal : MonoBehaviour {

        private enum DestinationIdentifier {
            A, B, C, D, E
        }

        [Header("Portal Settings")]
        [SerializeField] private int sceneToLoad = -1;
        [SerializeField] private Transform spawnPoint;
        [SerializeField] DestinationIdentifier destination;
        [SerializeField] private PortalControlRemover portalControlRemover;

        [Space(10)]
        [Header("Scene Fader")]
        [SerializeField] private float fadeOutTime = 0.5f;
        [SerializeField] private float fadeInTime = 1f;
        [SerializeField] private float fadeWaitTime = 0.5f;
        

        private void OnTriggerEnter(Collider other) {
            if (other.gameObject.tag == "Player") {
                StartCoroutine(Transition());
            }
        }

        private IEnumerator Transition() {
            if (sceneToLoad < 0) {
                Debug.LogError("Scene to load not set");
                yield break;
            }

            // Keep portal in the next scene until it is finished loading completely
            DontDestroyOnLoad(gameObject);

            // Remove player control for current player object
            portalControlRemover.EnableControl(false);

            SceneFader fader = FindObjectOfType<SceneFader>();
            // Fade out as transition while changing scenes
            yield return fader.FadeOut(fadeOutTime);

            // Save current (transitioned from) level state
            SavingWrapper savingWrapper = FindObjectOfType<SavingWrapper>();
            savingWrapper.Save();
            
            yield return SceneManager.LoadSceneAsync(sceneToLoad);

            // Remove player control for new player object
            portalControlRemover.EnableControl(false);

            // Load current (transitioned to) level
            savingWrapper.Load();            

            Portal otherPortal = GetOtherPortal();
            UpdatePlayer(otherPortal);

            // Save after teleporting as checkpoint
            savingWrapper.Save();

            // Have a bit of buffer for everything to initialize
            yield return new WaitForSeconds(fadeWaitTime);
            // Fade back in after loading everything needed in the new scene
            yield return fader.FadeIn(fadeInTime);

            // Restore player control
            portalControlRemover.EnableControl(true);

            Destroy(gameObject);
        }

        private Portal GetOtherPortal() {
            foreach (Portal portal in FindObjectsOfType<Portal>()) {
                if (portal == this) continue;
                if (portal.destination != destination) continue;

                return portal;
            }

            return null;
        }

        private void UpdatePlayer(Portal otherPortal) {
            GameObject player = GameObject.FindWithTag("Player");
            player.GetComponent<NavMeshAgent>().enabled = false;
            player.transform.position = otherPortal.spawnPoint.position;
            player.transform.rotation = otherPortal.spawnPoint.rotation;
            player.GetComponent<NavMeshAgent>().enabled = true;
        }
    }
}


