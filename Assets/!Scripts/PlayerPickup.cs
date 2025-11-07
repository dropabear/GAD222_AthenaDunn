using UnityEngine;

public class PlayerPickup : MonoBehaviour
{
    [Header("Pickup Settings")]
    public Camera playerCamera;
    public float pickupRange = 3f;
    public Transform holdPoint;   // Empty GameObject child of camera
    public KeyCode interactKey = KeyCode.E;

    [Header("Debug Settings")]
    public bool showDebugRay = true;

    private PickupObject heldObject;

    void Update()
    {
        // Draw the ray in Scene view (for debugging)
        if (showDebugRay && playerCamera != null)
        {
            Debug.DrawRay(playerCamera.transform.position, playerCamera.transform.forward * pickupRange, Color.green);
        }

        // Optional: show what the player is looking at
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out RaycastHit hit, pickupRange))
        {
            if (hit.collider != null)
            {
                Debug.Log($"Looking at: {hit.collider.name}");
            }
        }

        // Interact key pressed
        if (Input.GetKeyDown(interactKey))
        {
            if (heldObject == null)
            {
                TryPickup();
            }
            else
            {
                TryPlace();
            }
        }
    }

    void TryPickup()
    {
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out RaycastHit hit, pickupRange))
        {
            PickupObject pickup = hit.collider.GetComponent<PickupObject>();

            if (pickup != null && !pickup.isHeld)
            {
                Debug.Log($"Picking up object: {pickup.name}");
                heldObject = pickup;
                pickup.isHeld = true;

                // Disable physics and collisions while held
                if (pickup.TryGetComponent<Rigidbody>(out Rigidbody rb))
                {
                    rb.isKinematic = true;
                    rb.detectCollisions = false;
                }

                pickup.transform.SetParent(holdPoint);
                pickup.transform.localPosition = Vector3.zero;
                pickup.transform.localRotation = Quaternion.identity;
            }
            else
            {
                Debug.Log("No valid pickup object found.");
            }
        }
        else
        {
            Debug.Log("Raycast didn't hit anything when trying to pick up.");
        }
    }

    void TryPlace()
    {
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out RaycastHit hit, pickupRange))
        {
            PlacementBox box = hit.collider.GetComponent<PlacementBox>();
            if (box != null && box.acceptedCategory == heldObject.categoryTag)
            {
                Debug.Log($"Placing object: {heldObject.name} into box: {box.name}");
                heldObject.transform.SetParent(box.transform);
                heldObject.transform.localPosition = box.GetDropOffset();
                heldObject.transform.localRotation = Quaternion.identity;
                heldObject.isHeld = false;
                heldObject = null;
                return;
            }
            else
            {
                Debug.Log("Not looking at a valid placement box or category mismatch.");
            }
        }
        else
        {
            Debug.Log("Raycast didn't hit anything when trying to place.");
        }

        DropHeldObject();
    }

    void DropHeldObject()
    {
        Debug.Log($"Dropping object: {heldObject.name}");
        heldObject.transform.SetParent(null);
        heldObject.isHeld = false;

        if (heldObject.TryGetComponent<Rigidbody>(out Rigidbody rb))
        {
            rb.isKinematic = false;
            rb.detectCollisions = true;

            // Small forward push for realism
            rb.AddForce(playerCamera.transform.forward * 1f, ForceMode.Impulse);
        }

        heldObject = null;
    }
}


