using System.Collections;
using UnityEngine;


public class InteractionOven : MonoBehaviour, IInteractable
{
    public SpriteRenderer oven;
    public Item itemToSpawn;
    public Transform posToHold = null;

    public Sprite closed;
    public Sprite open;
    public Sprite on;

    public bool opend = false;
    public bool turnedOn = false;
    public bool blocked;

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

        if (Inventory.i.CheckCurrentItem(GlobalInfo.i.lighter))
        {
            StartCoroutine(Flame(5));
            UpdateSprite();

            if (itemToSpawn != null)
            {
                if (itemToSpawn.name != "Junk")
                    Burn();
            }
        }

        if (opend)
        {
            GiveToPlayer();
        }
    }
    public void ForceOn()
    {
        StartCoroutine(Flame(20));
        UpdateSprite();

        if (itemToSpawn != null)
        {
            if (itemToSpawn.name != "Junk")
                Burn();
        }
    }
    IEnumerator Flame(float value)
    {
        turnedOn = true;
        Block(true);
        float i = value;
        while (i >= 0)
        {
            yield return null;
            i -= Time.deltaTime;
        }
        Block(false);
        turnedOn = false;
        UpdateSprite();
    }
    public void SetItemToSpawn()
    {
        if (itemToSpawn != null)
        {
            itemToSpawn.transform.SetParent(posToHold.transform, false);
            itemToSpawn.transform.position = posToHold.position;
            itemToSpawn.GetComponent<Collider2D>().enabled = false; 
            itemToSpawn.GetComponent<SpriteRenderer>().sortingLayerName = oven.sortingLayerName;

            UpdateSprite();
        }
    }
    public void Burn()
    {
        itemToSpawn.transform.SetParent(GetComponentInParent<RoomController>().transform, false);
        itemToSpawn.transform.position = GlobalInfo.i.junk.transform.position;
        itemToSpawn.GetComponent<Collider2D>().enabled = true;
        Item junk = Instantiate(GlobalInfo.i.junk);
        itemToSpawn = junk;
        itemToSpawn.name = "Junk";
        SetItemToSpawn();
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

        transform.GetComponentInParent<RoomController>().CheckDoors();
        return true;
    }

    public bool IsHolding(Item item)
    {
        return item == itemToSpawn;
    }
    public void UpdateSprite()
    {
        if (turnedOn)
            opend = false;

        oven.sprite = opend ? open : turnedOn ? on : closed;
        if (itemToSpawn != null)
        {
            itemToSpawn.GetComponent<SpriteRenderer>().sortingOrder = opend ? oven.sortingOrder + 1 : oven.sortingOrder - 1;
        }
    }
    public void InteractEvent()
    {
        if (blocked)
            return;

        audioSource.Play();

        opend = !opend;
        turnedOn = false;
        UpdateSprite();
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