using System.Collections.Generic;
using UnityEngine;

public class PlacementBox : MonoBehaviour
{
    [Tooltip("Only accepts items with this categoryTag.")]
    public string acceptedCategory = "Generic";

    private List<PickupObject> objectsInBox = new List<PickupObject>();

    private void OnTriggerEnter(Collider other)
    {
        PickupObject obj = other.GetComponent<PickupObject>();
        if (obj != null && AcceptsCategory(obj.categoryTag) && !objectsInBox.Contains(obj))
        {
            objectsInBox.Add(obj);
            Debug.Log($"{obj.name} entered {name}. Total objects: {objectsInBox.Count}");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        PickupObject obj = other.GetComponent<PickupObject>();
        if (obj != null && objectsInBox.Contains(obj))
        {
            objectsInBox.Remove(obj);
            Debug.Log($"{obj.name} left {name}. Total objects: {objectsInBox.Count}");
        }
    }

    public bool AcceptsCategory(string category)
    {
        return category == acceptedCategory;
    }

    public void RegisterObject(PickupObject obj)
    {
        // Optional: can move object slightly above box so it falls naturally
        obj.transform.position = transform.position + Vector3.up * 0.5f;
    }

    public int ObjectCount => objectsInBox.Count;

    // Later you can add a method to clear objects when target count reached
    public void ClearBox()
    {
        foreach (var obj in objectsInBox)
        {
            if (obj != null)
                Destroy(obj.gameObject);
        }
        objectsInBox.Clear();
    }
}

