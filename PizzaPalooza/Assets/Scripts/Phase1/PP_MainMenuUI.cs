using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PP_MainMenuUI : MonoBehaviour
{
    [Header("Scene")]
    [SerializeField] private string level1 = "FFK Sample Scene";
    [SerializeField] private string level2 = "FFK Sample Scene 2";
    [SerializeField] private string level3 = "FFK Sample Scene 3";

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
            OnLevel1Pressed();
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

    public void OnLevel1Pressed()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(level1);
    }

    public void OnLevel2Pressed()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(level2);
    }

    public void OnLevel3Pressed()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(level3);
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
