using UnityEngine;


[RequireComponent(typeof(Collider))]
public class AutoOpenDoor : MonoBehaviour
{
    [Header("Door Settings")]
    public Transform door;           // Assign the door mesh that should rotate
    public Vector3 openRotationEuler; // Local rotation angles when open
    public float openSpeed = 2f;      // How fast it opens
    public float closeSpeed = 2f;     // How fast it closes

    [Header("Trigger Settings")]
    public float triggerDistance = 2f; // How close the player needs to be
    public Transform player;           // Assign your player here

    private Quaternion closedRotation;
    private Quaternion openRotation;

    private void Start()
    {
        if (door == null) door = transform; // fallback to object this script is on
        closedRotation = door.localRotation;
        openRotation = Quaternion.Euler(openRotationEuler) * closedRotation;

        // Ensure the object has a trigger collider
        Collider col = GetComponent<Collider>();
        col.isTrigger = true;

        // Auto-assign player if not set
        if (player == null && GameObject.FindWithTag("Player"))
            player = GameObject.FindWithTag("Player").transform;
    }

    private void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(player.position, transform.position);

        if (distance <= triggerDistance)
            door.localRotation = Quaternion.Slerp(door.localRotation, openRotation, Time.deltaTime * openSpeed);
        else
            door.localRotation = Quaternion.Slerp(door.localRotation, closedRotation, Time.deltaTime * closeSpeed);
    }
}

