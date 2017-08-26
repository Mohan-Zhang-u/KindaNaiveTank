//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.Networking;

//[RequireComponent(typeof(Rigidbody))]
//[RequireComponent(typeof(Collider))]
////[RequireComponent(typeof(NetworkIdentity))]
////[RequireComponent(typeof(NetworkTransform))]
////This class acts as a base for all powerup pickups, defining all common behaviours.
//public abstract class BoxBase : NetworkBehaviour
//{
//    //The name of this pickup.
//    [SerializeField]
//    protected string PickUpId;

//    //The effect prefab to spawn when the pickup is collected.
//    [SerializeField]
//    protected GameObject m_CollectionEffect;

//    //Delay before the attached TankSeeker is set to active.
//    [SerializeField]
//    protected float m_AttractorActivationDelay = 1.2f;

//    //Internal cache for the layer of objects that are able to trigger pickup.
//    private int m_PickupLayer;

//    ////Number of ticks before the pickup collider is enabled.
//    //private int m_ColliderEnableCount = 2;


//    //Internal reference to the TankSeeker that attracts this pickup to player tanks.
//    //private TankSeeker m_Attractor;

//    protected virtual void Awake()
//    {
//        m_PickupLayer = LayerMask.NameToLayer("TankToSpawn");
//        //GetComponent<Collider>().enabled = false;
//    }

//    [ServerCallback]
//    protected virtual void Start()
//    {

//        //Add this powerup to the GameManager so that it can be destroyed between rounds if needed.
//        GameManager.s_Instance.AddPowerup(this);

//        //Autospawn this object to clients when init is complete.
//        NetworkServer.Spawn(gameObject);
//    }

//    //public override void OnNetworkDestroy()
//    //{
//    //    if (isServer)
//    //    {
//    //        //Remove this object's reference from the GameManager, since it's quite happily dead.
//    //        GameManager.s_Instance.RemovePowerup(this);
//    //    }

//    //    base.OnNetworkDestroy();
//    //}

//    [ServerCallback]
//    //protected virtual void Update()
//    //{
//    //    // IDK why this is useful
//    //    //Tick down the collider enable count, and enable the collection collider when depleted.
//    //    if (m_ColliderEnableCount >= 0)
//    //    {
//    //        m_ColliderEnableCount--;

//    //        if (m_ColliderEnableCount == 0)
//    //        {
//    //            GetComponent<Collider>().enabled = true;
//    //        }
//    //    }

//    //}

//    private void OnTriggerEnter(Collider other)
//    {
//        //We only want to register triggers fired by objects in the player layer.
//        if (other.gameObject.layer == m_PickupLayer)
//        {
//            //Create the collection effect. Immediate collection feedback on clients looks better.
//            if (m_CollectionEffect != null)
//            {
//                Instantiate(m_CollectionEffect, transform.position + Vector3.up, Quaternion.LookRotation(Vector3.up));
//            }

//            ////If this is the server, fire powerup collection logic and networkdestroy this object.
//            //if (isServer)
//            //{
//            //    OnPickupCollected(other.gameObject);
//            //    NetworkServer.Destroy(gameObject);
//            //}
//            OnPickupCollected(other.GetComponentInParent<TankAndItsUIManager>());
//            Destroy(gameObject);
//        }
//    }

//    //Damage is called by any player fire, and implements IDamageObject. It allows drop pods to be destroyed by players as a denial tactic.
//    //It also spawns a big nasty explosion that can take out any nearby tanks.
//    public void Damage(float damage)
//    {
//        if (damage < m_MinDamage || !isAlive)
//        {
//            return;
//        }

//        isAlive = false;
//        if (m_DeathExplosion != null && ExplosionManager.s_InstanceExists)
//        {
//            ExplosionManager.s_Instance.SpawnExplosion(transform.position, transform.up, gameObject, m_DestroyingPlayer, m_DeathExplosion, false);
//        }

//        NetworkServer.Destroy(gameObject);
//    }

//    public void SetDamagedBy(int playerNumber, string explosionId)
//    {
//        m_DestroyingPlayer = playerNumber;
//    }

//    public Vector3 GetPosition()
//    {
//        return transform.position;
//    }

//    //This method is overridden in child classes to implement specific powerup logic
//    //As a standard, though, it updates the triggering tank with info to update its player's HUD.
//    protected virtual void OnPickupCollected(TankAndItsUIManager m)
//    {
//        m.AddPickupId(PickUpId);
//    }
//}
