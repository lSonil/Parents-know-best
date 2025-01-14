using System.Collections.Generic;
using UnityEngine;

public class RoomController : MonoBehaviour
{
    public List<Door> doors;
    public PolygonCollider2D border;
    public float camerSize = 1f;

    // Method to be called externally to update the list of doors
    public void CheckDoors()
    {
        doors.Clear();
        foreach (Transform obj in transform)
        {
            Door action = obj.gameObject.GetComponent<Door>();
            if (action != null && !doors.Contains(action))
            {
                doors.Add(action);
            }
        }
    }
}
