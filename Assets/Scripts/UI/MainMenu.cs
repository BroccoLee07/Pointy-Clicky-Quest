using RPG.SceneManagement;
using UnityEngine;

public class MainMenu : MonoBehaviour {
    [SerializeField] SavingWrapper saveControl;
    public void OnNewGameButtonClick() {
        // saveControl.Delete();
        // TODO: Restart game here
        Debug.Log("New Game");
    }

    public void OnLoadGameButtonClick() {
        saveControl.Load();
        Debug.Log("Load Game");
    }

    public void OnInstructionsButtonClick() {
        // TODO: Show instructions UI
        Debug.Log("Instructions");
    }

    public void OnExitGameButtonClick() {
        // TODO: Show exit game confirmation popup
        Debug.Log("Exit Game");
    }

}
