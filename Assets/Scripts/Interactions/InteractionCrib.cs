using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using UnityEngine;


public class InteractionCrib : MonoBehaviour, IInteractable
{
    public SpriteRenderer sr;
    public Item itemToSpawn;

    public bool cut = false;
    public bool repair = false;

    public bool blocked;
    public Transform posToHold = null;

    public Sprite closed;
    public Sprite broken;
    public Sprite repaired;

    private void Start()
    {

        itemToSpawn = GlobalInfo.i.ibby; 
        SetItemToSpawn();
    }

    public void UseItemEvent()
    {
        if (blocked)
            return;

        if (Inventory.i.CheckCurrentItem(GlobalInfo.i.scissors) && !cut)
        {
            Cut();
        }
    }

    public void SetItemToSpawn()
    {
        if (itemToSpawn != null)
        {
            itemToSpawn.transform.SetParent(posToHold.transform, false);
            itemToSpawn.transform.position = posToHold.position;
            itemToSpawn.GetComponent<Collider2D>().enabled = false;
            UpdateSprite();
        }
    }

    public void Block(bool state)
    {
        blocked = state;
    }

    public Item GiveItemTo()
    {
        if (itemToSpawn != null)
        {
            Item item = itemToSpawn;
            itemToSpawn = null;
            return item;
        }
        return null;
    }

    public void Drop()
    {
        itemToSpawn.transform.SetParent(GetComponentInParent<RoomController>().transform, false);
        itemToSpawn.transform.position = transform.position;
        itemToSpawn.GetComponent<Collider2D>().enabled = true;
        itemToSpawn.GetComponent<SpriteRenderer>().sortingLayerName = "MiddleGround";
        itemToSpawn.GetComponent<SpriteRenderer>().sortingOrder = 0;
        itemToSpawn = null;
    }

    public void NPCHold(Item item)
    {
        if (itemToSpawn != null)
            Drop();

        item.NPCDrop();

        itemToSpawn = item;
        SetItemToSpawn();
    }
    public bool IsHolding(Item item)
    {
        return item == itemToSpawn;
    }
    public void InteractEvent(){}

    public void UpdateSprite()
    {
        sr.sprite = repair ? repaired : cut? broken : closed;
    }

    public void Cut()
    {
        cut = true;
        Drop();
        GlobalInfo.i.bby.Free();
        sr.sprite = broken;
    }
    public void Repair()
    {
        repair = true;
        sr.sprite = repaired;
    }

    public bool CanUseItemCheck()
    {
        return Inventory.i.CheckCurrentItem(GlobalInfo.i.scissors) && !cut && !blocked;
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