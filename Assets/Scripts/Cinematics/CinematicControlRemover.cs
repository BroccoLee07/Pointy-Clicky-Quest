using System.Collections;
using System.Collections.Generic;
using RPG.Control;
using RPG.Core;
using UnityEngine;
using UnityEngine.Playables;

namespace RPG.Cinematics {
    public class CinematicControlRemover : MonoBehaviour {

        private GameObject player;
        private ActionScheduler actionScheduler;

        void Awake() {
            player = GameObject.FindWithTag("Player");
            actionScheduler = player.GetComponent<ActionScheduler>();
        }

        void OnEnable() {
            GetComponent<PlayableDirector>().played += DisableControl;
            GetComponent<PlayableDirector>().stopped += EnableControl;
        }

        void OnDisable() {
            GetComponent<PlayableDirector>().played -= DisableControl;
            GetComponent<PlayableDirector>().stopped -= EnableControl;
        }

        void DisableControl(PlayableDirector playableDirector) {
            if (player == null || actionScheduler == null) {
                return;
            }

            actionScheduler.CancelCurrentAction();
            player.GetComponent<PlayerController>().enabled = false;
        }

        void EnableControl(PlayableDirector playableDirector) {
            if (player == null || actionScheduler == null) {
                return;
            }

            player.GetComponent<PlayerController>().enabled = true;
        }
    }
}
