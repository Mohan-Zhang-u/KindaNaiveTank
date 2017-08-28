using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Complete;

public class BoxSpawnManager : MonoBehaviour
{

    private BoxLibrary BoxLibraryScript;

    // below come from BoxLibraryScript ----------------------------------------------------------
    private ExplosionSettings m_SpawnExplosion;

    //The prefab to spawn to indicate an incoming drop, and a temporary reference variable so we can early-out it if necessary.
    private GameObject m_HotdropEffectPrefab;
    private GameObject BoxExplosionEffect;

    //private BoxTypeDefinition[] _BoxesToSpawn;
    // above come from BoxLibraryScript ----------------------------------------------------------


    // TODO:this is something we can handle in Gamemanager.---------------------------------------
    public float m_DropRadius = 10f;

    //The interval between powerup drops.
    [SerializeField]
    protected float m_DropInterval = 30f;

    //The radius of the spherecast to determine whether a candidate drop area is clear or not.
    [SerializeField]
    protected float m_SpherecastRadius = 3f;

    //Flag to set whether this spawner is active or not.
    [SerializeField]
    protected bool m_IsSpawnerActive = false;
    // something we can handle in Gamemanager.---------------------------------------

    //The next time when a drop will occur.
    private float m_NextDropTime;

    //The next time when a drop pod will spawn. This is used to offset the time of actual instantiation of the drop pod from the time of the incoming drop effect.
    private float m_NextSpawnTime = 0f;

    //The point of the next drop.
    private Vector3 m_DropTargetPosition;

    private GameObject m_ActiveDropEffect;

    ////The total of all drop weightings for random selection purposes.
    //private int m_TotalWeighting;

    private LayerLibrary LayerLibraryScript;

    private void OnEnable()
    {
        BoxLibraryScript = FindObjectOfType<BoxLibrary>();

        m_SpawnExplosion = BoxLibraryScript.m_SpawnExplosion;
        m_HotdropEffectPrefab = BoxLibraryScript.m_HotdropEffectPrefab;
        BoxExplosionEffect = BoxLibraryScript.BoxExplosionEffect;
        m_NextDropTime = Time.time+ m_DropInterval;

        LayerLibraryScript = FindObjectOfType<LayerLibrary>();

        ////Aggregate all the drop weightings of the items in our powerup list for selection.
        //for (int i = 0; i < _BoxesToSpawn.Length; i++)
        //{
        //    m_TotalWeighting += _BoxesToSpawn[i].dropWeighting;
        //}
    }

    // TODO: [ServerCallback]
    private void Update()
    {
        if (m_IsSpawnerActive)
        {
            //This tracks the next time to spawn the drop effect.
            if (Time.time >= m_NextDropTime)
            {
                TryToBeginDrop();
                m_NextDropTime = Time.time + m_DropInterval;
            }

            //The tracks the next time to spawn a powerup.
            if ((m_NextSpawnTime > 0f) && (Time.time >= m_NextSpawnTime))
            {
                SpawnPowerup();
                m_NextSpawnTime = 0f;
            }
        }
    }

    //This method scans the battlefield for empty ground, and having found a candidate target zone, instantiates the drop pod effect and queues the spawning of the powerup object.
    private void TryToBeginDrop()
    {
        bool hasTarget = false;

        RaycastHit hitdata;

        int c = 0;
        while (!hasTarget && c < 20)
        {
            c += 1;
            Vector3 randomRadius = Vector3.right * Random.Range(0f, m_DropRadius);
            Quaternion randomRotation = Quaternion.AngleAxis(Random.Range(0f, 360f), Vector3.up);

            m_DropTargetPosition = randomRotation * randomRadius;

            Physics.SphereCast(m_DropTargetPosition + (Vector3.up * 500f), m_SpherecastRadius, Vector3.down, out hitdata, 600f);

            if ((hitdata.collider.gameObject.layer == LayerLibraryScript.GroundMask) || (hitdata.collider.gameObject.layer == LayerLibraryScript.TankMask)
                || (hitdata.collider.gameObject.layer == LayerLibraryScript.PlayersMask))
            {
                hasTarget = true;
            }
        }

        // the map is so crowded.
        if (c >= 20)
        {
            Debug.Log("<color=red> Cannot find a box drop Location!</color>");
            return;
        }


        //Instantiate the drop effect.
        GameObject dropEffect = (GameObject)Instantiate(m_HotdropEffectPrefab, m_DropTargetPosition, Quaternion.identity);
        m_ActiveDropEffect = dropEffect;
        //Set the powerup spawn timer for when the effect is done. assumes dropEffect.GetComponent<HotdropLight>().dropTime = 2.73333
        m_NextSpawnTime = Time.time + 2.73333f;
        // destory the animation after 5 seconds.
        Destroy(dropEffect, 5f);
    }

    //Gets a random powerup and spawns it to the field along with an explosion to correlate with its "drop" from orbit.
    private void SpawnPowerup()
    {
        m_ActiveDropEffect = null;

        GameObject cratePrefab = FindObjectOfType<GameManagerBase>().GetRandomBox().displayPrefab;
        Debug.Log("cratePrefab:" + cratePrefab.ToString());
        //Crates will auto-network-spawn on start, so we only need to instantiate them.
        // !!!!!!!!!!!!!we hereby add 1.5f inorder to let it be above the ground!!!!!!!!!!!
        GameObject dropPod = (GameObject)Instantiate(cratePrefab, m_DropTargetPosition+new Vector3(0,1.5f,0), Quaternion.identity);


        if (m_SpawnExplosion != null)
        {
            SpawnExplosion(dropPod.transform.position, transform.up, dropPod, -1, m_SpawnExplosion, false);
        }
    }


    #region explosioin settings
    /// <summary>
    /// Create an explosion at a given location
    /// </summary>
    private void SpawnExplosion(Vector3 explosionPosition, Vector3 explosionNormal, GameObject ignoreObject, int damageOwnerId, ExplosionSettings explosionConfig, bool clientOnly)
    {
        // TODO: do implement client and server
        //if (clientOnly)
        //{
        //    CreateVisualExplosion(explosionPosition, explosionNormal, explosionConfig.explosionClass);
        //}
        //else if (isServer)
        //{
        //    RpcVisualExplosion(explosionPosition, explosionNormal, explosionConfig.explosionClass);
        //}
        GameObject ExplosioinEffect = Instantiate<GameObject>(BoxExplosionEffect, explosionPosition, Quaternion.identity);
        ExplosioinEffect.transform.up = explosionNormal;
        // will this ExplosioinEffect call its freaking Awake?

        DoLogicalExplosion(explosionPosition, explosionNormal, ignoreObject, damageOwnerId, explosionConfig);
    }

    /// <summary>
    /// Perform logical explosion
    /// On server, this deals damage to stuff. On clients, it just applies forces
    /// </summary>
    private void DoLogicalExplosion(Vector3 explosionPosition, Vector3 explosionNormal, GameObject ignoreObject, int damageOwnerId, ExplosionSettings explosionConfig)
    {
        Collider[] colliders = Physics.OverlapSphere(explosionPosition, Mathf.Max(explosionConfig.explosionRadius, explosionConfig.physicsRadius), LayerMask.GetMask("TankToSpawn"));
        // Go through all the colliders...
        for (int i = 0; i < colliders.Length; i++)
        {
            Collider struckCollider = colliders[i];
            

            // Create a vector from the shell to the target.
            Vector3 explosionToTarget = struckCollider.transform.position - explosionPosition;

            // Calculate the distance from the shell to the target.
            float explosionDistance = explosionToTarget.magnitude;

            //// TODO:Server deals damage to objects
            //if (isServer)
            //{
            // Find the DamageObject script associated with the rigidbody.
                TankHealth targetHealth = struckCollider.GetComponentInParent<TankHealth>();

                // If there is one, deal it damage
                if (targetHealth != null &&
                    targetHealth.isAlive &&
                    explosionDistance < explosionConfig.explosionRadius)
                {
                    // Calculate the proportion of the maximum distance (the explosionRadius) the target is away.
                    float normalizedDistance = Mathf.Clamp01((explosionConfig.explosionRadius - explosionDistance) / explosionConfig.explosionRadius);

                    // Calculate damage as this proportion of the maximum possible damage.
                    float damage = normalizedDistance * explosionConfig.damage;

                    // Deal this damage to the tank.
                    targetHealth.Damage(damage, damageOwnerId, explosionConfig.id);
            }
            //}
            Rigidbody targetRigidBody = struckCollider.GetComponentInParent<Rigidbody>();
            if (targetRigidBody!=null && explosionDistance < explosionConfig.physicsRadius)
            {
                targetRigidBody.AddExplosionForce(explosionConfig.physicsForce, explosionPosition, explosionConfig.physicsRadius);
            }

            //public void AddExplosionForce(float explosionForce, Vector3 explosionPosition, float explosionRadius, float upwardsModifier = 0.0F, ForceMode mode = ForceMode.Force);
            //// Apply force onto PhysicsAffected objects, for anything we have authority on, or anything that's client only
            //PhysicsAffected physicsObject = struckCollider.GetComponentInParent<PhysicsAffected>();
            //NetworkIdentity identity = struckCollider.GetComponentInParent<NetworkIdentity>();

            //if (physicsObject != null && physicsObject.enabled && explosionDistance < explosionConfig.physicsRadius &&
            //    (identity == null || identity.hasAuthority))
            //{
            //    physicsObject.ApplyForce(explosionConfig.physicsForce, explosionPosition, explosionConfig.physicsRadius);
            //}
        }
    }

    #endregion

    // TODO: use this in game manager. Activates the spawner and sets its next drop time. Normally called from the GameManager at the beginning of a new round.
    public void ActivateSpawner()
    {
        m_IsSpawnerActive = true;
        m_NextDropTime = Time.time + m_DropInterval;
    }

    // TODO: use this in game manager. Deactivates the spawner, and stops any drop sequence that is still active in its tracks.
    public void DeactivateSpawner()
    {
        m_IsSpawnerActive = false;

        //// TODO: network behaviour
        //if (m_ActiveDropEffect != null)
        //{
        //    NetworkServer.Destroy(m_ActiveDropEffect);
        //}
        if (m_ActiveDropEffect != null)
        {
            Destroy(m_ActiveDropEffect);
        }

        m_NextSpawnTime = 0f;
    }


}
