using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionManager : MonoBehaviour
{
    public static PositionManager Instance;

    public CustomerOrderManager customerOrderManager;

    private List<Customer> activeCustomers = new List<Customer>();

    private void Awake()
    {
        Instance = this;
    }

    public void RegisterCustomer(Customer c)
    {
        activeCustomers.Add(c);

        CheckAll(); // start flow
    }

    public void CheckAll()
    {
        foreach (var c in activeCustomers)
        {
            if (c.state != CustomerState.Moving)
            {
                if (c.patience <= 0)
                    SendToExit(c, c.currentRoute.ExitRoute);
                else
                    c.currentRoute.Execute(c, this);
            }
        }
    }

    public void MoveCustomer(Customer c, Slot target)
    {
        Slot origin = c.currentSlot;

        if (origin != null)
            origin.Leave();

        c.MoveTo(target);
    }

    public void SendToExit(Customer c, Route exitRoute)
    {
        if (exitRoute == null)
        {
            Debug.LogError("ExitRoute is NULL", this);
            return;
        }

        c.currentRoute = exitRoute;
    }
}