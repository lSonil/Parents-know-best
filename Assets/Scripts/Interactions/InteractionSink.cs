using System.Collections;
using UnityEngine;


public class InteractionSink : MonoBehaviour, IInteractable
{
    public SpriteRenderer sink;
    public SpriteRenderer leak;

    public Sprite closed;
    public Sprite open;
    public Sprite openCut;
    public Sprite openRepaired;

    public Sprite closedRunning;
    public Sprite openRunning;
    public Sprite openRepairedRunning;

    public bool cut = false;
    public bool opend = false;
    public bool repaired = false;
    public bool running = false;
    public bool blocked = false;

    public Sprite[] leakPhase;
    AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();

        leak.enabled = false;
    }
    public void Block(bool state)
    {
        blocked = state;
    }

    public void UseItemEvent()
    {
        if (blocked)
            return;

        if (Inventory.i.CheckCurrentItem(GlobalInfo.i.scissors) && !cut && opend)
        {
            cut = true;
            StartCoroutine(Leak());
            UpdateSprite();
        }
    }

    public void Repair()
    {
        repaired = true;
        UpdateSprite();
    }

    public void UseSink(bool isOn)
    {
        running = isOn;
        UpdateSprite();
    }

    public void UpdateSprite()
    {
        if (cut && !repaired)
            running = false;

        if (opend)
        {
            if (running)
            {
                sink.sprite = repaired ? openRepairedRunning : openRunning;
            }
            else
            {
                sink.sprite = repaired ? openRepaired : cut ? openCut : open;
            }
        }
        else
        {
            sink.sprite = running ? closedRunning : closed;
        }
    }
    IEnumerator Leak()
    {
        leak.enabled = true;
        int phase = 0;
        while (!repaired)
        {
            yield return new WaitForSeconds(0.2f);
            leak.sprite = leakPhase[phase];
            phase++;
            if (phase == leakPhase.Length)
                phase -= 5;
        }

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
        return Inventory.i.CheckCurrentItem(GlobalInfo.i.scissors) && !cut && opend && !blocked;
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