using System.Collections.Generic;
using UnityEngine;

public class PlacementBox : MonoBehaviour
{
    [Header("Box Settings")]
    [Tooltip("Only accepts items with this categoryTag.")]
    public string acceptedCategory = "Generic";

    [Tooltip("How many items need to be placed to complete this room.")]
    public int requiredItemCount = 3;

    [Tooltip("Prefab of the closed box that replaces this one when complete.")]
    public GameObject closedBoxPrefab;

    [Header("Debug Info")]
    public bool showDebug = true;

    private List<PickupObject> objectsInBox = new List<PickupObject>();
    private bool roomCompleted = false;

    public delegate void RoomCompleteHandler(PlacementBox box);
    public static event RoomCompleteHandler OnRoomComplete;

    private void OnTriggerEnter(Collider other)
    {
        if (roomCompleted) return;

        PickupObject obj = other.GetComponent<PickupObject>();
        if (obj != null && AcceptsCategory(obj.categoryTag) && !objectsInBox.Contains(obj))
        {
            objectsInBox.Add(obj);
            if (showDebug) Debug.Log($"{obj.name} entered {name}. Total objects: {objectsInBox.Count}");
            CheckCompletion();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (roomCompleted) return;

        PickupObject obj = other.GetComponent<PickupObject>();
        if (obj != null && objectsInBox.Contains(obj))
        {
            objectsInBox.Remove(obj);
            if (showDebug) Debug.Log($"{obj.name} left {name}. Total objects: {objectsInBox.Count}");
        }
    }

    public bool AcceptsCategory(string category)
    {
        return category == acceptedCategory;
    }

    public void RegisterObject(PickupObject obj)
    {
        // Drop slightly above the box so physics settle naturally
        obj.transform.position = transform.position + Vector3.up * 0.5f;
    }

    void CheckCompletion()
    {
        if (objectsInBox.Count >= requiredItemCount)
        {
            roomCompleted = true;
            if (showDebug) Debug.Log($"{name} is now complete!");
            CompleteRoom();
        }
    }

    void CompleteRoom()
    {
        // Destroy all objects currently inside
        foreach (var obj in objectsInBox)
        {
            if (obj != null)
                Destroy(obj.gameObject);
        }
        objectsInBox.Clear();

        // Replace this box with the closed version
        if (closedBoxPrefab != null)
        {
            Instantiate(closedBoxPrefab, transform.position, transform.rotation);
        }

        // Notify a global tracker
        OnRoomComplete?.Invoke(this);

        // Remove this box object
        Destroy(gameObject);
    }

    public int ObjectCount => objectsInBox.Count;
}


