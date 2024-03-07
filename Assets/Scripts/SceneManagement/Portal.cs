using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RPG.SceneManagement {
    public class Portal : MonoBehaviour {

        [SerializeField] private int sceneToLoad = -1;


        private void OnTriggerEnter(Collider other) {
            if (other.gameObject.tag == "Player") {
                StartCoroutine(Transition());
            }
        }

        private IEnumerator Transition() {
            // Keep portal in the next scene until it is finished loading completely
            DontDestroyOnLoad(gameObject);
            yield return SceneManager.LoadSceneAsync(sceneToLoad);
            Debug.Log("Scene Loaded");
            Destroy(gameObject);
        }
    }
}


