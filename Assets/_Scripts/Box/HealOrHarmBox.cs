using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Complete;

public class HealOrHarmBox : BoxBase
{

    public float MinAmount;
    public float MaxAmount;

    protected override void OnPickupCollected(TankAndItsUIManager m, Collider tankCollider)
    {
        float amount = UnityEngine.Random.Range(MinAmount, MaxAmount);
        Debug.Log("amount:" + amount);
        TankHealth tss = tankCollider.GetComponentInParent<TankHealth>();
        if (amount > 0)
        {
            tss.AddHealth(amount, -2);
        }
        else if (amount < 0)
        {
            tss.Damage(-amount, -2, "");
        }
        
        m.OnPickupCollected(BoxId);
    }
}
