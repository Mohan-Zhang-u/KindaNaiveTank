using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Complete;

public class AccelerateBox : BoxBase
{
    public float AccelerateAmount;

    protected override void OnPickupCollected(TankAndItsUIManager m, Collider tankCollider)
    {
        tankCollider.GetComponentInParent<TankMovement>().MultiplySpeed(AccelerateAmount);
        m.OnPickupCollected(BoxId);
        Debug.Log("collected healthBox");
    }
}
