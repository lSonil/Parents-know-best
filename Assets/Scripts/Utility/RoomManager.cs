using UnityEngine;

public class RoomManager : MonoBehaviour
{
    // Method to trigger CheckDoors on all RoomController instances
    public void UpdateAllRooms()
    {
        BorderHandler[] allBorderHandlers = FindObjectsByType<BorderHandler>(FindObjectsSortMode.None);
        foreach (BorderHandler borderHandler in allBorderHandlers)
        {
            borderHandler.RecreateBorder();
        }
        RoomController[] allRoomControllers = FindObjectsByType<RoomController>(FindObjectsSortMode.None);
        foreach (RoomController roomController in allRoomControllers)
        {
            roomController.CheckDoors();
        }

    }
}
