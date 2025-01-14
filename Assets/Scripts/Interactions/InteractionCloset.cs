using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.XR;

public class InteractionCloset : MonoBehaviour, IInteractable
{
    public SpriteRenderer sr;

    public Sprite closed;
    public Sprite open;
    public Sprite broken;

    public bool opend = false;
    public bool broke = false;
    public bool blocked = false;

    AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }
    public void Block(bool state)
    {
        blocked = state;
    }

    public void UseItemEvent()
    {
        if (blocked)
            return;

        if (Inventory.i.CheckCurrentItem(GlobalInfo.i.bat) && !opend)
        {
            broke = true;
            UpdateSprite();
        }
    }

    public void UpdateSprite()
    {
        sr.sprite = opend ? open : broke ? broken : closed;
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
        return Inventory.i.CheckCurrentItem(GlobalInfo.i.bat) && opend && !broke && !blocked;
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
