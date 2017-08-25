using UnityEngine;
using System.Collections;
using Complete;

/// <summary>
/// Spawn point - has a collider to check if any player is in the zone
/// ATTENTION! Dont Collide With Ground!
/// </summary>
[RequireComponent(typeof(Collider))]
public class SpawnPoint : MonoBehaviour
{
    public bool General;
    public bool Player;
    public bool Npc;
    public bool Box;
    public bool ExplosiveItem;
    public bool DestroyableItem;

	// TODO: idk exactlly why this function is in need.
	public Transform SpawnPointTransform
	{
		get
		{
            return transform;
		}
	}

    private int NumberOfThingsIn = 0;

    public bool IsEmptyZone
    {
        get { return NumberOfThingsIn == 0; }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer!=LayerMask.NameToLayer("Ground"))
            NumberOfThingsIn++;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("Ground"))
            NumberOfThingsIn--;
    }

    public void Cleanup()
    {
        NumberOfThingsIn = 0;
    }
    ////if multiple respawns occurs simultaneously then there will be no firing on trigger functionality to ensure zones are marked as occupied, hence the need for a dirty variable
    //private bool m_IsDirty = false;

    ////number of tanks currently within the bounds of the spawn point
    //private int m_NumberOfTanksInZone = 0;

    ////Is the zone empty and not marked as dirty
    //public bool isEmptyZone
    //{
    //	get { return !m_IsDirty && m_NumberOfTanksInZone == 0; }
    //}

    ///// <summary>
    ///// Raises the trigger enter event - if the collider is a tank then increase the number of tanks in zone
    ///// </summary>
    ///// <param name="c">C.</param>
    //private void OnTriggerEnter(Collider c)
    //{
    //	TankHealth tankHealth = c.GetComponentInParent<TankHealth>();

    //	//if (tankHealth != null)
    //	//{
    //	//	m_NumberOfTanksInZone++;
    //	//	tankHealth.currentSpawnPoint = this;
    //	//}
    //}

    ///// <summary>
    ///// Raises the trigger exit event - if the collider is a tank then decrease the number of tanks in zone
    ///// </summary>
    ///// <param name="c">C.</param>
    //private void OnTriggerExit(Collider c)
    //{
    //    TankHealth tankHealth = c.GetComponentInParent<TankHealth>();

    //    //if (tankHealth != null)
    //    //{
    //    //	Decrement();
    //    //	tankHealth.NullifySpawnPoint(this);
    //    //}
    //}

    ///// <summary>
    ///// Safely decrement the number of tanks in zone and set isDirty to false
    ///// </summary>
    //public void Decrement()
    //{
    //	m_NumberOfTanksInZone--;
    //	if (m_NumberOfTanksInZone < 0)
    //	{
    //		m_NumberOfTanksInZone = 0;
    //	}
    //	//TODO:?? dont we need to check whether m_NumberOfTanksInZone is zero?
    //	m_IsDirty = false;
    //}

    ///// <summary>
    ///// Used to set the spawn point to dirty to prevent simultaneous spawns from occurring at the same point 
    ///// </summary>
    //public void SetDirty()
    //{
    //	m_IsDirty = true;
    //}

    ///// <summary>
    ///// Resets/cleans up the spawn point
    ///// </summary>
    //public void Cleanup()
    //{
    //	m_IsDirty = false;
    //	m_NumberOfTanksInZone = 0;
    //}
}
