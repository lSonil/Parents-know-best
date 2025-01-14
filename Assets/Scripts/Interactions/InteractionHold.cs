using UnityEngine;

public class InteractionHold : MonoBehaviour, IInteractable
{
    public Item itemToSpawn;
    public bool blocked;
    public Transform posToHold = null;
    public SpriteRenderer sr;

    private void Start()
    {
        itemToSpawn = GetComponentInChildren<Item>();
        SetItemToSpawn();
    }

    public void UseItemEvent()
    {
        GiveToPlayer();
    }

    public void SetItemToSpawn()
    {
        if(itemToSpawn != null)
        {
            itemToSpawn.transform.SetParent(posToHold.transform, false);
            itemToSpawn.transform.position = posToHold.position;
            itemToSpawn.GetComponent<Collider2D>().enabled = false;

            itemToSpawn.GetComponent<SpriteRenderer>().sortingLayerName = sr.sortingLayerName;
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

    public bool GiveToPlayer()
    {
        if(blocked)
            return false;

        if (itemToSpawn != null)
        {
            itemToSpawn.GetComponent<Item>().PickUp();
            itemToSpawn = null;
        }
        else
        {
            if (Inventory.i.activItem != 0)
            {
                itemToSpawn = Inventory.i.items[Inventory.i.activItem];
                Inventory.i.TakeItemFromInventory(itemToSpawn, transform);
                SetItemToSpawn();
            }
        }

        transform.GetComponentInParent<RoomController>().CheckDoors();
        return true;
    }

    public bool IsHolding(Item item)
    {
        return item == itemToSpawn;
    }
    public void InteractEvent()
    {
        return;
    }

    public bool CanUseItemCheck()
    {
        return (itemToSpawn != null || Inventory.i.activItem != 0) && !blocked;
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
