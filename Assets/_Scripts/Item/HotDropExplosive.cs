using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HotDropExplosive : ItemHandlerAbstractClass
{
    // TODO: initialization shall be handled by external script, which will set its position, rotation, FireByTankId
    public bool CanDropBoxes = true;
    //The prefab to spawn to indicate an incoming drop, and a temporary reference variable so we can early-out it if necessary.
    public GameObject m_HotdropEffectPrefab;

    public GameObject BoxExplosionEffect;

    private void OnEnable()
    {
        
    }
}
