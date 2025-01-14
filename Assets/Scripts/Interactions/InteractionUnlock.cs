using UnityEngine;

public class InteractionUnlock : MonoBehaviour, IInteractable
{

    public Door door;
    AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }
    public bool Break()
    {
        if (Inventory.i.CheckCurrentItem(GlobalInfo.i.key))
        {
            if (door.locked == true)
            {
                door.Unlock();
                audioSource.Play();
            }
        }


        return false;
    }
    public void UseItemEvent()
    {
        Break();
    }
    public void InteractEvent(){}

    public bool CanUseItemCheck()
    {
        return door.locked && Inventory.i.CheckCurrentItem(GlobalInfo.i.key);
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
