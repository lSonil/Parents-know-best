using Unity.VisualScripting;
using UnityEngine;

public class InteractionVase : MonoBehaviour
{
    public Sprite broken;
    public Transform newPos;
    public GameObject dropPosition;

    public void Break()
    {
        dropPosition.transform.SetParent(transform.parent);
        transform.position = newPos.transform.position;
        GetComponent<SpriteRenderer>().sprite = broken;
    }
}
