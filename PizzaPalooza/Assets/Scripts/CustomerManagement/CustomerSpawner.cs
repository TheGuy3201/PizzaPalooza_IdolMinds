using UnityEngine;

public class CustomerSpawner : MonoBehaviour
{
    public GameObject[] prefabs;
    public Transform spawnPoint;
    public float interval = 3f;

    public Route firstSlot;

    private bool canSpawn = true;

    private void Start()
    {
        InvokeRepeating(nameof(TrySpawn), 0f, interval);
    }

    void TrySpawn()
    {
        if (!canSpawn) return;

        GameObject obj = Instantiate(prefabs[Random.Range(0, prefabs.Length)], spawnPoint.position, Quaternion.identity);

        Customer c = obj.GetComponent<Customer>();
        c.currentRoute = firstSlot; // 🔥 REQUIRED

        PositionManager.Instance.RegisterCustomer(c);

        if (firstSlot.OwnerSlot.IsFree())
        {
            firstSlot.OwnerSlot.Reserve();
            PositionManager.Instance.MoveCustomer(c, firstSlot.OwnerSlot);
        }
        else
        {
            // stays at spawn
        }

        canSpawn = false;
    }

    public void OnSpawnFreed()
    {
        canSpawn = true;
    }
}