using Unity.VisualScripting;
using UnityEngine;

public class InteractionCage : MonoBehaviour, IInteractable
{
    public bool opend = false;
    public SpriteRenderer body;
    public Sprite open;
    public Sprite closed;

    AudioSource audioSource;

    public void InteractEvent()
    {
        opend = !opend;

        audioSource.Play();

        SetCell(!opend);
        UpdateSprite();

        if (GlobalInfo.i.cat.currentCell == GetComponent<MatrixBlock>().blockCell[0])
            GlobalInfo.i.cat.SetCaged(!opend);
    }

    public void UseItemEvent()
    {
        throw new System.NotImplementedException();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        SetCell(!opend);
        UpdateSprite();
    }
    public void SetCell(bool value)
    {
        GetComponent<MatrixBlock>().Block(value);
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
        return true;
    }

    public Transform Interactable()
    {
        return transform;
    }
}
