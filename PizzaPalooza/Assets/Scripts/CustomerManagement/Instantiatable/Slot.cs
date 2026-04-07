using UnityEngine;

public class Slot : MonoBehaviour
{
    public Transform point;

    public Customer occupant;
    public bool isOccupied;
    public bool isReserved;

    public bool IsFree() => !isOccupied && !isReserved;

    public void Reserve() => isReserved = true;

    public void Enter(Customer customer)
    {
        occupant = customer;
        isOccupied = true;
        isReserved = false;
    }

    public void Leave()
    {
        occupant = null;
        isOccupied = false;
    }
}