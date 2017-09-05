using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Complete;

public class ItemBoxBase : BoxBase
{

    public string ItemName;
    protected override void OnPickupCollected(TankAndItsUIManager m, Collider tankCollider)
    {
        m.OnPickUpItems(ItemName);
        m.OnPickupCollected(BoxId);

    }
}
