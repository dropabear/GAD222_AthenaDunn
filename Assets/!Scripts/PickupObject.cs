using UnityEngine;

public class PickupObject : MonoBehaviour
{
    [Tooltip("What type of box this object belongs to (e.g., 'LivingRoom', 'Kitchen').")]
    public string categoryTag = "Generic";

    [HideInInspector] public bool isHeld = false;
}

