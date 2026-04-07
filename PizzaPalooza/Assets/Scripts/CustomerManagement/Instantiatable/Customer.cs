using System;
using UnityEngine;
using UnityEngine.AI;

public enum CustomerState
{
    Moving,
    StandBy,
    Ordering
}

[RequireComponent(typeof(NavMeshAgent))]
public class Customer : MonoBehaviour
{
    public int patience = 100;

    public CustomerState state;

    public Slot currentSlot;
    public Route currentRoute;

    private NavMeshAgent agent;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    public void MoveTo(Slot slot)
    {
        state = CustomerState.Moving;
        agent.SetDestination(slot.point.position);
    }

    public bool HasArrived()
    {
        return !agent.pathPending && agent.remainingDistance <= 0.01f;
    }

    public void SnapToSlot(Slot slot)
    {
        transform.position = slot.point.position;
    }

    public void SetStandby()
    {
        state = CustomerState.StandBy;
    }

    public void SetOrdering()
    {
        state = CustomerState.Ordering;
    }

    public void DecreasePatience(int value)
    {
        patience -= value;
    }
}