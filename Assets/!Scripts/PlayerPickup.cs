using UnityEngine;

public class PlayerPickup : MonoBehaviour
{
    [Header("Pickup Settings")]
    public Camera playerCamera;
    public float pickupRange = 3f;
    public Transform holdPoint; // Empty GameObject child of camera
    public KeyCode interactKey = KeyCode.E;

    [Header("Debug Settings")]
    public bool showDebugRay = true;

    private PickupObject heldObject;
    private GameObject lastLookedAt;

    void Update()
    {
        if (Input.GetKeyDown(interactKey))
        {
            if (heldObject == null)
                TryPickup();
            else
                TryDropOrPlace();
        }

        if (showDebugRay && playerCamera != null)
        {
            Debug.DrawRay(playerCamera.transform.position, playerCamera.transform.forward * pickupRange, Color.green);
        }

        UpdateLookedAt();
    }

    void UpdateLookedAt()
    {
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out RaycastHit hit, pickupRange))
        {
            GameObject currentLook = hit.collider.gameObject;
            if (currentLook != lastLookedAt)
            {
                lastLookedAt = currentLook;
                Debug.Log($"Looking at: {currentLook.name}");
            }
        }
        else if (lastLookedAt != null)
        {
            lastLookedAt = null;
        }
    }

    void TryPickup()
    {
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out RaycastHit hit, pickupRange))
        {
            PickupObject pickup = hit.collider.GetComponent<PickupObject>();
            if (pickup != null && !pickup.isHeld)
            {
                heldObject = pickup;
                pickup.isHeld = true;

                if (pickup.TryGetComponent<Rigidbody>(out Rigidbody rb))
                {
                    rb.isKinematic = true;
                    rb.detectCollisions = false;
                }

                pickup.transform.SetParent(holdPoint);
                pickup.transform.localPosition = Vector3.zero;
                pickup.transform.localRotation = Quaternion.identity;
            }
        }
    }

    void TryDropOrPlace()
    {
        if (heldObject == null) return;

        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out RaycastHit hit, pickupRange))
        {
            PlacementBox box = hit.collider.GetComponent<PlacementBox>();
            if (box != null && box.AcceptsCategory(heldObject.categoryTag))
            {
                Debug.Log($"Placing {heldObject.name} into {box.name}");
                heldObject.transform.SetParent(null);
                heldObject.isHeld = false;

                if (heldObject.TryGetComponent<Rigidbody>(out Rigidbody rb))
                {
                    rb.isKinematic = false;
                    rb.detectCollisions = true;
                }

                // Resize if any dimension is larger than max
                Vector3 maxScale = Vector3.one; // x:1, y:1, z:1
                Vector3 currentScale = heldObject.transform.localScale;
                if (currentScale.x > maxScale.x || currentScale.y > maxScale.y || currentScale.z > maxScale.z)
                {
                    float scaleFactor = Mathf.Min(maxScale.x / currentScale.x,
                                                  maxScale.y / currentScale.y,
                                                  maxScale.z / currentScale.z);
                    heldObject.transform.localScale = currentScale * scaleFactor;
                }

                box.RegisterObject(heldObject);
                heldObject = null;
                return;
            }
        }

        DropHeldObject();
    }

    void DropHeldObject()
    {
        if (heldObject == null) return;

        heldObject.transform.SetParent(null);
        heldObject.isHeld = false;

        if (heldObject.TryGetComponent<Rigidbody>(out Rigidbody rb))
        {
            rb.isKinematic = false;
            rb.detectCollisions = true;
            rb.AddForce(playerCamera.transform.forward * 1f, ForceMode.Impulse);
        }

        heldObject = null;
    }
}




