using System;
using System.Linq;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public Item[] items = new Item[1];

    public static Inventory i;

    AudioSource audioSource;
    public AudioClip pickUpSound;
    public AudioClip dropSound;

    public int activItem=0;
    bool update;
    private void Awake()
    {
        i = this;
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        GlobalUIInfo.i.DropKey(false);
    }

    private void Update()
    {
        if (items.Length >= 2 || activItem >= items.Length)
        {
            if (Input.GetAxis("Mouse ScrollWheel") != 0f)
            {
                activItem += Input.GetAxis("Mouse ScrollWheel") < 0 ? -1 : 1;

                if (activItem >= items.Length)
                {
                    activItem = 0;
                }
                // Loop back to the last position if currentIndex goes below 0
                else if (activItem < 0)
                {
                    activItem = items.Length - 1;
                }

                update = true;
            }
            if (activItem >= items.Length)
            {
                activItem = items.Length - 1;
            }
        }

        if (update)
        {
            GlobalUIInfo.i.UpdateUI();
            update = false;
            if (activItem != 0)
                GlobalUIInfo.i.DropKey(true);
            else
                GlobalUIInfo.i.DropKey(false);

        }
    }

    // when you want to drop
    public void DropCurrentItemFromInventory()
    {
        if(items.Length > 1)
            DropItem(activItem);
    }

    public void DropItem(int itemNumber)
    {
        if (itemNumber == 0 && itemNumber< items.Length)
            return;

        audioSource.resource = dropSound;
        audioSource.Play();

        Item[] newInventory = new Item[items.Length];
        Item[] finalInventory = new Item[items.Length-1];

        items[itemNumber].gameObject.transform.SetParent(PlayerHandler.i.transform.parent, true);
        items[itemNumber].gameObject.GetComponent<SpriteRenderer>().enabled = true;
        items[itemNumber].GetComponent<Collider2D>().enabled = true;

        items[itemNumber] = null;

        newInventory = items.Where(item => item != null).ToArray();

        // Copy elements from the original array to the new array
        for (int i = 0; i < finalInventory.Length-1; i++)
        {
            finalInventory[i+1] = newInventory[i];
        }
        items = finalInventory;
        update = true;
    }

    // NPC leaves an item at that location
    public void DropThisItemFromInventory(Item itemInInventory)
    {
        if(items.Contains(itemInInventory))
        {
            int positionofItemInInventory = Array.IndexOf(items, itemInInventory);

            DropItem(positionofItemInInventory);
        }
    }

    // NPC takes item from you
    public bool DestroySpecifiedItem(Item itemInInventory)
    {
        var result = FindItemIndex(itemInInventory);
        int index = result.index;
        bool itemFound = result.itemFound;

        // if not found nothing to destroy
        if (!itemFound) return itemFound;

        items[index] = null;
        update = true;
        return itemFound;
    }

    // used by NPC to check if the wanted item is still in the inventory and at what position
    public (int index, bool itemFound) FindItemIndex(Item itemInInventory)
    {
        int index = 0;
        foreach (Item itm in items)
        {
            if (itm == itemInInventory)
            {
                return (index, true);
            }
            index++;
        }
        return (-1, false); // Returns -1 if item is not found
    }

    public Item TakeItemFromInventory(Item key, Transform newOwner)
    {
        DropThisItemFromInventory(key);
        key.gameObject.transform.SetParent(newOwner, true);
        key.gameObject.transform.position += new Vector3(0, 0, 0);
        update = true;
        return key;
    }
    // used by NPC to check if trigger item is in main hand
    public bool CheckCurrentItem(Item itemToCheck)
    {
        if (items.Contains(itemToCheck))
        {
            int positionofItemInInventory = Array.IndexOf(items, itemToCheck);

            if(positionofItemInInventory == activItem)
                return true;
        }

        return false;
    }

    public void AddItem(Item itemToAdd)
    {
        audioSource.resource = pickUpSound;
        audioSource.Play();

        Item[] newItemInInventory = new Item[items.Length + 1];

        // Copy elements from the original array to the new array
        for (int i = 0; i < items.Length; i++)
        {
            newItemInInventory[i] = items[i];
        }

        itemToAdd.gameObject.GetComponent<SpriteRenderer>().enabled = false;
        itemToAdd.GetComponent<Collider2D>().enabled = true;
        items = newItemInInventory;
        items[items.Length - 1] = itemToAdd;
        activItem = items.Length - 1;

        itemToAdd.transform.SetParent(this.transform);
        itemToAdd.transform.localPosition = new Vector3(0, 0, 0);

        update = true;
        GlobalUIInfo.i.DisplayItem(itemToAdd.name);
    }
}
