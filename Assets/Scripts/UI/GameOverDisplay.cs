using UnityEngine;

public class GameOverDisplay : MonoBehaviour {
    [SerializeField] private GameObject gameOverDisplay;

    void Start() {
        Show(false);
    }

    public void Show(bool isVisible) {
        Debug.Log("Showing game over text");
        gameOverDisplay.SetActive(isVisible);
    }
}
