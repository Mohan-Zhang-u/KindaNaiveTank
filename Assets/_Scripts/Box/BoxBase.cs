using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Complete;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
// TODO: add
//[RequireComponent(typeof(NetworkIdentity))]
//[RequireComponent(typeof(NetworkTransform))]
//This class acts as a base for all powerup pickups, defining all common behaviours.
public abstract class BoxBase : MonoBehaviour
{
    //The name of this pickup.
    [SerializeField]
    protected string BoxId;

    //The effect prefab to spawn when the pickup is collected.
    [SerializeField]
    protected GameObject m_CollectionEffect;

    //Internal cache for the layer of objects that are able to trigger pickup. this is tank by default.
    private int TankLayerMask;

    private GameManagerBase GameManagerScript; // this can be eitehr SinglePlayerGamemanager or MultiplayerGamemanager.

    protected virtual void Awake()
    {
        TankLayerMask = LayerMask.NameToLayer("TankToSpawn");
        GameManagerScript = FindObjectOfType<GameManagerBase>();
    }

    //TODO: implement this networkBehaviour [ServerCallback]
    protected virtual void OnEnable()
    {

        //Add this powerup to the GameManager so that it can be destroyed between rounds if needed.
        if (GameManagerScript != null)
        {
            GameManagerScript.AddPowerup(this);
        }
        else
        {
            Debug.Log("<color=red>Cannot find a gamemanager to add this box! </color>");
        }
       
        //Autospawn this object to clients when init is complete.
        //TODO: implement this networkBehaviour NetworkServer.Spawn(gameObject);
    }

    #region Network Utility
    //TODO: implement this networkBehaviour public override void OnNetworkDestroy()
    //{
    //    if (isServer)
    //    {
    //        //Remove this object's reference from the GameManager, since it's quite happily dead.
    //        GameManager.s_Instance.RemovePowerup(this);
    //    }

    //    base.OnNetworkDestroy();
    //}

    //TODO: implement this networkBehaviour  [ServerCallback]
    //protected virtual void Update()
    //{
    //    // IDK why this is useful
    //    //Tick down the collider enable count, and enable the collection collider when depleted.
    //    if (m_ColliderEnableCount >= 0)
    //    {
    //        m_ColliderEnableCount--;

    //        if (m_ColliderEnableCount == 0)
    //        {
    //            GetComponent<Collider>().enabled = true;
    //        }
    //    }

    //}
#endregion

    private void OnTriggerEnter(Collider other)
    {
        //We only want to register triggers fired by objects in the player layer.
        if (other.gameObject.layer == TankLayerMask)
        {
            //Create the collection effect. Immediate collection feedback on clients looks better.
            if (m_CollectionEffect != null)
            {
                Instantiate(m_CollectionEffect, transform.position + Vector3.up, Quaternion.LookRotation(Vector3.up));
            }

            // TODO: implement this networkBehaviour If this is the server, fire powerup collection logic and networkdestroy this object.
            //if (isServer)
            //{
            //    OnPickupCollected(other.gameObject);
            //    NetworkServer.Destroy(gameObject);
            //}

            OnPickupCollected(other.GetComponentInParent<TankAndItsUIManager>(), other);
            Destroy(gameObject);
        }
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }

    //This method is overridden in child classes to implement specific powerup logic
    //As a standard, though, it updates the triggering tank with info to update its player's HUD.
    protected virtual void OnPickupCollected(TankAndItsUIManager m, Collider tankCollider)
    {
        m.OnPickupCollected(BoxId);
        Debug.Log("executing parent!");
    }

}
