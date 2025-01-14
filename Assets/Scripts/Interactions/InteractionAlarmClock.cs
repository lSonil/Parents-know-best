using UnityEngine;

public class InteractionAlarmClock : MonoBehaviour
{
    public TimeState time;

    private void Update()
    {
        if(GlobalInfo.i.RoutineToFollow()==time)
        {
            GetComponent<Collider2D>().enabled = true;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<NPCDad>()!=null)
        {
            GetComponent<Collider2D>().enabled = false;
            collision.GetComponent<NPCDad>().WakeUp();
        }
    }
}
