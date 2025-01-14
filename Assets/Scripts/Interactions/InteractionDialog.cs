using UnityEngine;

public class InteractionDialog : MonoBehaviour
{
    public Dialog dialog;

    public void InteractEvent()
    {
        StartCoroutine(GlobalUIInfo.i.dialogManager.ShowDialog(dialog));
    }
}
