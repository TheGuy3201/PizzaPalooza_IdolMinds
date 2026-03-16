using TMPro;
using UnityEditor;
using UnityEditor.Events;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class PP_HUDBuilder
{
    private const string RootName = "GameplayHUD";
    private const string SystemsName = "GameSystems";
    private const string HudControllerName = "HUDController";

    [MenuItem("PizzaPalooza/Build Gameplay HUD (TMP)")]
    public static void BuildGameplayHud()
    {
        if (!EditorSceneManagerGuard())
        {
            return;
        }

        Undo.IncrementCurrentGroup();
        int group = Undo.GetCurrentGroup();

        GameObject root = FindOrCreate(RootName);
        Undo.RegisterCreatedObjectUndo(root, "Create Gameplay HUD Root");

        Canvas canvas = root.GetComponent<Canvas>();
        if (canvas == null)
        {
            canvas = Undo.AddComponent<Canvas>(root);
        }
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.pixelPerfect = true;

        CanvasScaler scaler = root.GetComponent<CanvasScaler>();
        if (scaler == null)
        {
            scaler = Undo.AddComponent<CanvasScaler>(root);
        }
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;

        if (root.GetComponent<GraphicRaycaster>() == null)
        {
            Undo.AddComponent<GraphicRaycaster>(root);
        }

        Transform topBar = CreatePanel(root.transform, "TopBar", new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0f, 1f), new Vector2(0f, -80f), new Vector2(0f, 0f), new Color(0f, 0f, 0f, 0.45f));
        TMP_Text timer = CreateLabel(topBar, "TimerText", "Time: 03:00", 28, TextAlignmentOptions.Left, new Vector2(0f, 0f), new Vector2(0.2f, 1f), new Vector2(12f, 0f), new Vector2(-8f, 0f));
        TMP_Text score = CreateLabel(topBar, "ScoreText", "Score: 0", 28, TextAlignmentOptions.Left, new Vector2(0.2f, 0f), new Vector2(0.4f, 1f), new Vector2(8f, 0f), new Vector2(-8f, 0f));
        TMP_Text tips = CreateLabel(topBar, "TipsText", "Tips: 0", 28, TextAlignmentOptions.Left, new Vector2(0.4f, 0f), new Vector2(0.6f, 1f), new Vector2(8f, 0f), new Vector2(-8f, 0f));
        TMP_Text satisfaction = CreateLabel(topBar, "SatisfactionText", "Satisfaction: 100%", 28, TextAlignmentOptions.Left, new Vector2(0.6f, 0f), new Vector2(1f, 1f), new Vector2(8f, 0f), new Vector2(-12f, 0f));

        Transform leftPanel = CreatePanel(root.transform, "OrdersPanel", new Vector2(0f, 0f), new Vector2(0.42f, 0.78f), new Vector2(0f, 0f), new Vector2(24f, 24f), new Vector2(-24f, -24f), new Color(0f, 0f, 0f, 0.4f));
        CreateLabel(leftPanel, "OrdersTitle", "Active Orders", 30, TextAlignmentOptions.Left, new Vector2(0f, 0.84f), new Vector2(1f, 1f), new Vector2(16f, -4f), new Vector2(-16f, -6f));
        TMP_Text orders = CreateLabel(leftPanel, "OrdersText", "No active orders", 24, TextAlignmentOptions.TopLeft, new Vector2(0f, 0f), new Vector2(1f, 0.84f), new Vector2(16f, 8f), new Vector2(-16f, -12f));

        Transform bottomPanel = CreatePanel(root.transform, "FeedbackPanel", new Vector2(0f, 0f), new Vector2(1f, 0f), new Vector2(0f, 0f), new Vector2(0f, 0f), new Vector2(0f, 120f), new Color(0f, 0f, 0f, 0.5f));
        TMP_Text feedback = CreateLabel(bottomPanel, "FeedbackText", "Ready for rush hour.", 30, TextAlignmentOptions.Center, new Vector2(0f, 0f), new Vector2(1f, 1f), new Vector2(24f, 8f), new Vector2(-24f, -8f));

        Transform gameOverPanel = CreatePanel(root.transform, "GameOverPanel", new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(-320f, -220f), new Vector2(320f, 220f), new Color(0f, 0f, 0f, 0.85f));
        gameOverPanel.gameObject.SetActive(false);
        TMP_Text gameOverTitle = CreateLabel(gameOverPanel, "GameOverTitle", "Game Over", 46, TextAlignmentOptions.Center, new Vector2(0f, 0.72f), new Vector2(1f, 1f), new Vector2(24f, -8f), new Vector2(-24f, -8f));
        TMP_Text reason = CreateLabel(gameOverPanel, "ReasonText", "Reason", 28, TextAlignmentOptions.Center, new Vector2(0f, 0.48f), new Vector2(1f, 0.72f), new Vector2(24f, -4f), new Vector2(-24f, -8f));

        Button restartButton = CreateButton(gameOverPanel, "RestartButton", "Restart Level", new Vector2(0.5f, 0.22f), new Vector2(250f, 64f));
        Button menuButton = CreateButton(gameOverPanel, "MainMenuButton", "Main Menu", new Vector2(0.5f, 0.06f), new Vector2(250f, 64f));

        GameObject hudObject = FindOrCreate(HudControllerName);
        PP_HUDController hud = hudObject.GetComponent<PP_HUDController>();
        if (hud == null)
        {
            hud = Undo.AddComponent<PP_HUDController>(hudObject);
        }

        GameObject systems = FindOrCreate(SystemsName);
        PP_GameManager gm = systems.GetComponent<PP_GameManager>();
        if (gm == null)
        {
            gm = Undo.AddComponent<PP_GameManager>(systems);
        }

        PP_OrderManager om = systems.GetComponent<PP_OrderManager>();
        if (om == null)
        {
            om = Undo.AddComponent<PP_OrderManager>(systems);
        }

        SetSerializedReference(hud, "gameManager", gm);
        SetSerializedReference(hud, "orderManager", om);
        SetSerializedReference(hud, "timerText", timer);
        SetSerializedReference(hud, "scoreText", score);
        SetSerializedReference(hud, "tipsText", tips);
        SetSerializedReference(hud, "satisfactionText", satisfaction);
        SetSerializedReference(hud, "ordersText", orders);
        SetSerializedReference(hud, "feedbackText", feedback);
        SetSerializedReference(hud, "gameOverPanel", gameOverPanel.gameObject);
        SetSerializedReference(hud, "gameOverReasonText", reason);

        SetSerializedReference(gm, "hud", hud);
        SetSerializedReference(om, "gameManager", gm);

        restartButton.onClick.RemoveAllListeners();
        menuButton.onClick.RemoveAllListeners();
        UnityEventTools.AddPersistentListener(restartButton.onClick, hud.OnRestartPressed);
        UnityEventTools.AddPersistentListener(menuButton.onClick, hud.OnMainMenuPressed);

        EditorUtility.SetDirty(hud);
        EditorUtility.SetDirty(gm);
        EditorUtility.SetDirty(om);

        Undo.CollapseUndoOperations(group);
        Debug.Log("Gameplay HUD created and wired. Check HUDController Main Menu scene name if needed.");
    }

    private static bool EditorSceneManagerGuard()
    {
        if (!SceneManager.GetActiveScene().IsValid())
        {
            Debug.LogError("No active scene found.");
            return false;
        }

        return true;
    }

    private static GameObject FindOrCreate(string objectName)
    {
        GameObject go = GameObject.Find(objectName);
        if (go != null)
        {
            return go;
        }

        go = new GameObject(objectName);
        Undo.RegisterCreatedObjectUndo(go, $"Create {objectName}");
        return go;
    }

    private static Transform CreatePanel(Transform parent, string name, Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot, Vector2 offsetMin, Vector2 offsetMax, Color color)
    {
        Transform existing = parent.Find(name);
        if (existing != null)
        {
            return existing;
        }

        GameObject go = new GameObject(name, typeof(RectTransform), typeof(Image));
        Undo.RegisterCreatedObjectUndo(go, $"Create {name}");
        go.transform.SetParent(parent, false);

        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
        rt.pivot = pivot;
        rt.offsetMin = offsetMin;
        rt.offsetMax = offsetMax;

        Image img = go.GetComponent<Image>();
        img.color = color;
        return go.transform;
    }

    private static TMP_Text CreateLabel(Transform parent, string name, string textValue, float fontSize, TextAlignmentOptions alignment, Vector2 anchorMin, Vector2 anchorMax, Vector2 offsetMin, Vector2 offsetMax)
    {
        Transform existing = parent.Find(name);
        if (existing != null)
        {
            TMP_Text existingText = existing.GetComponent<TMP_Text>();
            if (existingText != null)
            {
                return existingText;
            }
        }

        GameObject go = new GameObject(name, typeof(RectTransform), typeof(TextMeshProUGUI));
        Undo.RegisterCreatedObjectUndo(go, $"Create {name}");
        go.transform.SetParent(parent, false);

        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
        rt.offsetMin = offsetMin;
        rt.offsetMax = offsetMax;

        TextMeshProUGUI tmp = go.GetComponent<TextMeshProUGUI>();
        tmp.text = textValue;
        tmp.fontSize = fontSize;
        tmp.alignment = alignment;
        tmp.color = Color.white;
        tmp.enableWordWrapping = true;
        return tmp;
    }

    private static Button CreateButton(Transform parent, string name, string label, Vector2 normalizedCenter, Vector2 size)
    {
        Transform existing = parent.Find(name);
        if (existing != null)
        {
            Button existingButton = existing.GetComponent<Button>();
            if (existingButton != null)
            {
                return existingButton;
            }
        }

        GameObject go = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Button));
        Undo.RegisterCreatedObjectUndo(go, $"Create {name}");
        go.transform.SetParent(parent, false);

        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = normalizedCenter;
        rt.anchorMax = normalizedCenter;
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.sizeDelta = size;
        rt.anchoredPosition = Vector2.zero;

        Image img = go.GetComponent<Image>();
        img.color = new Color(0.15f, 0.15f, 0.15f, 0.95f);

        Button button = go.GetComponent<Button>();
        ColorBlock cb = button.colors;
        cb.normalColor = new Color(0.15f, 0.15f, 0.15f, 0.95f);
        cb.highlightedColor = new Color(0.3f, 0.3f, 0.3f, 1f);
        cb.pressedColor = new Color(0.1f, 0.1f, 0.1f, 1f);
        cb.selectedColor = cb.highlightedColor;
        button.colors = cb;

        CreateLabel(go.transform, "Label", label, 28f, TextAlignmentOptions.Center, Vector2.zero, Vector2.one, new Vector2(8f, 8f), new Vector2(-8f, -8f));
        return button;
    }

    private static void SetSerializedReference(Object target, string fieldName, Object value)
    {
        SerializedObject so = new SerializedObject(target);
        SerializedProperty prop = so.FindProperty(fieldName);
        if (prop != null)
        {
            prop.objectReferenceValue = value;
            so.ApplyModifiedPropertiesWithoutUndo();
        }
    }
}
