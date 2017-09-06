using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Complete;

public class ShieldBox : BoxBase
{
    public float AddShieldAmount;

    protected override void OnPickupCollected(TankAndItsUIManager m, Collider tankCollider)
    {
        tankCollider.GetComponentInParent<TankHealth>().AddShield(AddShieldAmount, -2);
        m.OnPickupCollected(BoxId);
    }
}
