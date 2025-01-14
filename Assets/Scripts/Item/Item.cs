using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Item : MonoBehaviour, IInteractable
{

    public bool consumable;

    public delegate void Function();
    public Function function;

    private void Awake()
    {
        GetComponent<Collider2D>().isTrigger = true;
        GetComponent<Collider2D>().enabled = true;
    }
    public void UseItemEvent()
    {
        if (function != null)
            function();
    }
    public bool PickUp()
    {
        Inventory.i.AddItem(this);
        return true;
    }
    public void NPCPickUp(Transform newOwner)
    {
        gameObject.transform.SetParent(newOwner, true);
        transform.position = newOwner.position;
        gameObject.GetComponent<SpriteRenderer>().enabled = false;
        gameObject.GetComponent<Collider2D>().enabled = false;
    }
    public void NPCDrop()
    {
        Transform owner = transform.parent;
        gameObject.transform.SetParent(owner.parent, true);
        transform.position = owner.position;
        gameObject.GetComponent<SpriteRenderer>().enabled = true;
        gameObject.GetComponent<Collider2D>().enabled = true;
    }

    public void InteractEvent()
    {
        PickUp();
    }

    public bool CanUseItemCheck()
    {
        return false;
    }

    public bool CanInteractCheck()
    {
        return true;
    }

    public Transform Interactable()
    {
        return transform;
    }
}
