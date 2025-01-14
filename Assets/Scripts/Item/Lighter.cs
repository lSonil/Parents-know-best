using System.Collections;
using UnityEngine;

public class Lighter : MonoBehaviour
{
    public bool on =false;
    public GameObject lgh;
    public Sprite lightOn;
    public Sprite lightOff;
    private void Start()
    {
        GetComponentInParent<Item>().function = transform.name=="Candle"? TurnLight:Light;
        ForceOff();
    }

    private void FixedUpdate()
    {
        if(Inventory.i.FindItemIndex(GetComponentInParent<Item>()).itemFound && !Inventory.i.CheckCurrentItem(GetComponentInParent<Item>()))
        {
            ForceOff();
        }
    }
    public void Light()
    {
        on = !on;
        lgh.SetActive(on);
        UpdateSprite();
    }
    public void ForceOff()
    {
        on = false;
        lgh.SetActive(false);
        UpdateSprite();
    }

    public void TurnLight()
    {
        var results = Inventory.i.FindItemIndex(GlobalInfo.i.lighter);
        if (true == results.itemFound)
        {
            Light();
        }
    }

    public void UpdateSprite()
    {
        GetComponent<SpriteRenderer>().sprite = on? lightOn : lightOff;
    }
}
