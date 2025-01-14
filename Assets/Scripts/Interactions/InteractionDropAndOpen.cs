using Unity.VisualScripting;
using UnityEngine;
using static Unity.Collections.AllocatorManager;

public class InteractionDropAndOpen : MonoBehaviour, IInteractable
{
    public SpriteRenderer sr;

    public Sprite closed;
    public Sprite open;
    public GameObject page;

    public bool opend = false;
    public bool blocked = false;

    AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();

        if (page != null)
            page.SetActive(false);
    }

    public void Block(bool state)
    {
        blocked = state;
    }

    public void UseItemEvent(){}

    public void UpdateSprite()
    {
        sr.sprite = opend ? open : closed;
    }

    public void InteractEvent()
    {
        if (blocked)
            return;
        if(page!=null && !page.activeInHierarchy)
            page.SetActive(true);
        else
        {
            audioSource.Play();
        }
        opend = !opend;
        UpdateSprite();
    }

    public bool CanUseItemCheck()
    {
        return false;
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
