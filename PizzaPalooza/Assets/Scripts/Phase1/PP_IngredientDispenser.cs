using UnityEngine;

public class PP_IngredientDispenser : MonoBehaviour, PP_IInteractable
{
    [SerializeField] private PP_PickupItem ingredientPickupPrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private float cooldownSeconds = 0.2f;

    private float cooldownTimer;

    private void Update()
    {
        if (cooldownTimer > 0f)
        {
            cooldownTimer -= Time.deltaTime;
        }
    }

    public void Interact(PP_PlayerInteractor interactor)
    {
        if (interactor == null || interactor.HasHeldItem || ingredientPickupPrefab == null || cooldownTimer > 0f)
        {
            return;
        }

        Vector3 position = spawnPoint != null ? spawnPoint.position : transform.position + transform.forward * 0.6f;
        Quaternion rotation = spawnPoint != null ? spawnPoint.rotation : Quaternion.identity;
        PP_PickupItem newItem = Instantiate(ingredientPickupPrefab, position, rotation);
        interactor.PickUp(newItem);
        cooldownTimer = cooldownSeconds;
    }
}
