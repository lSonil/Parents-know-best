using TMPro;
using UnityEngine;

public class InteractionNote : MonoBehaviour, IInteractable
{
    public TMP_Text text;
    AudioSource audioSource;
    public bool onTrigger =true;

    private void Start()
    {
        audioSource= GetComponent<AudioSource>();

        if (onTrigger)
            audioSource.Play();
    }

    public bool CanInteractCheck()
    {
        return true;
    }

    public bool CanUseItemCheck()
    {
        return false;
    }

    public void InteractEvent()
    {
        GlobalUIInfo.i.ShowNote(text);
    }

    public void UseItemEvent(){}
    public Transform Interactable()
    {
        return transform;
    }
}
