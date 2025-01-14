using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCCrawler : SearchAI
{
    public float maxSpeed = 5f;
    bool retreat = false;
    public bool found = false;
    public GameObject goBack;
    public List<Transform> spots;
    public Animator eyes;
    public Collider2D proximity;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(WaitToFollow());
    }
    IEnumerator WaitToFollow()
    {
        yield return new WaitForSeconds(5);
        anim.SetTrigger("getOut");
        eyes.SetTrigger("getOut");
    }


    public void GetOut()
    {
        sr.sortingLayerName = "Middleground";
        sr.sortingOrder = 1;
        anim.SetBool("isMoving", true);
        eyes.SetBool("isMoving", true);
        proximity.enabled=true;

        ResetSearch(GetRandomSpot());
        StartCoroutine(Watch());
    }

    IEnumerator BuildUp()
    {
        while(!found)
        {
            aiPathController.maxSpeed += Time.deltaTime;
            yield return new WaitForSeconds(0.01f);
            aiPathController.maxSpeed = Mathf.Clamp(aiPathController.maxSpeed, speed, maxSpeed);
        }
        aiPathController.maxSpeed = speed;
    }

    IEnumerator Watch()
    {
        found = false;
        bool candle=false;
        bool lighter = false;
        while(!candle && !lighter && target!= PlayerHandler.i.transform)
        {
            lighter = CheckForLight(GlobalInfo.i.lighter);
            candle = CheckForLight(GlobalInfo.i.candle);

            yield return null;
        }

        StartCoroutine(BuildUp());

        while (true)
        {
            if((lighter && !GlobalInfo.i.lighter.GetComponent<Lighter>().on) ||
                (candle && !GlobalInfo.i.candle.GetComponent<Lighter>().on))
            {
                ResetSearch(GetCloseSpot());
                StartCoroutine(Watch());
                break;
            }
            yield return null;
        }
    }

    public bool CheckForLight(Item source)
    {
        if (source.GetComponentInParent<RoomController>() == room && source.GetComponent<Lighter>().on)
        {
            if (Inventory.i.CheckCurrentItem(source))
            {
                ResetSearch(PlayerHandler.i.transform, true);
            }
            else
                ResetSearch(source.transform);
            return true;
        }
        return false;
    }
    // Update is called once per frame
    void Update()
    {
        if(!retreat && !goBack.activeInHierarchy)
        {
            retreat = true;
            ResetSearch(GlobalInfo.i.GetKeyPos("BasementSWToBasementSEDoorPosition").transform);
        }
        AnimationHandle();
    }
    public void AnimationHandle()
    {
        anim.SetFloat("moveX", moveX);
        eyes.SetFloat("moveX", moveX);
        anim.SetFloat("moveY", moveY);
        eyes.SetFloat("moveY", moveY);
    }

    public override void TargetIsInRange()
    {
        found = true;
        transform.position = target.position;
        if (target == PlayerHandler.i.transform)
        {
            PlayerHandler.i.PlayerLose();
            Stop();
            return;
        }

        if (target == GlobalInfo.i.candle.transform)
        {
            if (Inventory.i.FindItemIndex(GlobalInfo.i.candle).itemFound)
                ResetSearch(PlayerHandler.i.transform, true);
            else
            {
                GlobalInfo.i.candle.GetComponent<Lighter>().ForceOff();
                GlobalInfo.i.candle.NPCPickUp(transform);
            }
        }

        if (target == GlobalInfo.i.lighter.transform)
        {
            if (Inventory.i.FindItemIndex(GlobalInfo.i.lighter).itemFound)
                ResetSearch(PlayerHandler.i.transform, true);
            else
            {
                GlobalInfo.i.lighter.GetComponent<Lighter>().ForceOff();
                GlobalInfo.i.lighter.NPCPickUp(transform);
            }
        }

        ResetSearch(GetRandomSpot());

    }
    public void OnTriggerEnter2D(Collider2D collision)
    {
        found = false;
        if (collision.CompareTag("Player") && PlayerHandler.i.IsMoving())
        {
            StartCoroutine(BuildUp());
            ResetSearch(PlayerHandler.i.transform, true);
        }
    }

    public Transform GetRandomSpot()
    {
        return spots[UnityEngine.Random.Range(0, spots.Count)];
    }

    public Transform GetCloseSpot()
    {
        Transform goTo = spots[0];
        found = false;  
        foreach (Transform t in spots)
        {
            if(Vector2.Distance(PlayerHandler.i.transform.position, t.position) < 
                Vector2.Distance(PlayerHandler.i.transform.position, goTo.position) && 
                t!=target)
            {
                goTo = t;
            }
        }
        return goTo;
    }
}
