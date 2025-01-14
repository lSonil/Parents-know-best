using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.UI;

public enum TimeState
{
    Time8,
    Time9,
    Time10,
    Time11,
    Time12,
    Time13,
    Time14,
    Time15,
    Time16,
    Time17,
    Time18,
    Time19,
    Time20,
    Time21,
    Time22
}

public enum TimePhase
{
    Time8,
    Time9,
    Time10,
    Time11,
    Time12,
    Time13,
    Time14,
    Time15,
    Time16,
    Time17,
    Time18,
    Time19,
    Time20,
    Time21,
    Time22
}

public class GlobalInfo : MonoBehaviour
{

    public GameObject popUp;
    [SerializeField] public List<RoomController> rooms;
    [SerializeField] public List<KeyPos> keyPos;
    [SerializeField] public List<Door> doors;

    public float duration = 0.3f;
    public Vector2 globalTime;

    public static GlobalInfo i;

    public bool gameOver = false;
    public bool powerOutage = false;
    public bool gasLeak = false;

    public List<Item> items;
    public Item candle;
    public Item beer;
    public Item bat;
    public Item key;
    public Item lighter;
    public Item scissors;
    public Item book;
    public Item rbook;
    public Item remote;
    public Item ibby;
    public Item junk;
    public Item chocolate;

    public NPCBro bro;
    public NPCDad dad;
    public NPCMom mom;
    public NPCBby bby;
    public NPCCat cat;
    public Vector3 readyToHuntPlayer;

    public Transform tableBroPosition;
    public Transform tableDadPosition;
    public Transform tableMomPosition;
    public Transform bin;

    private void Awake()
    {
        i = this;
    }

    public Dictionary<TimeState, Vector2> timeVectors = new Dictionary<TimeState, Vector2>
    {
        { TimeState.Time8, new Vector2(8, 0) },
        { TimeState.Time9, new Vector2(9, 0) },
        { TimeState.Time10, new Vector2(10, 0) },
        { TimeState.Time11, new Vector2(11, 0) },
        { TimeState.Time12, new Vector2(12, 0) },
        { TimeState.Time13, new Vector2(13, 0) },
        { TimeState.Time14, new Vector2(14, 0) },
        { TimeState.Time15, new Vector2(15, 0) },
        { TimeState.Time16, new Vector2(16, 0) },
        { TimeState.Time17, new Vector2(17, 0) },
        { TimeState.Time18, new Vector2(18, 0) },
        { TimeState.Time19, new Vector2(19, 0) },
        { TimeState.Time20, new Vector2(20, 0) },
        { TimeState.Time21, new Vector2(21, 0) },
        { TimeState.Time22, new Vector2(22, 0) }
        // Add other states as needed
    };

    public TimeState RoutineToFollow()
    {
        TimeState lastTimeState = TimeState.Time8;
        Vector2 lastTimeVector;
        foreach (var kvp in timeVectors)
        {
            TimeState timeState = kvp.Key;
            Vector2 timeVector = kvp.Value;
            int currentTime = (int)(globalTime.x * 60 + globalTime.y);
            int stateTime = (int)(timeVector.x * 60 + timeVector.y);

            if (currentTime < stateTime)
            {
                return lastTimeState;
            }
            else
            {
                lastTimeState = kvp.Key;
                lastTimeVector = kvp.Value;

            }
        }
        return TimeState.Time22; // Default case if none match
    }

    [ExecuteAlways] // Ensures the script runs in the editor even when the game isn't playing.
    void OnValidate()
    {
        if (keyPos == null)
        {
            keyPos = new List<KeyPos>();
        }
        else
        {
            keyPos.Clear();
        }

        keyPos.AddRange(Object.FindObjectsByType<KeyPos>(FindObjectsSortMode.None));

        if (doors == null)
        {
            doors = new List<Door>();
        }
        else
        {
            doors.Clear();
        }

        doors.AddRange(Object.FindObjectsByType<Door>(FindObjectsSortMode.None));

        if (rooms == null)
        {
            rooms = new List<RoomController>();
        }
        else
        {
            rooms.Clear();
        }

        HashSet<RoomController> roomSet = new HashSet<RoomController>();

        foreach (Transform obj in transform)
        {
            RoomController room = obj.gameObject.GetComponent<RoomController>();
            if (room != null && roomSet.Add(room))
            {
                rooms.Add(room);
            }
        }
        items.Clear();
        items.Add(null);
        bat = GameObject.Find("Bat").GetComponent<Item>();
        items.Add(bat);
        beer = GameObject.Find("Beer").GetComponent<Item>();
        items.Add(beer);
        book = GameObject.Find("Book").GetComponent<Item>();
        items.Add(book);
        candle = GameObject.Find("Candle").GetComponent<Item>();
        items.Add(candle);
        junk = GameObject.Find("Junk").GetComponent<Item>();
        items.Add(junk);
        key = GameObject.Find("Key").GetComponent<Item>();
        items.Add(key);
        lighter = GameObject.Find("Lighter").GetComponent<Item>();
        items.Add(lighter);
        rbook = GameObject.Find("RBook").GetComponent<Item>();
        items.Add(rbook);
        remote = GameObject.Find("Remote").GetComponent<Item>();
        items.Add(remote);
        scissors = GameObject.Find("Scissors").GetComponent<Item>();
        items.Add(scissors);
        ibby = GameObject.Find("BbyNPC").GetComponent<Item>();
        items.Add(ibby);
        chocolate = GameObject.Find("Chocolate").GetComponent<Item>();
        items.Add(chocolate);

        dad = GameObject.Find("DadNPC").GetComponent<NPCDad>();
        bro = GameObject.Find("BroNPC").GetComponent<NPCBro>();
        mom = GameObject.Find("MomNPC").GetComponent<NPCMom>();
        bby = GameObject.Find("BbyNPC").GetComponent<NPCBby>();
        cat = GameObject.Find("CatNPC").GetComponent<NPCCat>();

        bin = GameObject.Find("ChurchUsePos").transform;
    }

    public RoomController GetRoom(string roomName)
    {
        foreach (RoomController room in rooms)
        {
            if (roomName.Equals(room.name))
                return room;
        }
        return null;
    }
    public KeyPos GetKeyPos(string objName)
    {
        foreach (KeyPos intractable in keyPos)
        {
            if (objName.Equals(intractable.transform.name))
                return intractable;
        }
        return null;
    }

    public Door GetDoor(string doorName)
    {
        foreach (Door door in doors)
        {
            if (doorName.Equals(door.name))
                return door;
        }
        return null;
    }

    public KeyPos GetRandomKeyPos()
    {
        return keyPos[UnityEngine.Random.Range(0, keyPos.Count)];
    }
}
