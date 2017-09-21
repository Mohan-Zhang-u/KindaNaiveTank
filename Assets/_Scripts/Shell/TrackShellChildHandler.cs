using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Complete;

public class TrackShellChildHandler : MonoBehaviour {

    public TrackShellHandler TSS;
    public LayerMask TargetMask;

    // TODO: set it to the first tank meet? or dont care?
    // TODO: set shoot from
    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log(LayerMask.LayerToName(other.gameObject.layer)+","+ MyLibrary.LayerInLayerMask(other.gameObject.layer, TargetMask));
        if (MyLibrary.LayerInLayerMask(other.gameObject.layer, TargetMask))
        {
            if (other.gameObject.GetComponentInParent<TankShooting>()._PlayerNumber != TSS.FireByTankId)
            {
                
                TSS.Target = other.gameObject;
                Destroy(gameObject);
            }
        }
    }
}
