using Unity.VisualScripting;
using UnityEngine;

public class InteractionBridge : MonoBehaviour, IInteractable
{

    public GameObject bridge;
    public GameObject planks;
    public bool instant=true;

    AudioSource audioSource;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (bridge != null)
            bridge.SetActive(false);

        if (planks != null)
            planks.SetActive(true);

    }

    public void InteractEvent()
    {
        audioSource.Play();

        if (bridge != null)
            bridge.SetActive(true);

        if (planks != null)
            planks.SetActive(false);
    }

    public void UseItemEvent(){}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && instant)
        {
            InteractEvent();
        }
    }

    public bool CanUseItemCheck()
    {
        return false;
    }

    public bool CanInteractCheck()
    {
        return !instant && bridge.activeInHierarchy;
    }

    public Transform Interactable()
    {
        return transform;
    }
}