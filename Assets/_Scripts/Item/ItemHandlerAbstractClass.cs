using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Complete;

public abstract class ItemHandlerAbstractClass : MonoBehaviour
{
    public GameObject ItemPrefab;
    public string ItemName;
    // IDs.
    [HideInInspector]
    public int DeployByTankId = -2;
    public int MaxLifeTime;
    
    public LayerMask TankMask;

    //	public void Awake(){
    //		TankMask = LayerMask.NameToLayer ("TankToSpawn");
    //		WallMask= LayerMask.NameToLayer ("Wall");
    //		ItemMask= LayerMask.NameToLayer ("Item");
    //		GeneralItemsMask= LayerMask.NameToLayer ("GeneralItem");
    //		ExplosiveItemsMask= LayerMask.NameToLayer ("ExplosiveItem");
    //		GroundMask = LayerMask.NameToLayer ("Ground");
    //	}

    // This function dont need to be overwrite
    public void Start()
    {
        // If it isn't destroyed by then, destroy the item after it's lifetime.
        Destroy(gameObject, MaxLifeTime);
    }

    ////  ezly copy from ProjectileItemHandler.cs
    //#region CopyAndPaste 
    /////-----------------------------------------------NOW, START COPY AND PASTE!!!!!!-------------------------------------------------------------
    ////its shared all accross!!!!!!!!!!!!!!!!!!!! 
    ///// <summary>
    ///// it is the ugliest part in C#. because, since this is a parent abstract class,
    ///// if we dont copy and paste this code to its parent, the ExplosionParticles and ExplosionAudio
    ///// will be THIS ABSTRACT CLASS' ExplosionParticles and ExplosionAudio.
    ///// The only way we can use ExplosionParticles and ExplosionAudio in parent class is that 
    ///// we copy and paste this function there.
    ///// </summary>
    ///// <param name="c">C.</param>
    //// the main handler, AND THE DEFUALT IS, explode whenever hits a thing.!!!! can OPTIMIZE PERFORMANCE here. 
    //virtual public void OnTriggerEnter(Collider other)
    //{
    //    Explode(other);
    //}

    //// TODO: when it is Client side, we shall only apply the force. WHEN IT IS SERVER SIDE, moreover, we shall deal the damage.
    ///// <summary>
    ///// it is the ugliest part in C#. because, since this is a parent abstract class,
    ///// if we dont copy and paste this code to its parent, the ExplosionParticles and ExplosionAudio
    ///// will be THIS ABSTRACT CLASS' ExplosionParticles and ExplosionAudio.
    ///// The only way we can use ExplosionParticles and ExplosionAudio in parent class is that 
    ///// we copy and paste this function there.
    ///// </summary>
    ///// <param name="c">C.</param>
    //virtual public void Explode(Collider other)
    //{
    //    // Collect all the colliders in a sphere from the item's current position to a radius of the explosion radius.
    //    Collider[] TankColliders = Physics.OverlapSphere(transform.position, ExplosionRadius, TankMask);
    //    Collider[] WallColliders = Physics.OverlapSphere(transform.position, ExplosionRadius, WallMask);
    //    Collider[] ItemColliders = Physics.OverlapSphere(transform.position, ExplosionRadius, ItemMask);
    //    Collider[] GeneralItemsColliders = Physics.OverlapSphere(transform.position, ExplosionRadius, GeneralItemsMask);
    //    Collider[] ExplosiveItemColliders = Physics.OverlapSphere(transform.position, ExplosionRadius, ExplosiveItemsMask);
    //    Collider[] GroundColliders = Physics.OverlapSphere(transform.position, ExplosionRadius, GroundMask);

    //    // Go through all the colliders...
    //    for (int i = 0; i < TankColliders.Length; i++)
    //    {
    //        CollideWithTanks(TankColliders[i]);
    //    }
    //    for (int i = 0; i < WallColliders.Length; i++)
    //    {
    //        CollideWithWalls(WallColliders[i]);
    //    }
    //    for (int i = 0; i < ItemColliders.Length; i++)
    //    {
    //        CollideWithItems(ItemColliders[i]);
    //    }
    //    for (int i = 0; i < GeneralItemsColliders.Length; i++)
    //    {
    //        CollideWithGeneralItem(GeneralItemsColliders[i]);
    //    }
    //    for (int i = 0; i < ExplosiveItemColliders.Length; i++)
    //    {
    //        CollideWithExplosiveItems(GeneralItemsColliders[i]);
    //    }
    //    for (int i = 0; i < GroundColliders.Length; i++)
    //    {
    //        CollideWithGround(GroundColliders[i]);
    //    }

    //    // now, finialize explosion. perform Particles.
    //    // prepare the explosion system
    //    ExplosionParticles = Instantiate(ItemExplosion).GetComponent<ParticleSystem>();
    //    if (ExplosionAudio == null)
    //        ExplosionAudio = ExplosionParticles.GetComponent<AudioSource>();
    //    ExplosionParticles.transform.position = transform.position;
    //    ExplosionParticles.gameObject.SetActive(true);
    //    if (ExplosionParticles)
    //    {
    //        // Unparent the particles from the item.
    //        ExplosionParticles.transform.parent = null;

    //        // Play the particle system.
    //        ExplosionParticles.Play();
    //        // Play the explosion sound effect.
    //        if (ExplosionAudio)
    //            ExplosionAudio.Play();

    //        // Once the particles have finished, destroy the gameobject they are on.
    //        ParticleSystem.MainModule mainModule = ExplosionParticles.main;
    //        Destroy(ExplosionParticles.gameObject, mainModule.duration);
    //    }

    //    Destroy(gameObject);
    //}

    //// its shared all accross!!!!!!!!!!!!!!!!!!!!
    ///// <summary>
    ///// it is the ugliest part in C#. because, since this is a parent abstract class,
    ///// if we dont copy and paste this code to its parent, the ExplosionParticles and ExplosionAudio
    ///// will be THIS ABSTRACT CLASS' ExplosionParticles and ExplosionAudio.
    ///// The only way we can use ExplosionParticles and ExplosionAudio in parent class is that 
    ///// we copy and paste this function there.
    ///// </summary>
    ///// <param name="c">C.</param>
    //virtual public void CollideWithTanks(Collider c)
    //{
    //    Rigidbody targetRigidbody = c.GetComponentInParent<Rigidbody>();
    //    if (!targetRigidbody)
    //        return;

    //    // Add an explosion force.
    //    targetRigidbody.AddExplosionForce(ExplosionForce, transform.position, ExplosionRadius);

    //    // Find the TankHealth script associated with the rigidbody.
    //    TankHealth targetHealth = targetRigidbody.GetComponentInParent<TankHealth>();

    //    // If there is no TankHealth script attached to the gameobject, go on to the next collider.
    //    if (!targetHealth)
    //        return;

    //    // Calculate the amount of damage the target should take based on it's distance from the item.
    //    float damage = CalculateDamage(targetRigidbody.position);

    //    // Deal this damage to the tank.
    //    targetHealth.Damage(damage, FireByTankId, ExplosionId);
    //}

    ////TODO: im not pretty sure whether its is CORRECT!!!!!!!!!!!!!!!!!!!!
    //virtual public void CollideWithItems(Collider c)
    //{
    //    if (c.isTrigger)
    //    {
    //        c.GetComponent<ItemHandlerAbstractClass>().Explode(ItemPrefab.GetComponent<Collider>());
    //    }
    //}

    //// targetPosition is tank's position. the function calculates according to tank's position and the explosive(item's) position
    //virtual public float CalculateDamage(Vector3 targetPosition)
    //{
    //    // Create a vector from the item to the target.
    //    Vector3 explosionToTarget = targetPosition - transform.position;

    //    // Calculate the distance from the item to the target.
    //    float explosionDistance = explosionToTarget.magnitude;

    //    // Calculate the proportion of the maximum distance (the explosionRadius) the target is away.
    //    float relativeDistance = (ExplosionRadius - explosionDistance) / ExplosionRadius;

    //    // Calculate damage as this proportion of the maximum possible damage.
    //    float damage = relativeDistance * MaxDamage;

    //    // Make sure that the minimum damage is always 0.
    //    damage = Mathf.Max(0f, damage);

    //    return damage;
    //}

    //public virtual void CollideWithWalls(Collider c)
    //{

    //}



    //public virtual void CollideWithGeneralItem(Collider c)
    //{

    //}

    ////TODO: We Assumes that It Have Explode.
    //public virtual void CollideWithExplosiveItems(Collider c)
    //{
    //    c.GetComponent<ItemHandlerAbstractClass>().Explode(ItemPrefab.GetComponent<Collider>());
    //}

    //public virtual void CollideWithGround(Collider c)
    //{

    //}

    /////-----------------------------------------------NOW, END COPY AND PASTE!!!!!!-------------------------------------------------------------
    //#endregion

}
