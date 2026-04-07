using UnityEngine;
using System;
using System.Collections.Generic;

public enum RouteType
{
    Wait,
    Order,
    End
}

public class Route : MonoBehaviour
{
    public RouteType type;

    [Header("Graph")]
    [SerializeField] private Slot ownerSlot;
    [SerializeField] private List<Route> nextRoutes;
    [SerializeField] private Route exitRoute;

    private Action<Customer, PositionManager> execute;

    private void Awake()
    {
        switch (type)
        {
            case RouteType.Wait:
                execute = ExecuteWait;
                break;

            case RouteType.Order:
                execute = ExecuteOrder;
                break;

            case RouteType.End:
                execute = ExecuteEnd;
                break;
        }
    }

    public void Execute(Customer c, PositionManager m)
    {
        if (execute == null)
        {
            Debug.LogError($"Route {name} has no execution assigned", this);
            return;
        }

        execute.Invoke(c, m);
    }

    // ------------------------
    // Behaviors
    // ------------------------

    private void ExecuteWait(Customer c, PositionManager m)
    {
        if (nextRoutes == null || nextRoutes.Count == 0)
            return;

        foreach (var route in nextRoutes)
        {
            if (route == null) continue;

            Slot target = route.ownerSlot;

            if (target == null)
            {
                Debug.LogError("Route has NULL ownerSlot", this);
                continue;
            }

            if (target.IsFree())
            {
                target.Reserve();

                c.currentRoute = route;
                m.MoveCustomer(c, target);
                return;
            }
        }

        c.SetStandby();
    }

    private void ExecuteOrder(Customer c, PositionManager m)
    {
        m.customerOrderManager.RequestOrder(c);
        c.SetOrdering();
    }

    private void ExecuteEnd(Customer c, PositionManager m)
    {
        m.SendToExit(c, exitRoute);
    }

    // ------------------------
    // Exposed
    // ------------------------

    public Slot OwnerSlot => ownerSlot;
    public List<Route> NextRoutes => nextRoutes;
    public Route ExitRoute => exitRoute;
}