using System.Collections;
using UnityEngine;

public class NPCBby : SearchAI, IInteractable
{
    public bool eat;
    public bool cry;
    public bool free;
    private bool seeChoco = false;

    public void UseItemEvent()
    {
    }

    public void Update()
    {
        if(free && !cry && !seeChoco)
        {
            if (SameRoom(PlayerHandler.i.room) && Inventory.i.CheckCurrentItem(GlobalInfo.i.chocolate))
            {
                StartCoroutine(Follow());
            }
            if (GlobalInfo.i.chocolate.GetComponentInParent<InteractionHold>() == null &&
                GlobalInfo.i.chocolate.GetComponentInParent<InteractionHoldAndOpen>() == null &&
                SameRoom(GlobalInfo.i.chocolate.GetComponentInParent<RoomController>()) &&
                GlobalInfo.i.chocolate.GetComponent<SpriteRenderer>().enabled)
            {
                StartCoroutine(Follow());
            }
        }
        AnimationHandle();
    }

    IEnumerator Follow()
    {
        seeChoco = true;
        while (seeChoco && free && !cry)
        {
            seeChoco = false;
            if (Inventory.i.CheckCurrentItem(GlobalInfo.i.chocolate))
            {
                seeChoco= true;
                ResetSearch(PlayerHandler.i.transform);
            }
            if (GlobalInfo.i.chocolate.GetComponentInParent<InteractionHold>() == null &&
                GlobalInfo.i.chocolate.GetComponentInParent<InteractionHoldAndOpen>() == null &&
                SameRoom(GlobalInfo.i.chocolate.GetComponentInParent<RoomController>()) &&
                GlobalInfo.i.chocolate.GetComponent<SpriteRenderer>().enabled)
            {
                seeChoco = true;
                ResetSearch(GlobalInfo.i.chocolate.transform);
            }
            yield return null;
        }
        ResetSearch(transform);
    }

    public void AnimationHandle()
    {
        anim.SetBool("isMoving", aiPathTarget.target != null);
        anim.SetFloat("moveX", moveX);
        anim.SetFloat("cry", cry ? 1 : 0);
        anim.SetFloat("eat", eat ? 1: 0);
    }

    // Update is called once per frame

    public void Scare()
    {
        print(2);
        cry = true;
        ResetSearch(GlobalInfo.i.GetRoom("HallW").transform);
    }
    public void Take()
    {
        free = false;
    }
    public void Free()
    {
        free = true;
    }

    public void Die()
    {
        Destroy(gameObject);
        Stop();
    }

    public override void TargetIsInRange()
    {
        if(target == GlobalInfo.i.chocolate.transform)
        {
            Stop();
            eat = true;
        }
        if (target == GlobalInfo.i.GetRoom("HallW").transform)
        {
            Stop();
            cry = false;
        }
    }
    public void InteractEvent(){}

    public bool CanUseItemCheck()
    {
        return false;
    }

    public bool CanInteractCheck()
    {
        return false;
    }

    public Transform Interactable()
    {
        return transform;
    }
}
