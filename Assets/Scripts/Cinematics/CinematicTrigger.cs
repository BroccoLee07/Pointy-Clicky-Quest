using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using RPG.Saving;
using UnityEngine;
using UnityEngine.Playables;

namespace RPG.Cinematics {
    public class CinematicTrigger : MonoBehaviour, IJsonSaveable {
        public bool isAlreadyTriggered = false;


        private void OnTriggerEnter(Collider other) {
            if(!isAlreadyTriggered && other.gameObject.tag == "Player") {
                isAlreadyTriggered = true;
                GetComponent<PlayableDirector>().Play();
            }            
        }

        public JToken CaptureAsJToken() {
            return JToken.FromObject(isAlreadyTriggered);
        }

        public void RestoreFromJToken(JToken state) {
            isAlreadyTriggered = state.ToObject<bool>();
        }
    }
}