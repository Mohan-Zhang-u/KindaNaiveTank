using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Complete;

public class FlameThrowerHandler : ItemHandlerAbstractClass
{
    public float HarmSpeed;

    private void OnParticleCollision(GameObject other)
    {
        if (other.layer == LayerMask.NameToLayer("Players"))
        {
            Debug.Log("collided");
            TankHealth tankHealthScript = other.GetComponentInParent<TankHealth>();
            if (tankHealthScript)
            {
                tankHealthScript.Damage(HarmSpeed * Time.deltaTime, DeployByTankId, "");
            }
        }
    }
}
