using System.Collections;
using UnityEngine;

public class NPCBro : SearchAI, IInteractable
{
    [Header("NPC")]
    public bool talking = false;
    public bool eventStarted = false;
    private bool hasRemote = false;
    private bool hasBook = false;
    private bool hasRBook = false;
    private bool hasBat = false;
    private bool batMissing = false;
    private bool hasCandle = false;
    private bool hasLighter = false;
    private bool hasBeer = false;
    public bool drunk = false;
    public bool mercy = false;

    private bool stopIt = false;

    public bool vote = true;

    public Transform lastTarget;

    public Dialog info;

    public void Start()
    {
        ResetSearch(GlobalInfo.i.GetKeyPos("BroChairUsePos").transform);
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
        anim.SetFloat("item", hasRemote? 0.9f : hasLighter ? 0.7f : hasCandle ? 0.4f : hasRBook ? 0.8f : hasBook ? 0.3f : hasBeer ? 0.2f : hasBat ? 0.1f:0);
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
                        bool dadVote = true, momVote = true;
                        if (GlobalInfo.i.dad != null)
                        {
                            dadVote = GlobalInfo.i.dad.vote;
                        }
                        if (GlobalInfo.i.mom != null)
                        {
                            momVote = GlobalInfo.i.mom.vote;
                        }
                        if (vote || dadVote || momVote)
                        {
                            ResetSearch(PlayerHandler.i.transform, true);
                        }
                        else
                        {
                            print("it!");
                        }
                    }
                    break;
                case TimeState.Time20:
                    if (!timeReached[TimePhase.Time20])
                    {
                        timeReached[TimePhase.Time20] = true;
                        stopIt = false;
                        ResetSearch(GlobalInfo.i.GetKeyPos("BroPointUsePos").transform);
                    }
                    break;
                case TimeState.Time18:
                    if (!timeReached[TimePhase.Time18])
                    {
                        timeReached[TimePhase.Time18] = true;
                        if (GlobalInfo.i.powerOutage && (hasBook || hasRBook))
                        {
                            ResetSearch(GlobalInfo.i.GetKeyPos("Box1UsePos").transform);
                        }
                        if (!GlobalInfo.i.powerOutage && (hasCandle))
                        {
                            ResetSearch(GlobalInfo.i.GetKeyPos("BroToHallWDoorPosition").transform);
                        }
                    }
                    break;
                case TimeState.Time16:
                    if (!timeReached[TimePhase.Time16])
                    {
                        timeReached[TimePhase.Time16] = true;
                        if (!GlobalInfo.i.powerOutage)
                        {
                            ResetSearch(GlobalInfo.i.GetKeyPos("Box1UsePos").transform);
                        }
                        stopIt = false;
                    }
                    break;
                case TimeState.Time8:
                    if (mercy && PlayerHandler.i.room != GlobalInfo.i.GetRoom("BroRoom"))
                    {
                        mercy = false;
                    }
                    if(GlobalInfo.i.powerOutage && !eventStarted)
                    {
                        eventStarted = true;
                        StartCoroutine(WaitForOutage());
                    }
                    break;
            }
            yield return null;

            if (SameRoom(GlobalInfo.i.GetRoom("BroRoom")) && PlayerHandler.i.room ==GlobalInfo.i.GetRoom("BroRoom") && !mercy && !hasRemote)
            {
                lastTarget = target;
                if (!batMissing)
                    ResetSearch(GlobalInfo.i.GetKeyPos("BroShelfUsePos").transform);
                else
                {
                    ResetSearch(PlayerHandler.i.transform);
                    StartCoroutine(ChasePlayer());
                }

                stopIt = false;
            }
        }
    }
    public void Die()
    {
        Destroy(gameObject);
    }
    public void Repair()
    {
        hasRemote = true;
    }
    public void FinishRepair()
    {
        hasRemote = false;
        GlobalInfo.i.dad.FinishRepair();
    }

    public void TalkTo()
    {
        talking = true;
    }

    public override void TargetIsInRange()
    {
        InteractionHoldAndOpen holdAndOpen = target.GetComponent<InteractionHoldAndOpen>();
        InteractionChange change = target.GetComponentInParent<InteractionChange>();
        InteractionNode node = target.GetComponentInParent<InteractionNode>();
        Door door = target.GetComponentInParent<Door>();

        transform.position = target.transform.position;

        switch (target.name)
        {
            case "BroChairUsePos":
                StartCoroutine(Routine());
                Stop();
                break;
            case "BroShelfUsePos":
                if (holdAndOpen.IsHolding(GlobalInfo.i.bat))
                {
                    holdAndOpen.GiveItemTo().NPCPickUp(transform);
                    hasBat = true;
                    ResetSearch(PlayerHandler.i.transform, true);
                    StartCoroutine(ChasePlayer());
                }
                else
                {
                    batMissing = true;
                    StartCoroutine(Shock());
                }
                break;
            case "BroToHallWDoorPosition":
                switch (GlobalInfo.i.RoutineToFollow())
                {
                    case TimeState.Time18:
                        door.Open();
                        ResetSearch(GlobalInfo.i.GetKeyPos("KitchenStand2UsePos").transform);
                        break;
                    default:
                        door.Close();
                        ResetSearch(GlobalInfo.i.GetKeyPos("BroChairUsePos").transform);
                        break;

                }
                break;
            case "Box1UsePos":
                if (holdAndOpen.IsHolding(GlobalInfo.i.candle))
                {
                    if (!holdAndOpen.open)
                    {
                        holdAndOpen.ForceEvent(true);
                    }
                    hasCandle = true;
                    holdAndOpen.GiveItemTo().NPCPickUp(transform);
                    ResetSearch(GlobalInfo.i.GetKeyPos("BroToHallWDoorPosition").transform);
                }
                else
                {
                    ResetSearch(GlobalInfo.i.GetKeyPos("CouchRightUsePos").transform);
                }
                break;
            case "KitchenStand2UsePos":
                if (node.IsHolding(GlobalInfo.i.lighter))
                {
                    hasLighter = true;
                    if (GlobalInfo.i.gasLeak)
                        GlobalInfo.i.GetKeyPos("KitchenGasPipeUsePos").GetComponent<InteractionGasPipe>().gas.GetComponent<InteractionGas>().Boom();

                    node.GiveItemTo().NPCPickUp(transform);
                    ResetSearch(GlobalInfo.i.GetKeyPos("BathroomSinkUsePos").transform);
                }
                else
                {
                    GlobalInfo.i.candle.NPCDrop();
                    hasCandle = false;
                    ResetSearch(GlobalInfo.i.GetKeyPos("BroWaitUsePos").transform);
                }
                break;
            case "BathroomSinkUsePos":
                if (!change.changed)
                {
                    if (hasBook)
                    {
                        StartCoroutine(Routine());
                    }
                    else
                    {
                        Die();
                    }
                }
                else
                {
                    ResetSearch(GlobalInfo.i.GetKeyPos("CouchRightUsePos").transform);
                }
                break;
            case "CouchRightUsePos":
            case "BroWaitUsePos":
            case "BroPointUsePos":
                Stop();
                StartCoroutine(Routine());
                break;
            case "CrossTableUsePos":
                if (holdAndOpen.IsHolding(GlobalInfo.i.book))
                {
                    hasBook = true;
                    holdAndOpen.GiveItemTo().NPCPickUp(transform);
                    holdAndOpen.ForceEvent(true);
                    ResetSearch(GlobalInfo.i.GetKeyPos("BroToHallWDoorPosition").transform);
                }
                else if (holdAndOpen.IsHolding(GlobalInfo.i.rbook))
                {
                    hasRBook = true;
                    holdAndOpen.GiveItemTo().NPCPickUp(transform);
                    holdAndOpen.ForceEvent(true);
                    ResetSearch(GlobalInfo.i.GetKeyPos("BroToHallWDoorPosition").transform);
                }
                else
                {
                    ResetSearch(GlobalInfo.i.GetKeyPos("CouchRightUsePos").transform);
                }
                break;
            case "Player":
                if (timeReached[TimePhase.Time21])
                {
                    PlayerHandler.i.PlayerLose();
                    return;
                }

                if (!hasBat)
                {
                    ResetSearch(GlobalInfo.i.GetKeyPos("HallWToBroDoorPosition").transform);
                    StartCoroutine(MovePlayer());
                }
                else
                {
                    PlayerHandler.i.PlayerLose();
                    Stop();
                }
                break;
            default:
                Debug.LogWarning("New location, no action");
                break;
        }
    }

    IEnumerator MovePlayer()
    {
        bool stopIt = true;

        PlayerHandler.i.restricted = true;

        while (stopIt)
        {
            PlayerHandler.i.transform.position = transform.position;
            if (!SameRoom(GlobalInfo.i.GetRoom("BroRoom")))
            {
                PlayerHandler.i.restricted = false;
                stopIt = false;
            }
            yield return null;
        }
        PlayerHandler.i.transform.position = GlobalInfo.i.GetKeyPos("HallWToBroDoorPosition").transform.position;
    }
    IEnumerator Shock()
    {
        Stop();
        yield return new WaitForSeconds(3);
        if (!mercy)
        {
            ResetSearch(PlayerHandler.i.transform);
            StartCoroutine(ChasePlayer());
        }
    }

    IEnumerator WaitForOutage()
    {
        TimeState time = GlobalInfo.i.RoutineToFollow();

        if (time == GlobalInfo.i.RoutineToFollow())
        {
            yield return null;
        }

        ResetSearch(GlobalInfo.i.GetKeyPos("CrossTableUsePos").transform);
        stopIt = false;
    }

    IEnumerator ChasePlayer()
    {
        bool stopIt = true;

        while (stopIt)
        {
            if (mercy || !SameRoom(PlayerHandler.i.room))
            {
                stopIt = false;
            }
            yield return null;
        }
        if (!hasBat)
        {
            if (lastTarget != null)
                ResetSearch(lastTarget);
            else
            {
                if(!mercy)
                    ResetSearch(GlobalInfo.i.GetKeyPos("BroToHallWDoorPosition").transform);
                else
                    ResetSearch(GlobalInfo.i.GetKeyPos("BroChairUsePos").transform);
            }
        }
    }

    public void UseItemEvent()
    {
        if(Inventory.i.CheckCurrentItem(GlobalInfo.i.beer) && !hasBat)
        {
            Inventory.i.DropThisItemFromInventory(GlobalInfo.i.beer);
            GlobalInfo.i.beer.NPCPickUp(transform);
            drunk = true;
            mercy = true;
            vote = false;
        }
    }
    public void InteractEvent(){
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