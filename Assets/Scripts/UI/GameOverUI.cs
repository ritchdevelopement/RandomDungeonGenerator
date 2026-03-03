using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour {
    [SerializeField] private GameObject gameOverPanel;

    private void Awake() {
        gameOverPanel.SetActive(false);
        PlayerController.OnDeath += ShowGameOver;
    }

    private void OnDestroy() {
        PlayerController.OnDeath -= ShowGameOver;
    }

    private void ShowGameOver() {
        Time.timeScale = 0f;
        gameOverPanel.SetActive(true);
    }

    public void RestartGame() {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
