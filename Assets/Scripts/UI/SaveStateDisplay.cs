using UnityEngine;
using TMPro;
using System.Collections;

namespace RPG.UI {
    public class SaveStateDisplay : MonoBehaviour {

        [SerializeField] private TextMeshProUGUI saveStateText;
        [SerializeField] private float displayDuration = 5f;
        private Coroutine displayCoroutine;

        void Awake() {
            saveStateText.gameObject.SetActive(false);
        }

        public void ShowSaveState(string stateMessage) {
            if (displayCoroutine != null) {
                StopCoroutine(displayCoroutine);
            }

            saveStateText.text = stateMessage;
            displayCoroutine = StartCoroutine(DisplaySaveState());
        }

        private IEnumerator DisplaySaveState() {
            saveStateText.gameObject.SetActive(true);
            yield return new WaitForSeconds(displayDuration);
            saveStateText.gameObject.SetActive(false);
        }
    }
}

