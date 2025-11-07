using UnityEngine;

public class PlacementBox : MonoBehaviour
{
    [Tooltip("Only accepts items with this categoryTag (e.g., 'LivingRoom').")]
    public string acceptedCategory = "Generic";

    [Tooltip("Where the item should appear when placed inside.")]
    public Transform dropPoint;

    public Vector3 GetDropOffset()
    {
        return dropPoint != null ? dropPoint.localPosition : Vector3.zero;
    }
}

