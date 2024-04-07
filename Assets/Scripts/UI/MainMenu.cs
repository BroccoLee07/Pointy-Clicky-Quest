using System;
using RPG.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {
    // [SerializeField] SavingWrapper saveControl;
    // public void OnNewGameButtonClick() {
    //     // saveControl.Delete();
    //     // TODO: Restart game here
    //     Debug.Log("New Game");
    // }

    // public void OnLoadGameButtonClick() {
    //     saveControl.Load();
    //     Debug.Log("Load Game");
    // }

    // public void OnInstructionsButtonClick() {
    //     // TODO: Show instructions UI
    //     Debug.Log("Instructions");
    // }

    // public void OnExitGameButtonClick() {
    //     // TODO: Show exit game confirmation popup
    //     Debug.Log("Exit Game");
    // }

    [SerializeField] private GameObject MainMenuObject;

    private bool isVisible = false;

    void Awake() {
        MainMenuObject.SetActive(false);
    }

    void Update() {
        // Check if Escape key was pressed
        if (Input.GetKeyDown(KeyCode.Escape)) {
            ToggleMainMenu();
        }
    }

    private void ToggleMainMenu() {
        isVisible = !isVisible;
        MainMenuObject.SetActive(isVisible);
    }
}
