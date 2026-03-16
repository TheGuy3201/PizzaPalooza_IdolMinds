using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PP_PickupItem))]
public class PP_Pizza : MonoBehaviour
{
    public enum CookState
    {
        Raw,
        Cooked,
        Burnt
    }

    private readonly HashSet<PP_IngredientType> toppings = new HashSet<PP_IngredientType>();

    [SerializeField] private bool hasDough;
    [SerializeField] private bool hasSauce;
    [SerializeField] private CookState cookState = CookState.Raw;
    [SerializeField] private PP_PizzaSpriteVisual spriteVisual;

    public bool HasDough => hasDough;
    public bool HasSauce => hasSauce;
    public CookState State => cookState;
    public IReadOnlyCollection<PP_IngredientType> Toppings => toppings;

    private void Awake()
    {
        if (spriteVisual == null)
        {
            spriteVisual = GetComponentInChildren<PP_PizzaSpriteVisual>();
        }

        // Don't apply cook state at spawn - layers start raw with white tint
        // Only apply tint when actually cooked
    }

    public bool AddIngredient(PP_IngredientType ingredientType)
    {
        // Ensure spriteVisual is cached
        if (spriteVisual == null)
        {
            spriteVisual = GetComponentInChildren<PP_PizzaSpriteVisual>();
            Debug.Log($"[PP_Pizza.AddIngredient] Re-cached spriteVisual: {(spriteVisual != null ? "found" : "NULL")}");
        }

        if (ingredientType == PP_IngredientType.Dough)
        {
            if (hasDough)
            {
                return false;
            }

            hasDough = true;
            spriteVisual?.ApplyIngredient(ingredientType);
            return true;
        }

        if (!hasDough)
        {
            return false;
        }

        if (ingredientType == PP_IngredientType.Sauce)
        {
            if (hasSauce)
            {
                return false;
            }

            hasSauce = true;
            spriteVisual?.ApplyIngredient(ingredientType);
            return true;
        }

        if (!hasSauce)
        {
            return false;
        }

        bool added = toppings.Add(ingredientType);
        if (added)
        {
            Debug.Log($"[PP_Pizza.AddIngredient] Applying {ingredientType}, spriteVisual={spriteVisual != null}");
            spriteVisual?.ApplyIngredient(ingredientType);
            LogPizzaState();
        }

        return added;
    }

    private void LogPizzaState()
    {
        string toppingsList = toppings.Count > 0 ? string.Join(", ", toppings) : "none";
        Debug.Log($"[PP_Pizza] Current state: Dough={hasDough}, Sauce={hasSauce}, Toppings=[{toppingsList}]");
    }

    public bool IsReadyForOven()
    {
        return hasDough && hasSauce;
    }

    public bool IsDeliverable()
    {
        return cookState == CookState.Cooked || cookState == CookState.Burnt;
    }

    public void SetCooked()
    {
        if (cookState == CookState.Raw)
        {
            cookState = CookState.Cooked;
            Debug.Log($"[PP_Pizza] Pizza now COOKED! Ingredients: Dough={hasDough}, Sauce={hasSauce}, Toppings=[{string.Join(", ", toppings)}]");
            spriteVisual?.ApplyCookState(cookState);
        }
    }

    public void SetBurnt()
    {
        cookState = CookState.Burnt;
        Debug.Log($"[PP_Pizza] Pizza now BURNT! Ingredients: Dough={hasDough}, Sauce={hasSauce}, Toppings=[{string.Join(", ", toppings)}]");
        spriteVisual?.ApplyCookState(cookState);
    }
}
