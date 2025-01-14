using System.Collections;
using UnityEngine;


public class NPCMom : SearchAI, IInteractable
{
    [Header("NPC")]
    public bool mad = false;
    public bool waiting = false;
    public bool hasBeer = false;
    public bool focused = false;
    public bool preparing = false;
    public bool searching = false;
    public bool cooking = false;
    public bool angry = false;
    public int ingredients = 0;
    public float cake = 0;

    private bool stopIt = false;
    public bool vote = true;

    public Dialog info;

    private void Start()
    {
        ResetSearch(GlobalInfo.i.GetKeyPos("KitchenSinkUsePos").transform);
    }

    public void Update()
    {
        AnimationHandle();
    }

    public void AnimationHandle()
    {
        anim.SetBool("isMoving", aiPathTarget.target != null);
        anim.SetFloat("moveX", moveX);
        anim.SetFloat("moveY", moveY);
        anim.SetFloat("item", cooking ? 1.1f : hasBeer ? 0.2f : 0);
        anim.SetFloat("wait", waiting ? 1f : 0);
        anim.SetFloat("cake", cake / 10);
    }
    IEnumerator Routine()
    {
        if (stopIt)
            yield return null;
        stopIt = true;

        while (stopIt)
        {
            switch (GlobalInfo.i.RoutineToFollow())
            {
                case TimeState.Time21:
                    if (!timeReached[TimePhase.Time21])
                    {
                        timeReached[TimePhase.Time21] = true;
                        stopIt = false;
                        bool dadVote = true, broVote = true;
                        if (GlobalInfo.i.dad != null)
                        {
                            dadVote = GlobalInfo.i.dad.vote;
                        }
                        if (GlobalInfo.i.bro != null)
                        {
                            broVote = GlobalInfo.i.bro.vote;
                        }
                        if (vote || dadVote || broVote)
                        {
                            ResetSearch(PlayerHandler.i.transform, true);
                        }
                        else
                        {
                            print("did");
                        }
                    }
                    break;
                case TimeState.Time20:
                    if (!timeReached[TimePhase.Time20])
                    {
                        cooking = false;
                        timeReached[TimePhase.Time20] = true;
                        ResetSearch(GlobalInfo.i.GetKeyPos("MomPointUsePos").transform);
                    }
                    break;
                case TimeState.Time19:
                    if (!timeReached[TimePhase.Time19])
                    {
                        timeReached[TimePhase.Time19] = true;
                        if (cake == 3)
                            ResetSearch(GlobalInfo.i.GetKeyPos("FridgeUsePos").transform);

                    }
                    break;
                case TimeState.Time17:
                    if (!timeReached[TimePhase.Time17])
                    {
                        timeReached[TimePhase.Time17] = true;
                        transform.position = GlobalInfo.i.GetKeyPos("ClockUsePos").transform.position;
                        if (cake == 2)
                            CookingSchedule();
                        else
                            ResetSearch(GlobalInfo.i.GetKeyPos("KitchenTableUsePos").transform);
                    }
                    break;
                case TimeState.Time15:
                    if (!timeReached[TimePhase.Time15])
                    {
                        timeReached[TimePhase.Time15] = true;
                        ResetSearch(GlobalInfo.i.GetKeyPos("ClockUsePos").transform);
                        cooking = false;
                        stopIt = false;
                    }
                    break;
                case TimeState.Time12:
                    if (!timeReached[TimePhase.Time12])
                    {
                        timeReached[TimePhase.Time12] = true;

                        ResetSearch(GlobalInfo.i.GetKeyPos("FridgeUsePos").transform);
                    }
                    break;
                case TimeState.Time11:
                    if (!timeReached[TimePhase.Time11])
                    {
                        timeReached[TimePhase.Time11] = true;

                        if (waiting && SameRoom(GlobalInfo.i.GetRoom("Livingroom")))
                        {
                            ResetSearch(GlobalInfo.i.GetRoom("Bedroom").transform);
                        }
                    }
                    break;
                case TimeState.Time10:
                case TimeState.Time9:
                case TimeState.Time8:
                    InteractionSink sink = GlobalInfo.i.GetKeyPos("KitchenSinkUsePos").GetComponent<InteractionSink>();
                    if (sink.cut && !sink.repaired && !GlobalInfo.i.dad.repair)
                    {
                        if (SameRoom(GlobalInfo.i.GetRoom("Livingroom")) && GlobalInfo.i.dad.SameRoom(GlobalInfo.i.GetRoom("Livingroom")) && GlobalInfo.i.dad.sitting && waiting)
                        {
                            ResetSearch(GlobalInfo.i.GetKeyPos("KitchenTableUsePos").transform);
                            GlobalInfo.i.dad.RepairSink();
                            stopIt = false;
                        }
                    }
                    if (waiting)
                    {
                        if (GlobalInfo.i.GetKeyPos("KitchenSinkUsePos").GetComponent<InteractionSink>().repaired)
                        {
                            FreeSink(false, false);

                            waiting = false;
                            ResetSearch(GlobalInfo.i.GetKeyPos("KitchenSinkUsePos").transform);
                            stopIt = false;
                        }
                    }
                    break;
            }


            if (SameRoom(PlayerHandler.i.room) && SameRoom(GlobalInfo.i.GetRoom("Kitchen")) && Inventory.i.CheckCurrentItem(GlobalInfo.i.beer))
            {
                FreeSink(false, false);
                GlobalInfo.i.GetKeyPos("KitchenStandStartUsePos").GetComponent<InteractionHold>().Block(false);
                cooking = false;
                ResetSearch(PlayerHandler.i.transform, true);
                stopIt = false;
            }
            else
            if (SameRoom(GlobalInfo.i.cat.room) && SameRoom(GlobalInfo.i.GetRoom("Kitchen")))
            {
                FreeSink(false, false);
                GlobalInfo.i.GetKeyPos("KitchenStandStartUsePos").GetComponent<InteractionHold>().Block(false);
                cooking = false;
                ResetSearch(GlobalInfo.i.cat.transform);
                stopIt = false;
            }
            yield return null;
        }
    }
    public void Die()
    {
        Destroy(gameObject);
    }

    public void FreeSink(bool val1 = true, bool val2 = true)
    {
        GlobalInfo.i.GetKeyPos("KitchenSinkUsePos").GetComponent<InteractionSink>().Block(val1);
        GlobalInfo.i.GetKeyPos("KitchenSinkUsePos").GetComponent<InteractionSink>().UseSink(val2);
    }

    public override void TargetIsInRange()
    {
        InteractionHoldAndOpen holdAndOpen = target.GetComponent<InteractionHoldAndOpen>();
        InteractionOven oven = target.GetComponent<InteractionOven>();
        InteractionNode node = target.GetComponent<InteractionNode>();
        InteractionHold hold = target.GetComponent<InteractionHold>();
        InteractionCrib crib = target.GetComponent<InteractionCrib>();
        InteractionSink sink = target.GetComponent<InteractionSink>();

        transform.position = target.transform.position;

        switch (target.name)
        {
            case "CatNPC":
                StartCoroutine(Routine());
                if (!SameRoom(GlobalInfo.i.GetRoom("Kitchen")))
                {
                    ResetSearch(GlobalInfo.i.GetKeyPos("LivingroomToKitchenDoorPosition").transform);
                }
                break;
            case "LivingroomToKitchenDoorPosition":
                Stop();
                StartCoroutine(Shock());
                break;
            case "KitchenSinkUsePos":
                ingredients++;
                Stop();
                if (sink.cut && !sink.repaired && !waiting)
                {
                    cooking = false;
                    FreeSink(false, false);
                    ResetSearch(GlobalInfo.i.GetKeyPos("MomWaitUsePos").transform);
                    stopIt = false;
                }
                else
                {
                    cooking = true;
                    StartCoroutine(Routine());
                    CookingSchedule(GlobalInfo.i.GetKeyPos("KitchenStandStartUsePos").transform);
                    FreeSink();
                }
                break;
            case "KitchenStandStartUsePos":
                hold.Block(true);
                Stop();
                StartCoroutine(Routine());
                CookingSchedule(GlobalInfo.i.GetKeyPos("KitchenSinkUsePos").transform);
                break;
            case "MomWaitUsePos":
            case "MomPointUsePos":
            case "KitchenToLivingroomDoorPosition":
                Stop();
                waiting = true;
                StartCoroutine(Routine());
                break;
            case "CouchRightUsePos":
            case "KitchenTableUsePos":
                waiting = true;
                Stop();
                StartCoroutine(Routine());
                break;
            case "KitchenStand1UsePos":
                if (node.IsHolding(GlobalInfo.i.candle))
                {
                    cake++;
                    vote = false;
                }
                ResetSearch(GlobalInfo.i.GetKeyPos("KitchenTableUsePos").transform);
                Stop();
                StartCoroutine(Routine());
                break;
            case "OvenUsePos":
                if (cake == 1)
                {
                    ResetSearch(GlobalInfo.i.GetKeyPos("KitchenTableUsePos").transform);
                    cooking = false;
                    oven.ForceOn();
                    cake++;
                }
                else
                {
                    cake++;
                    cooking = true;
                    CookingSchedule();
                }

                StartCoroutine(Routine());
                break;
            case "FridgeUsePos":
                switch (GlobalInfo.i.RoutineToFollow())
                {
                    case TimeState.Time8:
                        hasBeer = false;
                        holdAndOpen.NPCHold(GlobalInfo.i.beer);
                        ResetSearch(GlobalInfo.i.GetKeyPos("KitchenSinkUsePos").transform);
                        break;
                    case TimeState.Time12:
                        if (holdAndOpen.IsHolding(GlobalInfo.i.beer))
                        {
                            holdAndOpen.GiveItemTo().NPCPickUp(transform);
                        }
                        ResetSearch(GlobalInfo.i.GetKeyPos("KitchenTableUsePos").transform);
                        break;
                    default:
                        if (cooking)
                        {
                            ResetSearch(GlobalInfo.i.GetKeyPos("KitchenTableUsePos").transform);
                            // add new item frosting, melty
                            print(1);
                        }
                        else
                        {
                            ResetSearch(GlobalInfo.i.GetKeyPos("KitchenStand1UsePos").transform);
                            print(2);
                        }
                        cooking = !cooking;
                        break;
                }
                break;
            case "ClockUsePos":
                Stop();
                if (SameRoom(PlayerHandler.i.room))
                {
                    ResetSearch(PlayerHandler.i.transform, true);
                }
                else
                {
                    target = null;
                    transform.position = GlobalInfo.i.GetKeyPos("ChurchUsePos").transform.position;
                    ResetSearch(GlobalInfo.i.GetKeyPos("ChurchUsePos").transform);
                }
                break;
            case "ChurchUsePos":
                StartCoroutine(Routine());
                break;
            case "Bedroom":
                transform.position = target.transform.position;
                if (!SameRoom(PlayerHandler.i.room))
                {
                    crib = GlobalInfo.i.GetKeyPos("CribUsePos").GetComponent<InteractionCrib>();
                    if (GlobalInfo.i.dad.SameRoom(GlobalInfo.i.GetRoom("Bedroom")) && GlobalInfo.i.dad.sleep && !crib.IsHolding(GlobalInfo.i.ibby))
                    {
                        GlobalInfo.i.dad.DieBed();
                    }
                }
                angry = true;
                ResetSearch(GlobalInfo.i.GetKeyPos("KitchenTableUsePos").transform);
                break;
            case "Player":

                if (timeReached[TimePhase.Time21])
                {
                    PlayerHandler.i.PlayerLose();
                    return;
                }
                
                waiting = true;

                if (SameRoom(GlobalInfo.i.GetRoom("Kitchen")))
                {
                    Stop();
                    PlayerHandler.i.PlayerLose();

                }
                else
                {
                    StartCoroutine(GroundPlayer());
                    ResetSearch(GlobalInfo.i.GetKeyPos("PlayerToHallEDoorPosition").transform);
                }
                break;
            default:
                Debug.LogWarning("New location, no action");
                break;
        }
    }

    public void CookingSchedule(Transform moveTo = null)
    {
        switch (cake)
        {
            case 0:
                if (timeReached[TimePhase.Time11] != true)
                    StartCoroutine(PrepareIngredients(moveTo));
                break;
            case 1:
                if (timeReached[TimePhase.Time15] != true)
                {
                    ResetSearch(GlobalInfo.i.GetKeyPos("OvenUsePos").transform);
                }
                break;
            case 2:
                if (timeReached[TimePhase.Time18] != true)
                {
                    ResetSearch(GlobalInfo.i.GetKeyPos("OvenUsePos").transform);
                }
                break;
            case 3:
                if (timeReached[TimePhase.Time19] != true)
                {
                    ResetSearch(GlobalInfo.i.GetKeyPos("FridgeUsePos").transform);
                }
                break;
            case 4:
                if (timeReached[TimePhase.Time22] != true)
                {
                    ResetSearch(GlobalInfo.i.GetKeyPos("KitchenTableUsePos").transform);
                }
                break;
        }
    }
    IEnumerator Shock()
    {
        yield return new WaitForSeconds(5);
        ResetSearch(GlobalInfo.i.GetKeyPos("KitchenSinkUsePos").transform);
    }

    IEnumerator PrepareIngredients(Transform moveTo)
    {
        yield return new WaitForSeconds(2f);

        if (moveTo == GlobalInfo.i.GetKeyPos("KitchenStandStartUsePos").transform)
        {
            FreeSink(false);
        }
        else
        {
            GlobalInfo.i.GetKeyPos("KitchenStandStartUsePos").GetComponent<InteractionHold>().Block(false);
        }

        if (ingredients >= 30)
            cake++;
        if (stopIt)
            ResetSearch(moveTo);
    }
    IEnumerator GroundPlayer()
    {
        bool stopIt = true;

        PlayerHandler.i.restricted = true;

        while (stopIt)
        {
            PlayerHandler.i.transform.position = transform.position;
            if (SameRoom(GlobalInfo.i.GetRoom("PlayerRoom")))
            {
                PlayerHandler.i.restricted = false;
                stopIt = false;
            }
            yield return null;
        }
        PlayerHandler.i.transform.position = GlobalInfo.i.GetKeyPos("PlayerToHallEDoorPosition").transform.position;

        GlobalInfo.i.GetKeyPos("PlayerToHallEDoorPosition").GetComponentInParent<Door>().Close();
        ResetSearch(GlobalInfo.i.GetKeyPos("ClockUsePos").transform);
        transform.position = GlobalInfo.i.GetKeyPos("HallEToPlayerDoorPosition").transform.position;
        if (angry)
            GlobalInfo.i.dad.Die();
    }


    public void UseItemEvent(){}
    public void InteractEvent(){
        StartCoroutine(GlobalUIInfo.i.dialogManager.ShowDialog(info));
    }

    public bool CanUseItemCheck()
    {
        return false;
    }

    public bool CanInteractCheck()
    {
        return !GlobalUIInfo.i.dialogManager.dialogOn && target == null && info != null;
    }

    public Transform Interactable()
    {
        return transform;
    }
}