using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Complete;

public class DecelerationZoneHandler : ItemHandlerAbstractClass
{
    public float Decelerationpercent;

    private void OnTriggerEnter(Collider other)
    {
        if (LayerMask.LayerToName(other.gameObject.layer) == "TankToSpawn")
        {
            other.transform.parent.gameObject.GetComponent<TankMovement>().MultiplySpeed(Decelerationpercent);
        }
        else
        {
            return;
        }
    }
}
