using UnityEngine;
using System;

public class CustomerOrderManager : MonoBehaviour
{
    public event Action<Customer> OnOrderSatisfied;

    public void RequestOrder(Customer c)
    {
        Debug.Log("Order requested: " + c.name);
    }

    public void CompleteOrder(Customer c)
    {
        OnOrderSatisfied?.Invoke(c);
    }
}

