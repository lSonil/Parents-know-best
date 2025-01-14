using UnityEngine;


public class InteractionNodeAndDrop : MonoBehaviour, IInteractable
{
    public bool blocked;
    public SpriteRenderer sr;

    public Sprite open;
    public Sprite closed;
    public GameObject page;

    public bool opend = false;
    public bool interacted = false;

    AudioSource audioSource;

    private void Start()
    {
        audioSource=GetComponent<AudioSource>();
        if (page != null)
            page.SetActive(false);
    }
    public void UseItemEvent(){}

    public void Block(bool state)
    {
        blocked = state;
    }
    public void InteractEvent()
    {
        if (blocked)
            return;

        if (page != null)
            page.SetActive(true);
        else
        {
            audioSource.Play();
        }
        opend = !opend;
        interacted = true;
        UpdateSprite();
    }
    public void ForceEvent(bool value)
    {
        opend = value;
        UpdateSprite();
    }
    public void UpdateSprite()
    {
        sr.sprite = opend ? open : closed;
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
