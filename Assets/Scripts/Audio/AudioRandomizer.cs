using UnityEngine;

namespace RPG.Audio {
    public class AudioRandomizer : MonoBehaviour {
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip[] audioList;

        void Start() {
            if (audioSource == null) {
                audioSource = GetComponent<AudioSource>();
            }
        }
        public void PlayRandom() {
            int index = Random.Range(0, audioList.Length);
            audioSource.clip = audioList[index];
            audioSource.Play();
        }
    }
}
