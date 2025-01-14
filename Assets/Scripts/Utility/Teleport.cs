
using UnityEngine;

public class Teleport : MonoBehaviour
{
    public void PrepareToTeleport()
    {
        Door door = GetComponentInParent<Door>();
        bool horizontal = door.horizontal;
        float input;

        input = horizontal? input = Input.GetAxisRaw("Vertical"): Input.GetAxisRaw("Horizontal");
        if ((door.positive && input >= 0) || (!door.positive && input <= 0) || door.locked)
        {
            return;
        }

        Vector3 offset = Vector3.zero;

        offset.x = horizontal ? door.doorPost.transform.position.x - PlayerHandler.i.transform.position.x : 0;
        offset.y = horizontal ? 0 : door.doorPost.transform.position.y - PlayerHandler.i.transform.position.y;

        PlayerHandler.i.transform.position = door.doorConnTo.doorPost.position - offset;
        return;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PrepareToTeleport();
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PrepareToTeleport();
        }
    }
}