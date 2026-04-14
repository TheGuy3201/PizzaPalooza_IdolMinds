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
            spriteVisual?.ApplyIngredient(ingredientType);
        }

        return added;
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
            spriteVisual?.ApplyCookState(cookState);
        }
    }

    public void SetBurnt()
    {
        cookState = CookState.Burnt;
        spriteVisual?.ApplyCookState(cookState);
    }
}
