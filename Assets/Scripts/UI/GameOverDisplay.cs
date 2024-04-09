using UnityEngine;

public class GameOverDisplay : MonoBehaviour {
    [SerializeField] private GameObject gameOverDisplay;

    public void Show(bool isVisible) {
        gameOverDisplay.SetActive(isVisible);
    }
}
