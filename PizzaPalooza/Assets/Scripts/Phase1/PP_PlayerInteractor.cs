using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class PP_PlayerInteractor : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Transform holdPoint;

    [Header("Interaction")]
    [SerializeField] private float interactRange = 3f;
    [SerializeField] private float interactRadius = 0.85f;
    [SerializeField] private LayerMask interactMask = ~0;
    [SerializeField] private bool enableDebugLogs;

    public PP_PickupItem HeldItem { get; private set; }
    public bool HasHeldItem => HeldItem != null;

    private void Awake()
    {
        EnsureReferences();
    }

    private void OnValidate()
    {
        EnsureReferences();
    }

    private void Update()
    {
        if (Keyboard.current == null)
        {
            return;
        }

        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            TryInteract();
        }

        if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            TryPickUpPizza();
        }

        if (Keyboard.current.gKey.wasPressedThisFrame)
        {
            DropHeldItem();
        }
    }

    public void PickUp(PP_PickupItem item)
    {
        if (item == null || HasHeldItem)
        {
            LogDebug($"PickUp blocked. item={(item != null ? item.name : "null")}, hasHeldItem={HasHeldItem}");
            return;
        }

        EnsureReferences();
        Transform targetHoldPoint = holdPoint != null ? holdPoint : (playerCamera != null ? playerCamera.transform : null);
        if (targetHoldPoint == null)
        {
            LogDebug("PickUp blocked. No hold point or player camera found.");
            return;
        }

        HeldItem = item;
        HeldItem.OnPickedUp(targetHoldPoint);
        LogDebug($"Picked up '{item.name}' and attached to '{targetHoldPoint.name}'.");
    }

    public PP_PickupItem ConsumeHeldItem()
    {
        PP_PickupItem item = HeldItem;
        if (item != null)
        {
            item.OnReleasedFromHold();
        }

        HeldItem = null;
        return item;
    }

    public void DropHeldItem()
    {
        if (!HasHeldItem)
        {
            LogDebug("DropHeldItem ignored because nothing is held.");
            return;
        }

        LogDebug($"Dropped '{HeldItem.name}'.");
        HeldItem.OnDropped();
        HeldItem = null;
    }

    private void TryInteract()
    {
        EnsureReferences();

        if (playerCamera == null)
        {
            LogDebug("TryInteract aborted. playerCamera is null.");
            return;
        }

        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit[] hits = Physics.RaycastAll(ray, interactRange, interactMask, QueryTriggerInteraction.Ignore);
        if (hits == null || hits.Length == 0)
        {
            LogDebug($"No raycast hits. range={interactRange:0.00}, heldItem={(HeldItem != null ? HeldItem.name : "none")}");
            TryInteractNearby();
            return;
        }

        Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));
        LogDebug($"Raycast found {hits.Length} hit(s). First hit='{hits[0].collider.name}' distance={hits[0].distance:0.00}");

        for (int i = 0; i < hits.Length; i++)
        {
            Collider hitCollider = hits[i].collider;
            if (hitCollider == null || ShouldIgnoreCollider(hitCollider))
            {
                if (hitCollider != null)
                {
                    LogDebug($"Ignoring hit '{hitCollider.name}'.");
                }
                continue;
            }

            PP_IInteractable interactable = hitCollider.GetComponentInParent<PP_IInteractable>();
            if (interactable != null)
            {
                LogDebug($"Interacting via raycast with '{hitCollider.name}' on '{((Component)interactable).gameObject.name}'.");
                interactable.Interact(this);
                return;
            }
        }

        LogDebug("No valid interactable found in raycast hits. Falling back to nearby search.");
        TryInteractNearby();
    }

    private void TryInteractNearby()
    {
        if (playerCamera == null)
        {
            LogDebug("TryInteractNearby aborted. playerCamera is null.");
            return;
        }

        Vector3 searchCenter = playerCamera.transform.position + playerCamera.transform.forward * Mathf.Min(interactRange, 2f);
        Collider[] colliders = Physics.OverlapSphere(searchCenter, interactRadius, interactMask, QueryTriggerInteraction.Ignore);
        float bestDistance = float.MaxValue;
        PP_IInteractable bestInteractable = null;

        for (int i = 0; i < colliders.Length; i++)
        {
            Collider candidate = colliders[i];
            if (candidate == null || ShouldIgnoreCollider(candidate))
            {
                continue;
            }

            PP_IInteractable interactable = candidate.GetComponentInParent<PP_IInteractable>();
            if (interactable == null)
            {
                continue;
            }

            float distance = Vector3.Distance(playerCamera.transform.position, candidate.ClosestPoint(playerCamera.transform.position));
            if (distance < bestDistance)
            {
                bestDistance = distance;
                bestInteractable = interactable;
            }
        }

        if (bestInteractable != null)
        {
            LogDebug($"Interacting via nearby search with '{((Component)bestInteractable).gameObject.name}' at distance {bestDistance:0.00}.");
            bestInteractable.Interact(this);
            return;
        }

        LogDebug($"Nearby search found no interactable. radius={interactRadius:0.00}, colliders={colliders.Length}");
    }

    private void TryPickUpPizza()
    {
        EnsureReferences();

        if (playerCamera == null)
        {
            LogDebug("TryPickUpPizza aborted. playerCamera is null.");
            return;
        }

        Vector3 searchCenter = playerCamera.transform.position + playerCamera.transform.forward * Mathf.Min(interactRange, 2f);
        Collider[] colliders = Physics.OverlapSphere(searchCenter, interactRadius, interactMask, QueryTriggerInteraction.Ignore);
        float bestDistance = float.MaxValue;
        PP_AssemblyStation bestStation = null;

        for (int i = 0; i < colliders.Length; i++)
        {
            Collider candidate = colliders[i];
            if (candidate == null || ShouldIgnoreCollider(candidate))
            {
                continue;
            }

            PP_AssemblyStation station = candidate.GetComponentInParent<PP_AssemblyStation>();
            if (station == null)
            {
                continue;
            }

            float distance = Vector3.Distance(playerCamera.transform.position, candidate.ClosestPoint(playerCamera.transform.position));
            if (distance < bestDistance)
            {
                bestDistance = distance;
                bestStation = station;
            }
        }

        if (bestStation != null)
        {
            LogDebug($"Picking up pizza from assembly station at distance {bestDistance:0.00}.");
            bestStation.TryPickUpPizza(this);
            return;
        }

        LogDebug($"No assembly station found nearby for pickup. radius={interactRadius:0.00}");
    }

    private bool ShouldIgnoreCollider(Collider hitCollider)
    {
        if (hitCollider.transform.IsChildOf(transform))
        {
            return true;
        }

        if (HeldItem != null && hitCollider.transform.IsChildOf(HeldItem.transform))
        {
            return true;
        }

        return false;
    }

    private void EnsureReferences()
    {
        if (playerCamera == null)
        {
            playerCamera = GetComponentInChildren<Camera>();
        }

        if (holdPoint == null && playerCamera != null)
        {
            Transform existing = playerCamera.transform.Find("HoldPoint");
            if (existing != null)
            {
                holdPoint = existing;
            }
            else
            {
                GameObject hp = new GameObject("HoldPoint");
                hp.transform.SetParent(playerCamera.transform, false);
                hp.transform.localPosition = new Vector3(0f, -0.2f, 0.7f);
                hp.transform.localRotation = Quaternion.identity;
                holdPoint = hp.transform;
            }
        }
    }

    private void LogDebug(string message)
    {
        if (!enableDebugLogs)
        {
            return;
        }

        Debug.Log($"[PP_PlayerInteractor] {message}", this);
    }
}
