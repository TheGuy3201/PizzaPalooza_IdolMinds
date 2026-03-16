using UnityEngine;

[RequireComponent(typeof(BoxCollider), typeof(Rigidbody))]
public class PP_PickupItem : MonoBehaviour, PP_IInteractable
{
    [SerializeField] private Rigidbody rb;
    private Transform holdPointReference;
    private bool isHeld = false;

    public Rigidbody Rb => rb;

    private void Awake()
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody>();
        }
    }

    private void FixedUpdate()
    {
        // Continuously sync held item position to hold point using physics-safe movement
        if (isHeld && holdPointReference != null && rb != null && rb.isKinematic)
        {
            rb.MovePosition(holdPointReference.position);
            rb.MoveRotation(holdPointReference.rotation);
        }
    }

    private void Reset()
    {
        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = true;
            rb.isKinematic = false;
            rb.interpolation = RigidbodyInterpolation.Interpolate;
        }

        BoxCollider colliderComponent = GetComponent<BoxCollider>();
        if (colliderComponent != null)
        {
            colliderComponent.isTrigger = false;
        }
    }

    public void Interact(PP_PlayerInteractor interactor)
    {
        if (interactor == null)
        {
            return;
        }

        if (interactor.HeldItem == this)
        {
            interactor.DropHeldItem();
            return;
        }

        if (!interactor.HasHeldItem)
        {
            interactor.PickUp(this);
        }
    }

    public void OnPickedUp(Transform holdPoint)
    {
        if (holdPoint == null)
        {
            return;
        }

        holdPointReference = holdPoint;
        transform.SetParent(holdPoint);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        if (rb != null)
        {
            if (!rb.isKinematic)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }

            rb.isKinematic = true;
            rb.useGravity = false;
        }

        isHeld = true;
    }

    public void OnDropped()
    {
        isHeld = false;
        holdPointReference = null;
        transform.SetParent(null);

        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
        }
    }

    public void OnReleasedFromHold()
    {
        isHeld = false;
        holdPointReference = null;
    }

    public void SetPhysicsState(bool isKinematic)
    {
        if (rb == null)
        {
            return;
        }

        rb.isKinematic = isKinematic;
        rb.useGravity = !isKinematic;
    }
}
