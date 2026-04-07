using UnityEngine;

public class SlotTrigger : MonoBehaviour
{
    public Slot slot;

    private void OnTriggerEnter(Collider other)
    {
        Customer c = other.GetComponentInParent<Customer>();
        if (c == null) return;

        if (c.state != CustomerState.Moving) return;

        HandleArrival(c);
    }

    private void HandleArrival(Customer c)
    {
        slot.Enter(c);
        c.currentSlot = slot;

        Route current = c.currentRoute;

        if (current == null)
        {
            Debug.LogError("Customer has no currentRoute", c);
            return;
        }

        var nextRoutes = current.NextRoutes;

        if (nextRoutes == null || nextRoutes.Count == 0)
        {
            c.SetStandby();
            return;
        }

        foreach (var route in nextRoutes)
        {
            if (route == null) continue;

            Slot nextSlot = route.OwnerSlot;

            if (nextSlot != null && nextSlot.IsFree())
            {
                slot.Leave();

                nextSlot.Reserve();

                c.currentRoute = route;

                PositionManager.Instance.MoveCustomer(c, nextSlot);
                return;
            }
        }

        // no available path → stop
        c.SetStandby();
    }
}