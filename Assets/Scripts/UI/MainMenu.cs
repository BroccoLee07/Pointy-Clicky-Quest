using System;
using RPG.SceneManagement;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {
    [SerializeField] private GameObject MainMenuObject;
    [SerializeField] private TextMeshProUGUI controlsInstructionsText;
    [TextArea(5,10)]
    [SerializeField] private string instructionsPC;
    [TextArea(5,10)]
    [SerializeField] private string instructionsWeb;

    private bool isVisible = false;

    void Awake() {
        MainMenuObject.SetActive(false);
    #if UNITY_WEBGL
        controlsInstructionsText.text = instructionsWeb;
    #else
        controlsInstructionsText.text = instructionsPC;
    #endif
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
