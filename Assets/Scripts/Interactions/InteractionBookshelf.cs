using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;

public class InteractionBookshelf : MonoBehaviour, IInteractable
{
    public SpriteRenderer bookshelf;
    public int phase;
    public Sprite[] shelf;
    AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();

        foreach (Transform child in transform)
        {
            SetItem(child, false);
        }
    }
    public void SetItem(Transform child, bool value)
    {
        child.gameObject.SetActive(value);
        if (child.GetComponent<MatrixBlock>() != null)
            child.GetComponent<MatrixBlock>().Block(value);
    }
    private void Update()
    {
        if (GlobalInfo.i.GetRoom("Livingroom") != PlayerHandler.i.room && phase != 0)
        {
            foreach (Transform child in transform)
            {
                SetItem(child, false);
            }
            phase = 0;
            bookshelf.sprite = shelf[phase];
        }
    }
    public void InteractEvent()
    {
        if (phase >= shelf.Length-1)
            return;

        audioSource.Play();
        phase++;
        bookshelf.sprite = shelf[phase];

        foreach (Transform child in transform)
        {
            if (!child.gameObject.activeInHierarchy)
            {
                SetItem(child, true);
                break;
            }
        }
    }
    public void UseItemEvent() { }

    public bool CanUseItemCheck()
    {
        return false;
    }

    public bool CanInteractCheck()
    {
        return phase >= shelf.Length-1 ? false : true;
    }

    public Transform Interactable()
    {
        return transform;
    }
}