using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Complete;

public class FlameThrowerHandler : ItemHandlerAbstractClass
{
    public float HarmSpeed;

    private TankShooting tsScript;

    private void OnEnable()
    {
        tsScript = gameObject.GetComponentInParent<TankShooting>();
        tsScript.EnableFire = false;
    }

    private void OnDisable()
    {
        tsScript.EnableFire = true;
    }

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
