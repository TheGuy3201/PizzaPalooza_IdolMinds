using UnityEngine;

public class PP_OvenStation : MonoBehaviour, PP_IInteractable
{
    [SerializeField] private Transform ovenSlot;
    [SerializeField] private float cookSeconds = 12f;
    [SerializeField] private float burnAfterCookedSeconds = 6f;

    private PP_Pizza pizzaInOven;
    private float ovenTimer;

    private void Update()
    {
        if (pizzaInOven == null)
        {
            return;
        }

        ovenTimer += Time.deltaTime;

        if (pizzaInOven.State == PP_Pizza.CookState.Raw && ovenTimer >= cookSeconds)
        {
            pizzaInOven.SetCooked();
            AudioManager.Play("OvenFinish");
        }

        if (pizzaInOven.State == PP_Pizza.CookState.Cooked && ovenTimer >= cookSeconds + burnAfterCookedSeconds)
        {
            pizzaInOven.SetBurnt();
        }
    }

    public void Interact(PP_PlayerInteractor interactor)
    {
        if (interactor == null)
        {
            return;
        }

        if (interactor.HasHeldItem)
        {
            TryInsertPizza(interactor);
            return;
        }

        TryTakePizza(interactor);
    }

    private void TryInsertPizza(PP_PlayerInteractor interactor)
    {
        if (pizzaInOven != null)
        {
            return;
        }

        PP_PickupItem held = interactor.HeldItem;
        PP_Pizza heldPizza = held != null ? held.GetComponent<PP_Pizza>() : null;
        if (heldPizza == null || !heldPizza.IsReadyForOven() || heldPizza.State != PP_Pizza.CookState.Raw)
        {
            return;
        }

        interactor.ConsumeHeldItem();
        pizzaInOven = heldPizza;
        pizzaInOven.transform.SetParent(ovenSlot);
        pizzaInOven.transform.localPosition = Vector3.zero;
        pizzaInOven.transform.localRotation = Quaternion.identity;
        pizzaInOven.GetComponent<PP_PickupItem>().SetPhysicsState(true);
        ovenTimer = 0f;
    }

    private void TryTakePizza(PP_PlayerInteractor interactor)
    {
        if (pizzaInOven == null)
        {
            return;
        }

        PP_PickupItem pickup = pizzaInOven.GetComponent<PP_PickupItem>();
        pizzaInOven = null;
        pickup.SetPhysicsState(false);
        interactor.PickUp(pickup);
    }
}
