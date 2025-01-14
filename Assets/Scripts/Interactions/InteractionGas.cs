using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.Audio;


public class InteractionGas : MonoBehaviour, IInteractable
{
    public SpriteRenderer wall;
    public Color wallBurnedColor;
    public SpriteRenderer floor;
    public Sprite floorBurned;
    public InteractionOven oven;

    AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();

    }
    private void Update()
    {
        if(oven.turnedOn)
        {
            Boom();
        }    
    }
    public void Boom()
    {

        audioSource.Play();
        GlobalInfo.i.gasLeak = false;
        wall.color = wallBurnedColor;
        floor.sprite = floorBurned;
        if (PlayerHandler.i.room == GlobalInfo.i.GetRoom("Kitchen"))
        {
            PlayerHandler.i.PlayerLose();
        }
        if (GlobalInfo.i.dad.SameRoom(GlobalInfo.i.GetRoom("Kitchen")))
        {
            GlobalInfo.i.dad.Die();
        }
        if (GlobalInfo.i.mom.SameRoom(GlobalInfo.i.GetRoom("Kitchen")))
        {
            GlobalInfo.i.mom.Die();
        }
        if (GlobalInfo.i.bro.SameRoom(GlobalInfo.i.GetRoom("Kitchen")))
        {
            GlobalInfo.i.bro.Die();
        }
        if (GlobalInfo.i.bby.SameRoom(GlobalInfo.i.GetRoom("Kitchen")))
        {
            GlobalInfo.i.bby.Die();
        }
        Destroy(gameObject);
    }
    public void UseItemEvent()
    {
        if (Inventory.i.CheckCurrentItem(GlobalInfo.i.lighter))
            Boom();
    }
    public void InteractEvent(){}

    public bool CanUseItemCheck()
    {
        return Inventory.i.CheckCurrentItem(GlobalInfo.i.lighter);
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