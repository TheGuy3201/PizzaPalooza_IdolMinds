using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PP_MainMenuUI : MonoBehaviour
{
    [Header("Scene")]
    [SerializeField] private string gameplaySceneName = "FFK Sample Scene";

    [Header("Panels")]
    [SerializeField] private GameObject rootPanel;
    [SerializeField] private GameObject howToPanel;
    [SerializeField] private GameObject settingsPanel;

    [Header("Default Select")]
    [SerializeField] private Button defaultButton;

    private void Start()
    {
        ShowRoot();
        if (defaultButton != null && EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(defaultButton.gameObject);
        }
    }

    private void Update()
    {
        if (Keyboard.current == null)
        {
            return;
        }

        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            ShowRoot();
        }

        if (Keyboard.current.digit1Key.wasPressedThisFrame)
        {
            OnPlayPressed();
        }

        if (Keyboard.current.digit2Key.wasPressedThisFrame)
        {
            OnHowToPressed();
        }

        if (Keyboard.current.digit3Key.wasPressedThisFrame)
        {
            OnSettingsPressed();
        }

        if (Keyboard.current.qKey.wasPressedThisFrame)
        {
            OnQuitPressed();
        }
    }

    public void OnPlayPressed()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(gameplaySceneName);
    }

    public void OnHowToPressed()
    {
        SetPanels(root: false, howTo: true, settings: false);
    }

    public void OnSettingsPressed()
    {
        SetPanels(root: false, howTo: false, settings: true);
    }

    public void OnBackPressed()
    {
        ShowRoot();
    }

    public void OnQuitPressed()
    {
        Application.Quit();
    }

    private void ShowRoot()
    {
        SetPanels(root: true, howTo: false, settings: false);
    }

    private void SetPanels(bool root, bool howTo, bool settings)
    {
        if (rootPanel != null)
        {
            rootPanel.SetActive(root);
        }

        if (howToPanel != null)
        {
            howToPanel.SetActive(howTo);
        }

        if (settingsPanel != null)
        {
            settingsPanel.SetActive(settings);
        }
    }
}
