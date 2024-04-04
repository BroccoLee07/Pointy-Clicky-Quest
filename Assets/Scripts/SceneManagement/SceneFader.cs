using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.SceneManagement {
    public class SceneFader : MonoBehaviour {
        private CanvasGroup canvasGroup;
        private Coroutine currentActiveFade;

        void Awake() {
            canvasGroup = GetComponent<CanvasGroup>();
        }

        public void FadeOutImmediate() {
            canvasGroup.alpha = 1;
        }

        public IEnumerator FadeOut(float time) {
            return Fade(1, time);
        }

        public IEnumerator FadeIn(float time) {
            return Fade(0, time);
        }

        public IEnumerator Fade(float target, float time) {
            if (currentActiveFade != null) {
                StopCoroutine(currentActiveFade);
            }
            currentActiveFade = StartCoroutine(FadeCanvas(target, time));
            yield return currentActiveFade;
        }

        private IEnumerator FadeCanvas(float target, float time) {
            while (!Mathf.Approximately(canvasGroup.alpha, target)) {
                canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, target, Time.deltaTime / time);
                yield return null;
            }
        }
    }
}
