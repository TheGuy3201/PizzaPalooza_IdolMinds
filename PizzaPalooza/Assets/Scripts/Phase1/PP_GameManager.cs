using UnityEngine;
using UnityEngine.SceneManagement;

public class PP_GameManager : MonoBehaviour
{
    [Header("Shift")]
    [SerializeField] private float shiftLengthSeconds = 180f;
    [SerializeField] private float maxPressureMultiplierIncrease = 1f;

    [Header("Failure")]
    [SerializeField] private float satisfactionLossPerFailure = 0.15f;
    [SerializeField] private float satisfactionGainPerSuccess = 0.03f;

    [Header("Refs")]
    [SerializeField] private PP_HUDController hud;

    private float remainingTime;
    private int score;
    private int tips;
    private float satisfaction = 1f;
    private string gameOverReason = string.Empty;

    public bool IsGameOver { get; private set; }
    public float RemainingTime => remainingTime;
    public int Score => score;
    public int Tips => tips;
    public float Satisfaction => satisfaction;
    public string GameOverReason => gameOverReason;

    private void Awake()
    {
        OptimizeShadowRendering();
    }

    private void Start()
    {
        remainingTime = shiftLengthSeconds;
        hud?.Refresh(this);
    }

    private void OptimizeShadowRendering()
    {
        // Disable shadows on all non-directional lights to prevent shadow atlas overload
        Light[] allLights = FindObjectsByType<Light>(FindObjectsSortMode.None);
        foreach (Light light in allLights)
        {
            if (light.type != LightType.Directional && light.shadows != LightShadows.None)
            {
                light.shadows = LightShadows.None;
            }
        }
    }

    private void Update()
    {
        if (IsGameOver)
        {
            return;
        }

        remainingTime -= Time.deltaTime;
        if (remainingTime <= 0f)
        {
            remainingTime = 0f;
            EndGame("Shift complete");
        }

        hud?.Refresh(this);

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            GoToMainMenu("MainMenu");
        }
    }

    public float GetPressureMultiplier()
    {
        float elapsed01 = 1f - Mathf.Clamp01(remainingTime / shiftLengthSeconds);
        return 1f + elapsed01 * maxPressureMultiplierIncrease;
    }

    public void NotifyOrderCompleted(int scoreDelta, int tipDelta, float patienceAtDelivery)
    {
        if (IsGameOver)
        {
            return;
        }

        score += Mathf.Max(0, scoreDelta);
        tips += Mathf.Max(0, tipDelta);
        satisfaction = Mathf.Clamp01(satisfaction + satisfactionGainPerSuccess * patienceAtDelivery);
        hud?.SetLastFeedback($"Order complete (+{scoreDelta} score, +{tipDelta} tips)");
    }

    public void NotifyOrderFailed(string reason)
    {
        if (IsGameOver)
        {
            return;
        }

        satisfaction = Mathf.Clamp01(satisfaction - satisfactionLossPerFailure);
        hud?.SetLastFeedback(reason);
        if (satisfaction <= 0f)
        {
            EndGame("Customers lost confidence");
        }
    }

    public void EndGame(string reason)
    {
        if (IsGameOver)
        {
            return;
        }

        IsGameOver = true;
        gameOverReason = reason;

        if (reason == "Customers lost confidence")
        {
            AudioManager.Play("FUUU");
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        hud?.ShowGameOver(this);
        Time.timeScale = 0f;
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        Scene active = SceneManager.GetActiveScene();
        SceneManager.LoadScene(active.buildIndex);
    }

    public void GoToMainMenu(string mainMenuSceneName)
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SceneManager.LoadScene(mainMenuSceneName);
    }
}
