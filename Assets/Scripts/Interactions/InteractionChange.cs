using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using UnityEngine;
using static Unity.Collections.AllocatorManager;


public class InteractionChange : MonoBehaviour, IInteractable
{
    public SpriteRenderer sr;
    public Sprite changeToSprite;
    public GameObject pseudo;
    public Item itemToUse;
    public bool wakesUpDad;

    public bool blocked = false;
    public bool changed = false;

    private void Start()
    {
        if (pseudo != null)
            pseudo.SetActive(false);
    }
    public void Change()
    {
        if(pseudo!= null)
        {
            pseudo.SetActive(true);
        }
        sr.sprite = changeToSprite;
        changed = true;
        if(wakesUpDad && GlobalInfo.i.dad.sleep)
        {
            GlobalInfo.i.dad.WakeUp();
        }
        gameObject.SetActive(false);
    }
    public void Block(bool state)
    {
        blocked = state;
    }

    public void UseItemEvent()
    {
        if (!Inventory.i.CheckCurrentItem(itemToUse) || changed)
            return;

        Change();
    }
    public void InteractEvent()
    {
        throw new System.NotImplementedException();
    }

    public bool CanUseItemCheck()
    {
        return Inventory.i.CheckCurrentItem(itemToUse) && !changed && !blocked;
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