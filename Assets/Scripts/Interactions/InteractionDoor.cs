using UnityEngine;

public class InteractionDoor : MonoBehaviour, IInteractable
{

    public bool opend = false;
    public SpriteRenderer body;
    public Collider2D col;
    public Sprite open;
    public Sprite closed;

    public void InteractEvent()
    {
        opend = !opend;
        col.isTrigger = opend;
        col.gameObject.layer = opend ? 7 : 3;
        gameObject.layer = opend ? 0 : 3;
        UpdateSprite();
    }
    public void UseItemEvent()
    {
    }

    public void UpdateSprite()
    {
        body.sprite = opend ? open : closed;
    }

    public bool CanUseItemCheck()
    {
        return false;
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
