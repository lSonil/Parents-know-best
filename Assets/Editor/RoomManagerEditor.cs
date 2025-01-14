using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RoomManager))]
public class RoomManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        RoomManager manager = (RoomManager)target;

        if (GUILayout.Button("Update All Rooms"))
        {
            manager.UpdateAllRooms();
        }
    }
}
