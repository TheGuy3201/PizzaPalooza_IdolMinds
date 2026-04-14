using UnityEngine;

public class PP_DeliveryCounter : MonoBehaviour, PP_IInteractable
{
    [SerializeField] private PP_OrderManager orderManager;
    [SerializeField] private PP_HUDController hud;

    public void Interact(PP_PlayerInteractor interactor)
    {
        if (interactor == null || !interactor.HasHeldItem || orderManager == null)
        {
            return;
        }

        PP_PickupItem held = interactor.HeldItem;
        PP_Pizza pizza = held.GetComponent<PP_Pizza>();
        if (pizza == null || !pizza.IsDeliverable())
        {
            return;
        }

        bool wasDelivered = orderManager.TryDeliver(pizza, out _, out _, out string feedback, out bool isCorrectDelivery);

        if (wasDelivered)
        {
            interactor.ConsumeHeldItem();
            Destroy(held.gameObject);

            if (isCorrectDelivery)
            {
                AudioManager.Play("Ding");
            }
            else
            {
                AudioManager.Play("Error");
            }

            if (hud != null)
            {
                hud.SetLastFeedback(feedback);
            }
        }
        else if (feedback != "No order matched")
        {
            AudioManager.Play("Error");
            if (hud != null)
            {
                hud.SetLastFeedback(feedback);
            }
        }
    }
}
