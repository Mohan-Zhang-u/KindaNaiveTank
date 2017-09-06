using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoodRandomBox : BoxBase {

    public BoxesSettings bset;

    protected override void OnPickupCollected(TankAndItsUIManager m, Collider tankCollider)
    {
        int spawnId = UnityEngine.Random.Range(0, bset.IdOfGoodBoxTypes.Length);
        BoxLibrary bl = FindObjectOfType<BoxLibrary>();
        bool success;
        BoxTypeDefinition bd = bl.GetBoxDataForName(bset.IdOfGoodBoxTypes[spawnId], out success);
        if (bl && success)
        {
            Instantiate(bd.displayPrefab, gameObject.transform.position, gameObject.transform.rotation);
        }
        m.OnPickupCollected(BoxId);
    }
}
