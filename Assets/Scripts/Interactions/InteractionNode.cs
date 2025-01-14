using UnityEngine;

public class InteractionNode : MonoBehaviour, IInteractable
{
    public Item itemToSpawn;
    public bool blocked;
    public Transform posToHold = null;
    public SpriteRenderer sr;

    public Sprite open;
    public Sprite closed;

    public bool opend = false;
    public bool interacted = false;
    public bool itemchanged = false;

    AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();

        itemToSpawn = GetComponentInChildren<Item>();
        SetItemToSpawn();
    }

    public void UseItemEvent()
    {
        if(opend)
            GiveToPlayer();
    }

    public void SetItemToSpawn()
    {
        if (itemToSpawn != null)
        {
            itemToSpawn.transform.SetParent(posToHold.transform, false);
            itemToSpawn.transform.position = posToHold.position;
            itemToSpawn.GetComponent<Collider2D>().enabled = false;
            itemToSpawn.GetComponent<SpriteRenderer>().sortingLayerName = sr.sortingLayerName;
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

    public void Hold(Item item)
    {
        if (itemToSpawn != null)
            Drop();

        itemchanged = true;

        item.NPCDrop();

        itemToSpawn = item;
        SetItemToSpawn();
    }

    public bool GiveToPlayer()
    {
        if (blocked)
            return false;

        itemchanged = true;

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
        return true;
    }

    public bool IsHolding(Item item)
    {
        return item == itemToSpawn;
    }
    public void InteractEvent()
    {
        if (blocked)
            return;
        audioSource.Play();
        opend = !opend;
        interacted = true;
        UpdateSprite();

    }
    public void ForceEvent(bool value)
    {
        opend = value;
        UpdateSprite();
    }
    public void UpdateSprite()
    {
        sr.sprite = opend ? open : closed;
        if(itemToSpawn!=null)
            itemToSpawn.GetComponent<SpriteRenderer>().sortingOrder = opend ? sr.sortingOrder+1:sr.sortingOrder - 1;
    }

    public bool CanUseItemCheck()
    {
        return opend && (itemToSpawn != null || Inventory.i.activItem != 0) && !blocked;
    }

    public bool CanInteractCheck()
    {
        return !blocked;
    }

    public Transform Interactable()
    {
        return transform;
    }
}
