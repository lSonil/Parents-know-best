using System.Collections;
using UnityEngine;

public class NPCDad : SearchAI, IInteractable
{
    [Header("NPC")]
    private bool interupt=false;
    public bool drunk=false;
    public bool searching = false;
    private bool hasKey = false;
    private bool hasRemote = false;
    private bool hasBaby = false;
    private bool hasBeer = false;
    public bool sitting = false;
    public bool sleep = false;
    public bool repair = false;
    public bool waitForPlayerToLeave = false;
    public bool missing = false;
    public bool atChurch = false;
    public bool inBed = false;
    public bool decide = false;
    public bool tryToRepair = false;

    public bool vote = true;

    public Dialog info;

    private void Start()
    {
        hasKey = true;
        if (hasKey)
        {
            GlobalInfo.i.key.NPCPickUp(transform);
        }
        ResetSearch(GlobalInfo.i.GetKeyPos("CouchLeftUsePos").transform);
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
        anim.SetFloat("item", hasBaby ? 1.1f : hasRemote & hasKey? 0.75f : hasRemote ? 0.9f : hasBeer ? 0.2f : hasKey ? 0.6f : 0);
        anim.SetFloat("drunk", drunk ? 1 : 0);
        anim.SetFloat("sitting", sitting ? 1 : 0);
        anim.SetFloat("asleep", sleep ? 1 : 0);
        anim.SetFloat("inBed", inBed ? 1 : 0);
    }

    IEnumerator Routine()
    {
        bool stopIt = true;

        while (stopIt || interupt)
        {
            switch (GlobalInfo.i.RoutineToFollow())
            {
                case TimeState.Time21:
                    if (!timeReached[TimePhase.Time21])
                    {
                        timeReached[TimePhase.Time21] = true;
                        stopIt = false;
                        bool momVote=true, broVote= true;
                        if (GlobalInfo.i.mom != null)
                        {
                            momVote = GlobalInfo.i.mom.vote;
                        }
                        if (GlobalInfo.i.bro != null)
                        {
                            broVote = GlobalInfo.i.bro.vote;
                        }
                        if(vote || momVote|| broVote)
                        {
                            ResetSearch(PlayerHandler.i.transform, true);
                        }
                        else
                        {
                            print("You");
                        }
                    }
                    break;
                case TimeState.Time20:
                    if (!timeReached[TimePhase.Time20])
                    {
                        print("GO GO GO");
                        timeReached[TimePhase.Time20] = true;
                        stopIt = false;
                        ResetSearch(GlobalInfo.i.GetKeyPos("DadPointUsePos").transform);
                    }
                    break;
                case TimeState.Time17:
                    if (!timeReached[TimePhase.Time17])
                    {
                        timeReached[TimePhase.Time17] = true;
                        if (sleep)
                        {
                            Die();
                            stopIt = false;
                        }

                        decide = true;
                        
                        if (atChurch)
                        {
                            atChurch = false;
                            transform.position = GlobalInfo.i.GetKeyPos("ClockUsePos").transform.position;
                        }    
                        
                        if(!SameRoom(GlobalInfo.i.GetRoom("Livingroom")))
                            ResetSearch(GlobalInfo.i.GetKeyPos("CouchLeftUsePos").transform);

                        stopIt = false;
                    }
                    break;
                case TimeState.Time15:
                    if (!timeReached[TimePhase.Time15])
                    {
                        if (!sleep)
                        {
                            timeReached[TimePhase.Time15] = true;
                            ResetSearch(GlobalInfo.i.GetKeyPos("ClockUsePos").transform);
                            stopIt = false;
                        }
                    }
                    break;

                case TimeState.Time13:
                    if (!timeReached[TimePhase.Time13])
                    {
                        timeReached[TimePhase.Time13] = true;
                        if (SameRoom(GlobalInfo.i.GetRoom("HallE")))
                        {
                            if (drunk)
                                ResetSearch(GlobalInfo.i.GetKeyPos("BedroomBedGetUpUsePos").transform);
                            else
                                ResetSearch(GlobalInfo.i.GetKeyPos("CouchLeftUsePos").transform);
                        }
                    }
                    break;
                case TimeState.Time10:
                    if (!timeReached[TimePhase.Time10])
                    {
                        timeReached[TimePhase.Time10] = true;
                        if (!sleep)
                        {
                            if (!GlobalInfo.i.powerOutage && hasKey)
                            {
                                ResetSearch(GlobalInfo.i.GetKeyPos("StairPaintingUsePos").transform);
                            }
                            else
                            {
                                if (drunk)
                                    ResetSearch(GlobalInfo.i.GetKeyPos("BedroomBedGetUpUsePos").transform);
                                else
                                    ResetSearch(GlobalInfo.i.GetKeyPos("CribUsePos").transform);
                                sitting = false;
                                stopIt = false;
                            }
                        }
                    }
                    if (waitForPlayerToLeave)
                    {
                        if(!SameRoom(PlayerHandler.i.room))
                        {
                            transform.position = GlobalInfo.i.GetKeyPos("AtticSideToAtticEDoorPosition").transform.position;
                            transform.SetParent(GlobalInfo.i.GetRoom("AtticSide").transform);
                            room = GlobalInfo.i.GetRoom("AtticSide");
                            ResetSearch(GlobalInfo.i.GetRoom("AtticSide").transform);
                            waitForPlayerToLeave = false;
                            stopIt = false;
                        }
                    }
                    break;  
                case TimeState.Time9:
                case TimeState.Time8:
                    if (GlobalInfo.i.powerOutage && !sleep && !tryToRepair)
                    {
                        tryToRepair=true;
                        stopIt = false;
                        sitting = false;
                        GlobalInfo.i.GetKeyPos("CouchLeftUsePos").GetComponent<InteractionHold>().Block(false);
                        if (drunk && hasKey)
                        {
                            GlobalInfo.i.GetKeyPos("CouchLeftUsePos").GetComponent<InteractionHold>().NPCHold(GlobalInfo.i.key);
                            hasKey = false;
                        }
                        ResetSearch(GlobalInfo.i.GetKeyPos("GeneratorUsePos").transform);
                    }
                    break;
            }
            yield return null;

            if (SameRoom(PlayerHandler.i.room) && !GlobalInfo.i.powerOutage && !sleep)
            {
                if (Inventory.i.CheckCurrentItem(GlobalInfo.i.remote))
                {
                    stopIt = false;
                    sitting = false;
                    GlobalInfo.i.GetKeyPos("CouchLeftUsePos").GetComponent<InteractionHold>().Block(false);
                    if (drunk && hasKey)
                    {
                        GlobalInfo.i.GetKeyPos("CouchLeftUsePos").GetComponent<InteractionHold>().NPCHold(GlobalInfo.i.key);
                        hasKey = false;
                    }
                    ResetSearch(PlayerHandler.i.transform, true);
                }
            }

        }
    }

    public void RepairSink()
    {
        repair = true;
        ResetSearch(GlobalInfo.i.GetKeyPos("KitchenSinkUsePos").transform);
    }
    public void Drunk()
    {
        drunk = true;
        hasBeer = false;
        interupt = false;
        StartCoroutine(Routine());
    }
    public void Die()
    {
        Destroy(gameObject);
    }
    public void DieBed()
    {
        anim.SetBool("dead", true);
        sr.sortingLayerName="Background";
        transform.position = GlobalInfo.i.GetKeyPos("BedroomBedDeadUsePos").transform.position;
        ResetSearch(GlobalInfo.i.GetKeyPos("BedroomBedDeadUsePos").transform);
    }
    public void WakeUp()
    {
        sleep = false;

        if(SameRoom(GlobalInfo.i.GetRoom("AtticSide")))
        {
            transform.position = GlobalInfo.i.GetKeyPos("StairPaintingUsePos").transform.position;
            room = GlobalInfo.i.GetRoom("HallE");
            transform.SetParent(GlobalInfo.i.GetRoom("HallE").transform);
        }

        ResetSearch(GlobalInfo.i.GetKeyPos("CouchLeftUsePos").transform);
    }
    public void Sleep()
    {
        Stop();
        sleep = true;
    }

    public override void TargetIsInRange()
    {
        InteractionHoldAndOpen holdAndOpen = target.GetComponent<InteractionHoldAndOpen>();
        InteractionChange change = target.GetComponentInParent<InteractionChange>();
        InteractionHold hold = target.GetComponent<InteractionHold>();
        InteractionCrib crib = target.GetComponent<InteractionCrib>();
        InteractionSink sink = target.GetComponent<InteractionSink>();
        InteractionNode node = target.GetComponentInParent<InteractionNode>();
        Door door = target.GetComponentInParent<Door>();

        transform.position = target.transform.position;

        switch(target.name)
        {
            case "CouchLeftUsePos":
                hold.Block(true);
                if (hold.IsHolding(GlobalInfo.i.key))
                {
                    hold.GiveItemTo().NPCPickUp(transform);
                }
                sitting = true;
                StartCoroutine(Routine());
                Stop();
                break;
            case "FatherRepairSpotUsePos":
                GlobalInfo.i.bro.Repair();
                break;
            case "CribUsePos":
                if (!hasBaby)
                {
                    if (crib.IsHolding(GlobalInfo.i.ibby))
                    {
                        if (!missing)
                        {
                            Stop();
                            StartCoroutine(Routine());
                            vote = false;
                            crib.NPCHold(GlobalInfo.i.ibby);
                            hasBaby = true;
                        }
                        else
                        {
                            ResetSearch(GlobalInfo.i.GetKeyPos("BedroomBedGetUpUsePos").transform);
                        }
                    }
                    else
                    {
                        missing = true;
                        ResetSearch(GlobalInfo.i.bby.transform);
                    }
                }
                else
                {
                    crib.NPCHold(GlobalInfo.i.ibby);
                    crib.Repair();
                    hasBaby = false;
                    GlobalInfo.i.ibby.enabled = true;
                    ResetSearch(GlobalInfo.i.GetKeyPos("CribUsePos").transform);
                }
                break;
            case "BedroomBedSleepUsePos":
                Sleep();
                inBed = true;
                StartCoroutine(Routine());
                break;
            case "BedroomBedDeadUsePos":
                Stop();
                break;
            case "BedroomBedGetUpUsePos":
                Stop();
                ResetSearch(GlobalInfo.i.GetKeyPos("BedroomBedSleepUsePos").transform);
                transform.position = GlobalInfo.i.GetKeyPos("BedroomBedSleepUsePos").transform.position;
                break;
            case "BathroomSinkUsePos":
                drunk = false;
                ResetSearch(GlobalInfo.i.GetKeyPos("KitchenSinkUsePos").transform);
                break;
            case "KitchenSinkUsePos":
                repair = false;
                sink.Repair();
                ResetSearch(GlobalInfo.i.GetKeyPos("CouchLeftUsePos").transform);
                break;
            case "BbyNPC":
                GlobalInfo.i.ibby.NPCPickUp(transform);
                GlobalInfo.i.ibby.enabled = false;
                hasBaby = true;
                GlobalInfo.i.bby.Take();
                ResetSearch(GlobalInfo.i.GetKeyPos("CribUsePos").transform);
                break;
            case "GlassTableUsePos":
                hold.NPCHold(GlobalInfo.i.remote);
                ResetSearch(GlobalInfo.i.GetKeyPos("CouchLeftUsePos").transform);
                hasRemote = false;
                break;
            case "GeneratorUsePos":
                StartCoroutine(RepairGenerator());
                break;
            case "StairPaintingUsePos":
                Stop();
                waitForPlayerToLeave = true;
                break;
            case "ClockUsePos":
                if (SameRoom(PlayerHandler.i.room))
                {
                    ResetSearch(PlayerHandler.i.transform, true);
                }
                else
                {
                    atChurch = true;
                    target = null;

                    ResetSearch(GlobalInfo.i.GetKeyPos("ChurchUsePos").transform);
                    transform.position = GlobalInfo.i.GetKeyPos("ChurchUsePos").transform.position;
                }
                break;
            case "DadPointUsePos":
                Stop();
                StartCoroutine(Routine());
                break;
            case "AtticSide":
                sleep = true;
                Stop();
                StartCoroutine(Routine());

                break;
            case "Player":
                if (timeReached[TimePhase.Time21])
                {
                    PlayerHandler.i.PlayerLose();
                    return;
                }

                ResetSearch(GlobalInfo.i.GetKeyPos("PlayerToHallEDoorPosition").transform);


                if (Inventory.i.FindItemIndex(GlobalInfo.i.key).itemFound)
                {
                    Inventory.i.DropThisItemFromInventory(GlobalInfo.i.key);
                    GlobalInfo.i.key.NPCPickUp(transform);
                    hasKey = true;
                }

                if (Inventory.i.FindItemIndex(GlobalInfo.i.remote).itemFound)
                {
                    Inventory.i.DropThisItemFromInventory(GlobalInfo.i.remote);
                    GlobalInfo.i.remote.NPCPickUp(transform);
                    hasRemote = true;
                    ResetSearch(GlobalInfo.i.GetKeyPos("GlassTableUsePos").transform);
                }
                else
                {
                    StartCoroutine(GroundPlayer());
                }

                break;
            default:
                Debug.LogWarning("New location, no action");
                break;
        }
    }
    IEnumerator RepairGenerator()
    {
        Stop();
        yield return new WaitForSeconds(5);
        InteractionGenerator generator = GlobalInfo.i.GetKeyPos("GeneratorUsePos").GetComponent<InteractionGenerator>();
        if (generator.burned && drunk)
        {
            Die();
        }
        else
        if (drunk)
        {
            generator.Repair();
            Sleep();
        }
        else
        if (generator.burned)
        {
            ResetSearch(GlobalInfo.i.GetKeyPos("CribUsePos").transform);
        }
        else
        {
            generator.Repair();
            ResetSearch(GlobalInfo.i.GetKeyPos("CouchLeftUsePos").transform);
        }

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

        if (GlobalInfo.i.RoutineToFollow() != TimeState.Time22)
        {
            if (GlobalInfo.i.remote.GetComponentInParent<InteractionHold>() != null)
            {
                GlobalInfo.i.remote.GetComponentInParent<InteractionHold>().GiveItemTo().NPCPickUp(transform);
            }
            else
            {
                GlobalInfo.i.remote.NPCPickUp(transform);
            }
            hasRemote= true;
            ResetSearch(GlobalInfo.i.GetKeyPos("FatherRepairSpotUsePos").transform);
        }
        else
        {
            GlobalInfo.i.GetKeyPos("PlayerToHallEDoorPosition").GetComponentInParent<Door>().Close();
            ResetSearch(GlobalInfo.i.GetKeyPos("ClockUsePos").transform);
            transform.position = GlobalInfo.i.GetKeyPos("HallToPlayerDoorPosition").transform.position;
        }
    }

    public void FinishRepair()
    {
        ResetSearch(GlobalInfo.i.GetKeyPos("CouchLeftUsePos").transform);
    }

    public void UseItemEvent()
    {
        if (Inventory.i.CheckCurrentItem(GlobalInfo.i.beer) && sitting)
        {
            Inventory.i.DropThisItemFromInventory(GlobalInfo.i.beer);
            GlobalInfo.i.beer.NPCPickUp(transform);
            interupt = true;
            hasBeer = true;
        }
    }

    public void InteractEvent()
    {
        StartCoroutine(GlobalUIInfo.i.dialogManager.ShowDialog(info));
    }

    public bool CanUseItemCheck()
    {
        return Inventory.i.CheckCurrentItem(GlobalInfo.i.beer);
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