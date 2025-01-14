using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoverHandler : MonoBehaviour
{
    public Collider2D body;
    public bool isUsed;
    // Update is called once per frame

    private void Start()
    {
        body = GetComponent<Collider2D>();
    }
    void Update()
    {
        if (PlayerHandler.i.crouch || isUsed)
        {
            body.isTrigger = true;
        }
        else
        {
            body.isTrigger = false;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isUsed = false;
            PlayerHandler.i.crawlingUnder = false;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isUsed = true;
            PlayerHandler.i.crawlingUnder = true;
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isUsed = true;
            PlayerHandler.i.crawlingUnder = true;
        }
    }
}
