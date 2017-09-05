using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Complete;

public class TrapHandler : ItemHandlerAbstractClass
{
    public float DamageAmount;
    public float TrapTime;
    public void OnTriggerEnter(Collider other)
    {
        if (LayerMask.LayerToName(other.gameObject.layer) == "TankToSpawn")
        {
            this.gameObject.GetComponent<Animation>().Play();
            other.transform.parent.gameObject.GetComponent<TankHealth>().Damage(DamageAmount, DeployByTankId, "");
            if (other.transform.parent.gameObject.GetComponent<TankMovement>().TryDisableMove(TrapTime))
            {
                Destroy(this.gameObject, TrapTime);
            }
        }
        else
        {
            return;
        }
    }
}
