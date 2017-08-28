using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Complete;

public class healthBox : BoxBase
{
    public float HealAmount;

    protected override void OnPickupCollected(TankAndItsUIManager m, Collider tankCollider)
    {
        tankCollider.GetComponentInParent<TankHealth>().AddHealth(HealAmount, -2);
        m.OnPickupCollected(BoxId);
        Debug.Log("collected healthBox");
    }
}
