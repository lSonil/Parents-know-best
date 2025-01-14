using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.Audio;


public class InteractionChair : MonoBehaviour, IInteractable
{
    public Transform spot;
    public Vector3 lastPos;
    public Vector3 startPos;
    public RoomController room;
    public bool isOn=false;
    public bool blocked;
    private Collider2D col;
    public int newSortOrder=0;

    AudioSource audioSource;

    private void Start()
    {
        audioSource= GetComponent<AudioSource>();
        col = GetComponent<Collider2D>();
        room = GetComponentInParent<RoomController>();
        startPos= transform.parent.transform.position;
    }
    private void Update()
    {
        if (isOn && !blocked)
        {
            if(Input.GetKeyDown(KeyCode.Q))
            {
                HopOff();
            }
        }

        if(room!=PlayerHandler.i.room && transform.parent.transform.position != startPos)
        {
            transform.parent.transform.position = startPos;
        }
    }

    public void UseItemEvent(){}

    public void InteractEvent()
    {
        if(!isOn && !PlayerHandler.i.onChair)
        {
            audioSource.Play();

            PlayerHandler.i.col.isTrigger = true;
            lastPos = PlayerHandler.i.transform.position;
            PlayerHandler.i.transform.position = spot.position;
            PlayerHandler.i.gameObject.GetComponent<SpriteRenderer>().sortingOrder = newSortOrder;
            PlayerHandler.i.restricted = true;
            PlayerHandler.i.onChair = true;
            col.enabled = false;
            StartCoroutine(Wait());
        }
    }
    IEnumerator Wait()
    {
        yield return new WaitForSeconds(0.1F);
        isOn = true;

    }
    IEnumerator Stop()
    {
        yield return new WaitForSeconds(0.1F);
        PlayerHandler.i.onChair = false;

    }
    public void HopOff()
    {
        audioSource.Play();

        isOn = false;
        col.enabled = true;
        PlayerHandler.i.transform.position = lastPos;
        PlayerHandler.i.gameObject.GetComponent<SpriteRenderer>().sortingOrder = 0;
        PlayerHandler.i.restricted = false;
        PlayerHandler.i.col.isTrigger = false;
        StartCoroutine(Stop());

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