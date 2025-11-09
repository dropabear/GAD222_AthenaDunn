using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Tooltip("Total number of rooms in the house.")]
    public int totalRooms = 5;

    private int completedRooms = 0;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        PlacementBox.OnRoomComplete += HandleRoomComplete;
    }

    void OnDestroy()
    {
        PlacementBox.OnRoomComplete -= HandleRoomComplete;
    }

    void HandleRoomComplete(PlacementBox box)
    {
        completedRooms++;
        Debug.Log($"Room completed: {box.name}. ({completedRooms}/{totalRooms})");

        if (completedRooms >= totalRooms)
        {
            Debug.Log("All rooms complete! You win!");
            // TODO: Trigger victory UI or sequence here
        }
    }
}

