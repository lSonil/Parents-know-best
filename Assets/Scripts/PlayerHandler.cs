using Cinemachine;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
public class PlayerHandler : MonoBehaviour
{
    public static PlayerHandler i;

    private float actualSpeed = 2.5f;  // Speed of the character
    public Vector3 speed = Vector3.zero;
    private Vector2 movement;

    public bool crawlingUnder;
    public bool crouch;
    public bool onChair;
    public bool searching;

    public RoomController room;
    public bool inAction = false;
    public bool restricted = false;

    public static bool flip;
    public float moveX;
    public float moveY;

    private Animator anim;
    private Rigidbody2D rb;
    public Collider2D col;
    public SpriteRenderer sr;
    public List<IInteractable> eventObjects = new List<IInteractable>();
    private void Awake()
    {
        i = this;
    }
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        col = GetComponent<Collider2D>();
    }

    public bool IsMoving()
    {
        return movement != Vector2.zero;
    }
    void Update()
    {
        if (GlobalInfo.i.gameOver || GlobalUIInfo.i.pause)
            return;
        movement = new Vector2(0, 0);

        if (!restricted && !inAction && Time.timeScale != 0 && !searching)
        {
            CrawlHandle();

            // Get horizontal input (A/D keys or Left/Right arrows)
            float moveInputX = Input.GetAxisRaw("Horizontal");
            float moveInputY = Input.GetAxisRaw("Vertical");

            if (crouch)
            {
                actualSpeed = speed.x;
            }
            else
            if (Input.GetKey(KeyCode.LeftShift))
            {
                actualSpeed = speed.z;
            }
            else
            {
                actualSpeed = speed.y;
            }

            if (moveInputX != 0 || moveInputY != 0)
            {
                moveX = moveInputX;

                moveY = moveInputY;
            }
            // Create movement vector
            movement = new Vector2(moveInputX, moveInputY).normalized * actualSpeed;

        }

        rb.linearVelocity = movement;

        // Animate
        AnimationHandle();
        CheckForAction();

        // Interact
    }

    public void PlayerLose()
    {
        rb.linearVelocity = Vector2.zero;
        GlobalInfo.i.gameOver = true;
        GlobalUIInfo.i.GameOver();
        anim.SetBool("dead", true);
    }

    public void CrawlHandle()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl) && !crawlingUnder)
        {
            crouch = !crouch;
        }
    }

    public void AnimationHandle()
    {
        anim.SetFloat("item", (float)GlobalInfo.i.items.IndexOf(Inventory.i.items[Inventory.i.activItem]) / 10);
        anim.SetBool("isMoving", Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0);
        anim.SetFloat("moveX", moveX);
        anim.SetFloat("moveY", moveY);
        bool isOn = false;
        if (Inventory.i.CheckCurrentItem(GlobalInfo.i.lighter))
        {
            isOn = GlobalInfo.i.lighter.GetComponent<Lighter>().on;
        }
        if (Inventory.i.CheckCurrentItem(GlobalInfo.i.candle))
        {
            isOn = GlobalInfo.i.candle.GetComponent<Lighter>().on;
        }
        anim.SetFloat("isOn", isOn ? 1 : 0);
    }

    public void CheckForAction()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GlobalUIInfo.i.ShowMenu();
            return;
        }

        if (eventObjects.Count > 0)
        {
            float distance = Vector3.Distance(transform.position, eventObjects[0].Interactable().position);
            IInteractable target = eventObjects[0];

            foreach (IInteractable eventObj in eventObjects)
            {
                if (distance > Vector3.Distance(transform.position, eventObj.Interactable().position))
                {
                    target = eventObj;
                }
            }
            if (target != null)
            {
                GlobalUIInfo.i.UseKey(target.CanUseItemCheck());

                if (Input.GetKeyDown(KeyCode.E))
                {
                    if (target.CanUseItemCheck())
                        target.UseItemEvent();
                }

                GlobalUIInfo.i.ShowKey(target.CanInteractCheck());

                if (Input.GetKeyDown(KeyCode.Space) && target.CanInteractCheck())
                    target.InteractEvent();
            }
        }
        else
        {
            GlobalUIInfo.i.UseKey(false);
            GlobalUIInfo.i.ShowKey(false);
        }

        if (Input.GetKeyDown(KeyCode.E) && Inventory.i.activItem != 0)
            Inventory.i.items[Inventory.i.activItem].UseItemEvent();

        if (onChair)
            GlobalUIInfo.i.DropKey(false);

        if (restricted)
            return;

        if (Input.GetKeyDown(KeyCode.Q))
        {
            Inventory.i.DropCurrentItemFromInventory();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<RoomController>() != null)
        {
            room = collision.GetComponent<RoomController>();
            transform.SetParent(collision.transform);
            GlobalUIInfo.i.vc.GetComponent<CinemachineConfiner>().m_BoundingShape2D = room.gameObject.GetComponent<Collider2D>();
            GlobalUIInfo.i.vc.m_Lens.OrthographicSize = room.camerSize;
        }
        if (collision.GetComponent<IInteractable>() != null)
        {
            if (!eventObjects.Contains(collision.GetComponent<IInteractable>()))
            {
                eventObjects.Add(collision.GetComponent<IInteractable>());
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        IInteractable eventObj = collision.GetComponent<IInteractable>();
        if (eventObj != null)
        {
            if (eventObjects.Contains(eventObj))
            {
                eventObjects.Remove(eventObj);
            }
        }
    }
}