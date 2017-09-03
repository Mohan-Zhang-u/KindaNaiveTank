using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Complete;

public class SwordHandler : ShellHandlerAbstractClass
{

    override public void OnTriggerEnter(Collider c)
    {
        // if not tank, return.
        if (c.gameObject.layer != TankMask)
            return;
        Rigidbody targetRigidbody = c.GetComponentInParent<Rigidbody>();
        if (!targetRigidbody)
            return;
        // Find the TankHealth script associated with the rigidbody.
        TankHealth targetHealth = targetRigidbody.GetComponentInParent<TankHealth>();
        TankShooting targetShooting = targetRigidbody.GetComponentInParent<TankShooting>();
        // If there is no TankHealth script attached to the gameobject, go on to the next collider.
        if (!targetHealth || !targetShooting)
            return;
        //check if it is the fired tank itself.
        if (targetShooting._PlayerNumber == FireByTankId)
            return;
        targetHealth.Damage(MaxDamage, FireByTankId, ExplosionId);
    }
}
