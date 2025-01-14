using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using static Unity.Collections.AllocatorManager;


public class InteractionGenerator : MonoBehaviour, IInteractable
{
    public SpriteRenderer sr;
    public Sprite spriteClosed;
    public Sprite spriteOpen;
    public Sprite spriteRepair;
    public Sprite spriteBurn;
    public Sprite spriteCut;

    public bool opend = false;
    public bool cut = false;
    public bool burned = false;
    public bool repaired = false;

    public bool blocked = false;

    AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();

    }
    public void Cut()
    {
        cut = true;
        GlobalInfo.i.powerOutage = true;
        sr.sprite = spriteCut;
    }
    public void Burn()
    {
        burned = true;
        sr.sprite = spriteBurn;
    }
    public void Repair()
    {
        repaired = true;
        GlobalInfo.i.powerOutage = false;
        UpdateSprite();
    }

    public void UpdateSprite()
    {
        sr.sprite = !opend ? spriteClosed : !cut ? spriteOpen : repaired ? spriteRepair : burned ? spriteBurn : spriteCut;
    }


    public void UseItemEvent()
    {
        if (blocked || !opend)
            return;

        if (Inventory.i.CheckCurrentItem(GlobalInfo.i.scissors) && !cut)
            Cut();
        if (Inventory.i.CheckCurrentItem(GlobalInfo.i.lighter) && cut && !burned)
            Burn();

        UpdateSprite();
    }
    public void InteractEvent()
    {
        if (blocked)
            return;

        audioSource.Play();

        opend = !opend;
        UpdateSprite();
    }

    public bool CanUseItemCheck()
    {
        return (Inventory.i.CheckCurrentItem(GlobalInfo.i.scissors) && !blocked && !cut && opend) || (Inventory.i.CheckCurrentItem(GlobalInfo.i.lighter) && !blocked && cut && !burned && opend);
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