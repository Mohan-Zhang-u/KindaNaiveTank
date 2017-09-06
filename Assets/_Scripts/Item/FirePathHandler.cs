using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Complete;

public class FirePathHandler : ItemHandlerAbstractClass
{
    public float HarmSpeed;

    private void OnParticleCollision(GameObject other)
    {
        if (other.layer == LayerMask.NameToLayer("Players"))
        {
            TankHealth tankHealthScript = other.GetComponentInParent<TankHealth>();
            if (tankHealthScript)
            {
                tankHealthScript.Damage(HarmSpeed * Time.deltaTime, DeployByTankId, "");
            }
        }
    }
}
