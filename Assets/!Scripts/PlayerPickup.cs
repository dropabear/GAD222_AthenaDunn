using UnityEngine;
using TMPro;

public class PlayerPickup : MonoBehaviour
{
    [Header("Pickup Settings")]
    public Camera playerCamera;
    public float pickupRange = 3f;
    public Transform holdPoint; // Empty GameObject child of camera
    public KeyCode interactKey = KeyCode.E;

    [Header("Debug Settings")]
    public bool showDebugRay = true;

    [Header("Floating Text Settings")]
    public GameObject floatingTextPrefab; // 3D TextMeshPro prefab
    public Vector3 textOffset = new Vector3(0, 2f, 0); // offset above object
    public float textLifetime = 1.5f;

    private PickupObject heldObject;
    private GameObject lastLookedAt;
    private GameObject currentFloatingText;

    public FloatingTextManager ftManager;

    void Start()
    {
        ftManager = FindObjectOfType<FloatingTextManager>();
    }

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

                // Determine label
                string label = "";
                PickupObject pickup = currentLook.GetComponent<PickupObject>();
                PlacementBox box = currentLook.GetComponent<PlacementBox>();

                if (pickup != null)
                    label = "Press E to pick up";
                else if (box != null)
                    label = $"Press E to place inside ({box.ObjectCount}/{box.requiredItemCount})";

                if (ftManager != null)
                {
                    if (!string.IsNullOrEmpty(label))
                        ftManager.ShowText(currentLook.transform, label);
                    else
                        ftManager.ClearText();
                }
            }
        }
        else if (lastLookedAt != null)
        {
            lastLookedAt = null;
            ftManager?.ClearText();
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

                // Resize large objects
                Vector3 maxScale = Vector3.one;
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





