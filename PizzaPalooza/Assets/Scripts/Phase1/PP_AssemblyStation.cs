using UnityEngine;
using System.Collections.Generic;

public class PP_AssemblyStation : MonoBehaviour, PP_IInteractable
{
    [SerializeField] private PP_Pizza pizzaPrefab;
    [SerializeField] private Transform stationSlot;
    [SerializeField] private bool enableDebugLogs;

    private PP_Pizza stationPizza;
    
    // Track all active station pizzas for debugging
    private static List<PP_Pizza> activePizzas = new List<PP_Pizza>();

    private void Awake()
    {
        EnsureStationSlot();
    }

    public void Interact(PP_PlayerInteractor interactor)
    {
        if (interactor == null)
        {
            LogDebug("Interact ignored because interactor is null.");
            return;
        }

        LogDebug($"Interact called. heldItem={(interactor.HeldItem != null ? interactor.HeldItem.name : "none")}, stationPizza={(stationPizza != null ? stationPizza.name : "none")}");

        if (interactor.HasHeldItem)
        {
            TryPlaceOrApplyHeldItem(interactor);
            return;
        }

        LogDebug("Interact did nothing because station is empty and player is not holding anything. (Use R key to pick up pizza)");
    }

    public void TryPickUpPizza(PP_PlayerInteractor interactor)
    {
        if (interactor == null)
        {
            LogDebug("TryPickUpPizza ignored because interactor is null.");
            return;
        }

        if (stationPizza == null)
        {
            LogDebug("TryPickUpPizza ignored because station has no pizza.");
            return;
        }

        if (interactor.HasHeldItem)
        {
            LogDebug("TryPickUpPizza ignored because player is already holding something.");
            return;
        }

        LogDebug($"Picking up pizza '{stationPizza.name}'.");
        PP_PickupItem pickup = stationPizza.GetComponent<PP_PickupItem>();
        activePizzas.Remove(stationPizza);
        LogDebug($"[PIZZA TRACKER] Unregistered pizza. Total active: {activePizzas.Count}");
        stationPizza = null;
        pickup.SetPhysicsState(false);
        interactor.PickUp(pickup);
        LogDebug($"Returned pizza '{pickup.name}' to player.");
    }

    private void TryPlaceOrApplyHeldItem(PP_PlayerInteractor interactor)
    {
        EnsureStationSlot();

        PP_PickupItem held = interactor.HeldItem;
        if (held == null)
        {
            LogDebug("TryPlaceOrApplyHeldItem aborted because held item is null.");
            return;
        }

        PP_Pizza heldPizza = held.GetComponent<PP_Pizza>();
        if (heldPizza != null)
        {
            if (stationPizza != null)
            {
                LogDebug($"Cannot place held pizza '{heldPizza.name}' because station already has '{stationPizza.name}'.");
                return;
            }

            interactor.ConsumeHeldItem();
            stationPizza = heldPizza;
            stationPizza.transform.SetParent(stationSlot);
            stationPizza.transform.localPosition = Vector3.zero;
            stationPizza.transform.localRotation = Quaternion.identity;
            heldPizza.GetComponent<PP_PickupItem>().SetPhysicsState(true);
            PreparePizzaForGameplay(stationPizza);
            LogDebug($"Placed existing pizza '{heldPizza.name}' on station.");
            return;
        }

        PP_IngredientItem ingredient = held.GetComponent<PP_IngredientItem>();
        if (ingredient == null)
        {
            LogDebug($"Held item '{held.name}' is not an ingredient and not a pizza.");
            return;
        }

        LogDebug($"Trying ingredient '{ingredient.IngredientType}'. pizzaPrefab={(pizzaPrefab != null ? pizzaPrefab.name : "null")}, stationPizza={(stationPizza != null ? stationPizza.name : "none")}");

        if (stationPizza == null)
        {
            if (pizzaPrefab == null)
            {
                LogDebug($"Cannot create pizza. ingredient={ingredient.IngredientType}, pizzaPrefabMissing=True");
                return;
            }

            stationPizza = Instantiate(pizzaPrefab, stationSlot.position, stationSlot.rotation, stationSlot);
            PreparePizzaForGameplay(stationPizza);
            
            // Track this pizza for debugging
            activePizzas.Add(stationPizza);
            LogDebug($"[PIZZA TRACKER] Registered pizza. Total active: {activePizzas.Count}");
            
            LogDebug($"Created new station pizza '{stationPizza.name}'.");
        }

        EnsurePrerequisitesForIngredient(stationPizza, ingredient.IngredientType);

        if (stationPizza.AddIngredient(ingredient.IngredientType))
        {
            PP_PickupItem usedItem = interactor.ConsumeHeldItem();
            if (usedItem != null)
            {
                Destroy(usedItem.gameObject);
            }

            AudioManager.Play("Clunk");

            LogDebug($"Applied ingredient '{ingredient.IngredientType}' successfully.");
        }
        else
        {
            LogDebug($"Failed to apply ingredient '{ingredient.IngredientType}'. hasDough={stationPizza.HasDough}, hasSauce={stationPizza.HasSauce}, state={stationPizza.State}");
        }
    }

    private void EnsurePrerequisitesForIngredient(PP_Pizza pizza, PP_IngredientType ingredientType)
    {
        if (pizza == null)
        {
            return;
        }

        if (ingredientType == PP_IngredientType.Sauce)
        {
            if (!pizza.HasDough && pizza.AddIngredient(PP_IngredientType.Dough))
            {
                LogDebug("Auto-added Dough prerequisite for Sauce.");
            }

            return;
        }

        if (ingredientType == PP_IngredientType.Cheese || ingredientType == PP_IngredientType.Pepperoni || ingredientType == PP_IngredientType.Mushroom || ingredientType == PP_IngredientType.Olives || ingredientType == PP_IngredientType.Peppers || ingredientType == PP_IngredientType.Onions)
        {
            if (!pizza.HasDough && pizza.AddIngredient(PP_IngredientType.Dough))
            {
                LogDebug("Auto-added Dough prerequisite for topping.");
            }

            if (!pizza.HasSauce && pizza.AddIngredient(PP_IngredientType.Sauce))
            {
                LogDebug("Auto-added Sauce prerequisite for topping.");
            }
        }
    }

    private void EnsureStationSlot()
    {
        if (stationSlot != null)
        {
            return;
        }

        Transform existing = transform.Find("StationSlot");
        if (existing != null)
        {
            stationSlot = existing;
            return;
        }

        GameObject slot = new GameObject("StationSlot");
        slot.transform.SetParent(transform, false);
        slot.transform.localPosition = new Vector3(0f, 0.5f, 0f);
        stationSlot = slot.transform;
        LogDebug("Created missing StationSlot automatically.");
    }

    private void PreparePizzaForGameplay(PP_Pizza pizza)
    {
        if (pizza == null)
        {
            return;
        }

        PP_PickupItem pickup = pizza.GetComponent<PP_PickupItem>();
        if (pickup == null)
        {
            pickup = pizza.gameObject.AddComponent<PP_PickupItem>();
            LogDebug("Added missing PP_PickupItem to pizza instance.");
        }

        pickup.SetPhysicsState(true);

        PP_PizzaSpriteVisual visual = pizza.GetComponentInChildren<PP_PizzaSpriteVisual>();
        if (visual == null)
        {
            visual = pizza.gameObject.AddComponent<PP_PizzaSpriteVisual>();
            LogDebug("Added missing PP_PizzaSpriteVisual to pizza instance.");
        }

        // Just rebind layer references, don't reset visibility (Awake already did that)
        visual.BindLayerReferences();
    }

    private void LogDebug(string message)
    {
        if (!enableDebugLogs)
        {
            return;
        }

        Debug.Log($"[PP_AssemblyStation] {message}", this);
    }
}
