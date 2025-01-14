
using UnityEngine;

public interface IInteractable
{
    void UseItemEvent();
    void InteractEvent();
    bool CanUseItemCheck();
    bool CanInteractCheck();
    Transform Interactable();
}
