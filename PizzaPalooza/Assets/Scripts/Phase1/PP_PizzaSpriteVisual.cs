using UnityEngine;

public class PP_PizzaSpriteVisual : MonoBehaviour
{
    [Header("Layer Renderers")]
    [SerializeField] private SpriteRenderer doughLayer;
    [SerializeField] private SpriteRenderer sauceLayer;
    [SerializeField] private SpriteRenderer cheeseLayer;
    [SerializeField] private SpriteRenderer pepperoniLayer;
    [SerializeField] private SpriteRenderer mushroomLayer;
    [SerializeField] private SpriteRenderer olivesLayer; // Optional future layer
    [SerializeField] private SpriteRenderer peppersLayer; // Optional future layer
    [SerializeField] private SpriteRenderer onionLayer; // Optional future layer

    [Header("Cook Tint")]
    [SerializeField] private Color rawTint = Color.white;
    [SerializeField] private Color cookedTint = new Color(0.9f, 0.78f, 0.6f, 1f);
    [SerializeField] private Color burntTint = new Color(0.35f, 0.25f, 0.18f, 1f);

    private void Awake()
    {
        // At runtime, bind layers AND initialize visibility state
        BindLayerReferences();
        ResetLayerVisibility();
    }

    private void OnValidate()
    {
        // Only bind layers, don't change visibility (this runs in editor)
        BindLayerReferences();
    }

    private void Reset()
    {
        BindLayerReferences();
        ApplyTint(rawTint);
    }

    public void ApplyIngredient(PP_IngredientType ingredient)
    {
        Debug.Log($"[PP_PizzaSpriteVisual.ApplyIngredient] Applying {ingredient}");
        switch (ingredient)
        {
            case PP_IngredientType.Dough:
                Debug.Log($"[PP_PizzaSpriteVisual] Dough: doughLayer={doughLayer != null}");
                SetLayerEnabled(doughLayer, true, "dough");
                break;
            case PP_IngredientType.Sauce:
                Debug.Log($"[PP_PizzaSpriteVisual] Sauce: sauceLayer={sauceLayer != null}");
                SetLayerEnabled(sauceLayer, true, "sauce");
                break;
            case PP_IngredientType.Cheese:
                Debug.Log($"[PP_PizzaSpriteVisual] Cheese: cheeseLayer={cheeseLayer != null}");
                SetLayerEnabled(cheeseLayer, true, "cheese");
                break;
            case PP_IngredientType.Pepperoni:
                Debug.Log($"[PP_PizzaSpriteVisual] Pepperoni: pepperoniLayer={pepperoniLayer != null}");
                SetLayerEnabled(pepperoniLayer, true, "pepperoni");
                break;
            case PP_IngredientType.Mushroom:
                Debug.Log($"[PP_PizzaSpriteVisual] Mushroom: mushroomLayer={mushroomLayer != null}");
                SetLayerEnabled(mushroomLayer, true, "mushroom");
                break;
            case PP_IngredientType.Olives:
                Debug.Log($"[PP_PizzaSpriteVisual] Olives: olivesLayer={olivesLayer != null}");
                SetLayerEnabled(olivesLayer, true, "olives");
                break;
            case PP_IngredientType.Peppers:
                Debug.Log($"[PP_PizzaSpriteVisual] Peppers: peppersLayer={peppersLayer != null}");
                SetLayerEnabled(peppersLayer, true, "peppers");
                break;
            case PP_IngredientType.Onions:
                Debug.Log($"[PP_PizzaSpriteVisual] Onions: onionLayer={onionLayer != null}");
                SetLayerEnabled(onionLayer, true, "onions");
                break;
        }
    }

    public void ApplyCookState(PP_Pizza.CookState state)
    {
        switch (state)
        {
            case PP_Pizza.CookState.Raw:
                ApplyTint(rawTint);
                break;
            case PP_Pizza.CookState.Cooked:
                ApplyTint(cookedTint);
                break;
            case PP_Pizza.CookState.Burnt:
                ApplyTint(burntTint);
                break;
        }
    }

    private void ApplyTint(Color tint)
    {
        ApplyTint(doughLayer, tint);
        ApplyTint(sauceLayer, tint);
        ApplyTint(cheeseLayer, tint);
        ApplyTint(pepperoniLayer, tint);
        ApplyTint(mushroomLayer, tint);
        ApplyTint(olivesLayer, tint);
        ApplyTint(peppersLayer, tint);
        ApplyTint(onionLayer, tint);
    }

    private static void ApplyTint(SpriteRenderer layer, Color tint)
    {
        if (layer != null)
        {
            layer.color = tint;
        }
    }

    private static void SetLayerEnabled(SpriteRenderer layer, bool enabled, string layerName = "unknown")
    {
        if (layer != null)
        {
            layer.gameObject.SetActive(enabled);
            Debug.Log($"[PP_PizzaSpriteVisual.SetLayerEnabled] {layerName} layer (GameObject: {layer.gameObject.name}) set active={enabled}");
        }
        else
        {
            Debug.LogWarning($"[PP_PizzaSpriteVisual.SetLayerEnabled] {layerName} layer is NULL, cannot set active state!");
        }
    }

    /// <summary>
    /// Finds and caches references to all layer SpriteRenderers.
    /// Safe to call from editor (OnValidate, Reset) and runtime.
    /// </summary>
    public void BindLayerReferences()
    {
        doughLayer = FindLayer("Base_Dough", "PizzaBase", "Layer_Dough");
        sauceLayer = FindLayer("Layer_Sauce", "Sauce", "SauceLayer");
        cheeseLayer = FindLayer("Layer_Cheese", "Cheese", "CheeseLayer");
        pepperoniLayer = FindLayer("Layer_Pepperoni", "Pepperoni", "PepperoniLayer");
        mushroomLayer = FindLayer("Layer_Mushroom", "Mushroom", "MushroomLayer");
        olivesLayer = FindLayer("Layer_Olives", "Olives", "OlivesLayer");
        peppersLayer = FindLayer("Layer_Peppers", "Peppers", "PeppersLayer");
        onionLayer = FindLayer("Layer_Onion", "Layer_Onions", "Onions", "OnionsLayer");

        Debug.Log($"[PP_PizzaSpriteVisual] Rebound layers: dough={doughLayer != null}, sauce={sauceLayer != null}, cheese={cheeseLayer != null}, pepperoni={pepperoniLayer != null}, mushroom={mushroomLayer != null}, olives={olivesLayer != null}, peppers={peppersLayer != null}, onions={onionLayer != null}");
    }

    /// <summary>
    /// Resets layer visibility to initial state: dough=enabled, all toppings=disabled.
    /// Should only be called at runtime (Awake or when reusing a pizza).
    /// </summary>
    private void ResetLayerVisibility()
    {
        Debug.Log($"[PP_PizzaSpriteVisual.ResetLayerVisibility] Enabling dough, disabling toppings");
        SetLayerEnabled(doughLayer, true, "dough");
        SetLayerEnabled(sauceLayer, false, "sauce");
        SetLayerEnabled(cheeseLayer, false, "cheese");
        SetLayerEnabled(pepperoniLayer, false, "pepperoni");
        SetLayerEnabled(mushroomLayer, false, "mushroom");
        SetLayerEnabled(olivesLayer, false, "olives");
        SetLayerEnabled(peppersLayer, false, "peppers");
        SetLayerEnabled(onionLayer, false, "onions");
    }

    /// <summary>
    /// Complete rebind: finds layers and resets visibility.
    /// Only use at runtime (Awake). Don't use in editor or PreparePizzaForGameplay.
    /// </summary>
    public void AutoBindLayers()
    {
        BindLayerReferences();
        ResetLayerVisibility();
    }

    private SpriteRenderer FindLayer(params string[] names)
    {
        // Try direct transform.Find first
        for (int i = 0; i < names.Length; i++)
        {
            Transform candidate = transform.Find(names[i]);
            if (candidate != null)
            {
                SpriteRenderer renderer = candidate.GetComponent<SpriteRenderer>();
                if (renderer != null)
                {
                    Debug.Log($"[PP_PizzaSpriteVisual.FindLayer] Found layer via transform.Find('{names[i]}')");
                    return renderer;
                }
            }
        }

        // Try searching all children by name
        SpriteRenderer[] renderers = GetComponentsInChildren<SpriteRenderer>(true);
        Debug.Log($"[PP_PizzaSpriteVisual.FindLayer] Searching {renderers.Length} SpriteRenderers for names: {string.Join(", ", names)}");
        
        for (int i = 0; i < renderers.Length; i++)
        {
            string rendererName = renderers[i].gameObject.name;
            for (int j = 0; j < names.Length; j++)
            {
                if (rendererName == names[j])
                {
                    Debug.Log($"[PP_PizzaSpriteVisual.FindLayer] Found layer '{rendererName}' in children");
                    return renderers[i];
                }
            }
        }

        Debug.LogWarning($"[PP_PizzaSpriteVisual.FindLayer] NOT FOUND - searched for: {string.Join(", ", names)}");
        return null;
    }
}
