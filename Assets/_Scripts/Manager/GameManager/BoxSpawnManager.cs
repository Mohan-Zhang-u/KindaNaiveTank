using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class BoxSpawnManager : MonoBehaviour
{

    private BoxLibrary BoxLibraryScript;

    // below come from BoxLibraryScript ----------------------------------------------------------
    private ExplosionSettings m_SpawnExplosion;

    //The prefab to spawn to indicate an incoming drop, and a temporary reference variable so we can early-out it if necessary.
    private GameObject m_HotdropEffectPrefab;

    private BoxTypeDefinition[] _BoxesToSpawn;
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
    private bool m_IsSpawnerActive = false;
    // something we can handle in Gamemanager.---------------------------------------

    //The next time when a drop will occur.
    private float m_NextDropTime;

    //The next time when a drop pod will spawn. This is used to offset the time of actual instantiation of the drop pod from the time of the incoming drop effect.
    private float m_NextSpawnTime = 0f;

    //The point of the next drop.
    private Vector3 m_DropTargetPosition;

    private GameObject m_ActiveDropEffect;

    //The total of all drop weightings for random selection purposes.
    private int m_TotalWeighting;

    private LayerLibrary LayerLibraryScript;

    private void OnEnable()
    {
        BoxLibraryScript = FindObjectOfType<BoxLibrary>();

        m_SpawnExplosion = BoxLibraryScript.m_SpawnExplosion;
        m_HotdropEffectPrefab = BoxLibraryScript.m_HotdropEffectPrefab;
        m_NextDropTime = m_DropInterval;

        LayerLibraryScript = FindObjectOfType<LayerLibrary>();

        //Aggregate all the drop weightings of the items in our powerup list for selection.
        for (int i = 0; i < _BoxesToSpawn.Length; i++)
        {
            m_TotalWeighting += _BoxesToSpawn[i].dropWeighting;
        }

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
        //Set the powerup spawn timer for when the effect is done.
        m_NextSpawnTime = Time.time + dropEffect.GetComponent<HotdropLight>().dropTime;
    }

    private void SpawnPowerup()
    {
        
    }
}
