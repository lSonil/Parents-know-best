using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;


public class InteractionGasPipe : MonoBehaviour, IInteractable
{
    public GameObject gas;
    public bool blocked;
    public SpriteRenderer cupboard;

    public bool cut = false;
    public Sprite open;
    public Sprite closed;
    public Sprite broken;

    public bool opend = false;
    public bool interacted = false;
    AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();

        gas.SetActive(false);
        ForceEvent(true);
    }
    public void UseItemEvent()
    {
        if (Inventory.i.CheckCurrentItem(GlobalInfo.i.scissors) && !cut && opend)
        {
            cut = true;
            GlobalInfo.i.gasLeak = true;
            gas.SetActive(true);
            UpdateSprite();
        }
    }
    public void InteractEvent()
    {
        if (blocked)
            return;
        audioSource.Play();
        opend = !opend;
        UpdateSprite();
    }
    public void ForceEvent(bool value)
    {
        opend = value;
        cupboard.sprite = opend ? open : closed;
    }
    public void UpdateSprite()
    {

        cupboard.sprite = !opend ? closed : cut ? broken : open;
        interacted = true;
    }

    public bool CanUseItemCheck()
    {
        return Inventory.i.CheckCurrentItem(GlobalInfo.i.scissors) && !cut && opend && !blocked;
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