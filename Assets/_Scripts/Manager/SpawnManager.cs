using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


/// <summary>
/// Spawn manager - used to get an unoccupied spawn point
/// </summary>
public class SpawnManager : Singleton<SpawnManager>
{


	private List<SpawnPoint> _SpawnPointScripts = new List<SpawnPoint>();
    private RandomSpawnPoint _RandomSpawnPointScript;

    protected override void Awake()
	{
		base.Awake();
		LazyLoadSpawnPoints();
	}

	private void Start()
	{
		LazyLoadSpawnPoints();
	}

	/// <summary>
	/// Lazy load the spawn points - this assumes that all spawn points are children of the SpawnManager
	/// </summary>
	private void LazyLoadSpawnPoints()
	{
        if (_SpawnPointScripts == null || _SpawnPointScripts.Count == 0){
            // else, add all spawn points from child.
            SpawnPoint[] foundSpawnPoints = GetComponentsInChildren<SpawnPoint>();
            _SpawnPointScripts.AddRange(foundSpawnPoints);
        }
        if (_RandomSpawnPointScript == null)
        {
            _RandomSpawnPointScript = GetComponentInChildren<RandomSpawnPoint>();
        }
	}

	///// <summary>
	///// Gets index of a random empty spawn point
	///// </summary>
	///// <returns>The random empty spawn point index.</returns>
	//public int GetRandomEmptySpawnPointIndex()
	//{
	//	LazyLoadSpawnPoints();
	//	//Check for empty zones
	//	List<SpawnPoint> emptySpawnPoints = _SpawnPointScripts.Where(sp => sp.isEmptyZone).ToList();
		
	//	//If no zones are empty, which is impossible if the setup is correct, then return the first spawnpoint in the list
	//	if (emptySpawnPoints.Count == 0)
	//	{
	//		return 0;
	//	}
		
	//	//Get random empty spawn point
	//	SpawnPoint emptySpawnPoint = emptySpawnPoints[Random.Range(0, emptySpawnPoints.Count)];
		
	//	//Mark it as dirty
	//	emptySpawnPoint.SetDirty();
		
	//	//return the index of this spawn point
	//	return _SpawnPointScripts.IndexOf(emptySpawnPoint);
	//}

    //public bool GetARandomPositionToSpawn(ref Transform location)
    //{


    //}

	public bool IfSpawnPointExists(int i)
	{
		if (_SpawnPointScripts [i])
			return true;
		else
			return false;
	}
		
	public SpawnPoint GetSpawnPointByIndex(int i)
	{
		LazyLoadSpawnPoints();
		return _SpawnPointScripts[i];
	}

	//public Transform GetSpawnPointTransformByIndex(int i)
	//{
	//	return GetSpawnPointByIndex(i).spawnPointTransform;
	//}

	/// <summary>
	/// Cleans up the spawn points.
	/// </summary>
	public void CleanupSpawnPoints()
	{
		for (int i = 0; i < _SpawnPointScripts.Count(); i++)
		{
			_SpawnPointScripts[i].Cleanup();
		}
	}

    /* NOTE!!!!!! Both have collider, at least one have Istrigger and rigidBody.
    type must be in:
    public bool General;
    public bool Player;
    public bool Npc;
    public bool Box;
    public bool ExplosiveItem;
    public bool DestroyableItem;
    */
    public bool FoundARandomSpawnPoint(string type, ref Transform t)
    {
        if (type == "General")
        {
            foreach(SpawnPoint sp in _SpawnPointScripts)
            {
                if (sp.General && sp.IsEmptyZone)
                {
                    t = sp.transform;
                    return true;
                }
            }
        }
        if (type == "Player")
        {
            foreach (SpawnPoint sp in _SpawnPointScripts)
            {
                if ((sp.General || sp.Player) && sp.IsEmptyZone)
                {
                    t = sp.transform;
                    return true;
                }
            }
        }
        if (type == "Npc")
        {
            foreach (SpawnPoint sp in _SpawnPointScripts)
            {
                if ((sp.General || sp.Npc) && sp.IsEmptyZone)
                {
                    t = sp.transform;
                    return true;
                }
            }
        }
        if (type == "Box")
        {
            foreach (SpawnPoint sp in _SpawnPointScripts)
            {
                if ((sp.General || sp.Box) && sp.IsEmptyZone)
                {
                    t = sp.transform;
                    return true;
                }
            }
        }
        if (type == "ExplosiveItem")
        {
            foreach (SpawnPoint sp in _SpawnPointScripts)
            {
                if ((sp.General || sp.ExplosiveItem) && sp.IsEmptyZone)
                {
                    t = sp.transform;
                    return true;
                }
            }
        }
        if (type == "DestroyableItem")
        {
            foreach (SpawnPoint sp in _SpawnPointScripts)
            {
                if ((sp.General || sp.DestroyableItem) && sp.IsEmptyZone)
                {
                    t = sp.transform;
                    return true;
                }
            }
        }
        // if all above fails, we need RanodomSpawnPoint.
        if (_RandomSpawnPointScript.IsEmptyZone)
        {
            t = _RandomSpawnPointScript.transform;
            return true;
        }
        if (!_RandomSpawnPointScript.IsEmptyZone)
        {
            _RandomSpawnPointScript.MoveToNewPosition();
            if (!_RandomSpawnPointScript.IsEmptyZone)
            {
                return false;
            }
            else
            {
                t = _RandomSpawnPointScript.transform;
                return true;
            }
        }
        return false;
    }

}
 