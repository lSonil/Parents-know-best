using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using Unity.VisualScripting;

public class SearchAI : MonoBehaviour
{
    [Header("SearchAI")]

    public float speed;

    public List<Transform> route;
    [HideInInspector]
    public SpriteRenderer sr;
    [HideInInspector]
    public Rigidbody2D rb;
    [HideInInspector]
    public Collider2D col;
    [HideInInspector]
    public Animator anim;

    [HideInInspector]
    public bool hasFoundTarget = false;
    [HideInInspector]
    public AIDestinationSetter aiPathTarget;
    [HideInInspector]
    public AIPath aiPathController;

    public RoomController room;
    [HideInInspector]
    public Transform targetMidPoint;
    public Transform target;
    [HideInInspector]
    public RoomController targetCurrentRoom;
    [HideInInspector]
    public bool eventTriggered = false;

    [HideInInspector]
    public float moveX;
    [HideInInspector]
    public float moveY;

    [HideInInspector]
    public bool hunt;
    [HideInInspector]
    public bool shouldHunt;

    public virtual void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        sr = GetComponent<SpriteRenderer>();

        room = GetComponentInParent<RoomController>();
        transform.SetParent(room.transform);

        aiPathTarget = GetComponentInParent<AIDestinationSetter>();
        aiPathController = GetComponentInParent<AIPath>();
        aiPathController.maxSpeed = speed;
    }
    public virtual void TargetIsInRange() { }
    public virtual bool HuntPreconditionsCheck() { return true; }

    void FixedUpdate()
    {
        if (target != null && !hasFoundTarget)
        {
            GoToLocation();
        }
    }

    public void Stop()
    {
        target = null;
        aiPathTarget.target = null;
    }

    public void ResetSearch(Transform newTarget, bool should = false)
    {
        shouldHunt = should;
        targetMidPoint = null;
        target = newTarget;
        hasFoundTarget = false;
        PrepareSearch();
    }

    public bool SameRoom(RoomController theirRoom)
    {
        return theirRoom == room;
    }

    public virtual void GoToLocation()
    {
        if (room != null)
        {
            if (targetMidPoint == null || targetCurrentRoom != target.GetComponentInParent<RoomController>())
            {
                PrepareSearch();
            }
            aiPathTarget.target = targetMidPoint;

            hunt = (targetMidPoint == PlayerHandler.i.transform) && shouldHunt;

            if (Vector2.Distance(transform.position, targetMidPoint.position) <= 0.05f)
            {
                if (targetMidPoint != target)
                {                    
                    Door door = targetMidPoint.gameObject.GetComponentInParent<Door>();
                    if (!door.locked)
                    {
                        Transform goTo = door.doorConnTo.doorPost;
                        transform.position = goTo.position;

                        room = goTo.GetComponentInParent<RoomController>();
                        transform.SetParent(goTo.GetComponentInParent<RoomController>().transform);
                    }
                    PrepareSearch();
                }
                else
                {
                    hasFoundTarget = true;
                    TargetIsInRange();
                }

                if (route.Count != 0)
                    route.RemoveAt(0);
            }
            else
            {
                moveX = targetMidPoint.position.x - transform.position.x;
                moveY = targetMidPoint.position.y - transform.position.y;
            }
        }
    }

    public void PrepareSearch()
    {
        targetMidPoint = null;
        if (hasFoundTarget)
            return;
        route.Clear();
        targetCurrentRoom = target.GetComponentInParent<RoomController>();
        Search(room, target.GetComponentInParent<RoomController>());

        if (route.Count != 0)
            targetMidPoint = route[0];
    }

    public void Search(RoomController actualRoom, RoomController destination, List<RoomController> bannedRoom = null, List<List<Transform>> allRoutes = null, List<Transform> posibleRoute = null, int counter = 0, bool foundRoute = false)
    {
        if (bannedRoom == null)
        {
            bannedRoom = new List<RoomController>();

            allRoutes = new List<List<Transform>>();

            posibleRoute = new List<Transform>();
        }

        foreach (Door pos in actualRoom.doors)
        {

            RoomController parent = pos.transform.GetComponentInParent<RoomController>();
            if (actualRoom == destination)
            {
                if (!allRoutes.Contains(posibleRoute))
                {
                    List<Transform> tempP = new List<Transform>(posibleRoute);
                    tempP.Add(target);
                    allRoutes.Add(tempP);
                    foundRoute = true;
                }
            }
            else
            {
                List<RoomController> tempB = new List<RoomController>(bannedRoom);
                if (!bannedRoom.Contains(parent))
                {
                    tempB.Add(parent);
                }

                RoomController goesTo = pos.doorConnTo.transform.parent.GetComponent<RoomController>();

                if (!tempB.Contains(goesTo))
                {
                    Ban(tempB);
                    if (counter < GlobalInfo.i.rooms.Count - 1)
                    {
                        List<Transform> tempP = new List<Transform>(posibleRoute);
                        tempP.Add(pos.doorPost);
                        Search(goesTo, destination, tempB, allRoutes, tempP, ++counter, false);
                    }
                    else
                        return;
                }
            }
        }
        if (foundRoute)
            Choose(allRoutes);
    }

    public void Ban(List<RoomController> rout)
    {
        string route = "";
        foreach (RoomController item in rout)
        {
            route += item.name + " ";
        }
    }

    public void Choose(List<List<Transform>> allRoutes)
    {

        int shortestPath = allRoutes[0].Count;
        foreach (List<Transform> route in allRoutes)
        {
            if (route.Count < shortestPath)
                shortestPath = route.Count;
        }
        List<List<Transform>> shortList = new List<List<Transform>>();
        foreach (List<Transform> route in allRoutes)
        {
            if (route.Count == shortestPath)
                shortList.Add(route);
        }
        int i = Random.Range(0, shortList.Count);
        route = shortList[i];
    }


    public Dictionary<TimePhase, bool> timeReached = new Dictionary<TimePhase, bool>
    {
        { TimePhase.Time8, false },
        { TimePhase.Time9, false },
        { TimePhase.Time10, false },
        { TimePhase.Time11, false },
        { TimePhase.Time12, false },
        { TimePhase.Time13, false },
        { TimePhase.Time14, false },
        { TimePhase.Time15, false },
        { TimePhase.Time16, false },
        { TimePhase.Time17, false },
        { TimePhase.Time18, false },
        { TimePhase.Time19, false },
        { TimePhase.Time20, false },
        { TimePhase.Time21, false },
        { TimePhase.Time22, false }
    };

    public int GetIndexOfKey(TimeState key)
    {
        int index = 0; foreach (var kvp in timeReached) { if (kvp.Key.Equals(key)) { return index; } index++; }
        return -1; // Key not found
    }
}