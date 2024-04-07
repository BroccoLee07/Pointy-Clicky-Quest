using System;
using RPG.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {
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

        if (isVisible) {
            Time.timeScale = 0f;
        } else {
            Time.timeScale = 1f;
        }
    }
}
