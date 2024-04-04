using RPG.Control;
using RPG.Core;
using UnityEngine;

namespace RPG.SceneManagement {
    public class PortalControlRemover : MonoBehaviour {

        private GameObject player;
        private ActionScheduler actionScheduler;

        public void EnableControl(bool isEnabled) {
            player = GameObject.FindWithTag("Player");
            actionScheduler = player.GetComponent<ActionScheduler>();

            if (player == null || actionScheduler == null) {
                return;
            }

            if (!isEnabled) {
                actionScheduler.CancelCurrentAction();
            }
            player.GetComponent<PlayerController>().enabled = isEnabled;
        }
    }

}