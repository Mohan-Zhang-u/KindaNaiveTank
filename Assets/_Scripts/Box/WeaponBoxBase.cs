using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Complete;

public class WeaponBoxBase : BoxBase
{
    public string ShellName;
    protected override void OnPickupCollected(TankAndItsUIManager m, Collider tankCollider)
    {
        tankCollider.GetComponentInParent<TankShooting>().OnChangeShellByName(ShellName);
        m.OnPickupCollected(BoxId);
    }
}
