using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{

    public Door doorConnTo;

    public bool locked;

    public Teleport doorTeleport;
    public GameObject doorLocked;
    public Transform doorPost;
    private Vector3 startPos;
    public Vector3 offset;
    public bool horizontal;
    public bool positive;

    private void Awake()
    {
        doorTeleport = FindChildEndingWith(transform, "DoorUnlocked").GetComponent<Teleport>();
        doorPost = FindChildEndingWith(transform, "DoorPosition");
        if (FindChildEndingWith(transform, "DoorLocked"))
            doorLocked = FindChildEndingWith(transform, "DoorLocked").gameObject;
        startPos= doorPost.transform.position;
        doorConnTo = FindMatchingDoor();

        if (locked)
        {
            doorConnTo.locked = true;
        }
        SetDoor(locked);
    }
    public void SetDoor(bool locked)
    {
        if (doorLocked == null) return;
        doorLocked.SetActive(locked);
        doorTeleport.gameObject.SetActive(!locked);
        doorPost.transform.position = locked ? startPos + offset : startPos;
    }
    Transform FindChildEndingWith(Transform parent, string suffix)
    {
        foreach (Transform child in parent)
        {
            if (child.name.EndsWith(suffix))
            {
                return child;
            }
        }
        return null;
    }
    public bool Unlock()
    {
        locked = !locked;
        doorConnTo.locked = locked;
        SetDoor(locked);
        doorConnTo.SetDoor(locked);
        return true;
    }

    public bool Open()
    {
        locked = false;
        doorConnTo.locked = false;
        SetDoor(false);
        doorConnTo.SetDoor(false);
        return true;
    }
    public bool Close()
    {
        locked = true;
        doorConnTo.locked = true;
        SetDoor(true);
        doorConnTo.SetDoor(true);
        return true;
    }
    public Door FindMatchingDoor()
    {
        // Get the name of the current object
        string currentName = gameObject.name;

        // Check if the name follows the expected format "DoorFromToTo"
        if (!currentName.StartsWith("Door") || !currentName.Contains("To"))
        {
            Debug.LogError("Object name format is incorrect. Expected format: 'DoorFromToTo'");
            return null;
        }

        string[] parts = currentName.Substring(4).Split(new string[] { "To" }, System.StringSplitOptions.None);
        if (parts.Length != 2)
        {
            Debug.LogError("Object name format is incorrect. Could not parse From and To.");
            return null;
        }

        string From = parts[0];
        string To = parts[1];

        // Construct the target object name
        string targetName = $"Door{To}To{From}";

        // Find the object in the scene with the constructed name
        GameObject targetDoor = GameObject.Find(targetName);

        if (targetDoor == null)
        {
            Debug.LogError($"Matching door '{targetName}' not found in the scene.");
        }

        return targetDoor.GetComponent<Door>();
    }

}
