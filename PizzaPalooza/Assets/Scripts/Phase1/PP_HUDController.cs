using TMPro;
using UnityEngine;

public class PP_HUDController : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private PP_GameManager gameManager;
    [SerializeField] private PP_OrderManager orderManager;

    [Header("HUD Text")]
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text tipsText;
    [SerializeField] private TMP_Text satisfactionText;
    [SerializeField] private TMP_Text ordersText;
    [SerializeField] private TMP_Text feedbackText;

    [Header("Game Over")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TMP_Text gameOverReasonText;
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    private void Update()
    {
        if (gameManager == null)
        {
            return;
        }

        Refresh(gameManager);
        if (orderManager != null && ordersText != null)
        {
            ordersText.text = orderManager.GetOrdersDisplayText();
        }
    }

    public void Refresh(PP_GameManager state)
    {
        if (state == null)
        {
            return;
        }

        if (timerText != null)
        {
            int totalSeconds = Mathf.CeilToInt(state.RemainingTime);
            int minutes = totalSeconds / 60;
            int seconds = totalSeconds % 60;
            timerText.text = $"Time: {minutes:00}:{seconds:00}";
        }

        if (scoreText != null)
        {
            scoreText.text = $"Score: {state.Score}";
        }

        if (tipsText != null)
        {
            tipsText.text = $"Tips: {state.Tips}";
        }

        if (satisfactionText != null)
        {
            satisfactionText.text = $"Satisfaction: {Mathf.RoundToInt(state.Satisfaction * 100f)}%";
        }
    }

    public void SetLastFeedback(string value)
    {
        if (feedbackText != null)
        {
            feedbackText.text = value;
        }
    }

    public void ShowGameOver(PP_GameManager state)
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        if (gameOverReasonText != null)
        {
            gameOverReasonText.text = state != null ? state.GameOverReason : "Game Over";
        }
    }

    public void OnRestartPressed()
    {
        gameManager?.RestartLevel();
    }

    public void OnMainMenuPressed()
    {
        gameManager?.GoToMainMenu(mainMenuSceneName);
    }
}
