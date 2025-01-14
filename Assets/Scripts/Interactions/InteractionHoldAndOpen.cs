using UnityEngine;

public class InteractionHoldAndOpen : MonoBehaviour, IInteractable
{
    public Item itemToSpawn;
    public Item key;
    public bool blocked;
    public bool triggered;
    public Transform posToHold = null;
    public SpriteRenderer body;

    public Sprite open;
    public Sprite closed;

    public bool opend = false;
    AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();

        itemToSpawn = GetComponentInChildren<Item>();
        SetItemToSpawn();    
    }

    public void UseItemEvent()
    {
        if (blocked)
            return;

        if (opend)
            GiveToPlayer();
        else
        {
            if (Inventory.i.CheckCurrentItem(key) && key != null)
            {
                opend = !opend;
                UpdateSprite();
            }
        }
    }

    public void SetItemToSpawn()
    {
        if(itemToSpawn != null)
        {
            itemToSpawn.transform.SetParent(posToHold.transform, false);
            itemToSpawn.transform.position = posToHold.position;
            itemToSpawn.GetComponent<Collider2D>().enabled = false;
            itemToSpawn.GetComponent<SpriteRenderer>().sortingLayerName = body.sortingLayerName;
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

        item.NPCDrop();

        itemToSpawn = item;
        SetItemToSpawn();
    }

    public bool GiveToPlayer()
    {
        if (blocked)
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
        return true;
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
    public void InteractEvent()
    {
        if (blocked)
            return;

        if (key == null|| (key != null && Inventory.i.CheckCurrentItem(key)))
        {
            audioSource.Play();
            triggered = true;
            opend = !opend;
            UpdateSprite();
        }
    }

    public void ForceEvent(bool value)
    {
        opend = value;
        triggered = true;
        UpdateSprite();
    }

    public void UpdateSprite()
    {
        body.sprite = opend ? open : closed;
        if (itemToSpawn != null)
        {
            itemToSpawn.GetComponent<SpriteRenderer>().sortingOrder = opend ? body.sortingOrder + 1 : body.sortingOrder - 1;
        }
        foreach (Transform obj in transform)
        {
            if (obj != posToHold)
                obj.gameObject.SetActive(opend);
        }
    }

    public bool CanUseItemCheck()
    {
        return ((opend && (itemToSpawn != null || Inventory.i.activItem != 0)) || (key != null && Inventory.i.CheckCurrentItem(key))) && !blocked;
    }

    public bool CanInteractCheck()
    {
        return !blocked && key == null;
    }
    public Transform Interactable()
    {
        return transform;
    }
}
