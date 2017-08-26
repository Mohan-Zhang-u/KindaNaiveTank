//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.Networking;

//public class GameMangerBase : NetworkBehaviour
//{

//    //Pickups
//    private List<BoxBase> m_PowerupList;

//    public BoxSpawnManager _BoxSpawnManagerScript;

//    private void Awake()
//    {
//        m_PowerupList = new List<BoxBase>();

//    }

//#region PowerUpRelated
//    /// <summary>
//    /// Adds the powerup.
//    /// </summary>
//    /// <param name="powerUp">Power up.</param>
//    public void AddPowerup(BoxBase powerUp)
//    {
//        m_PowerupList.Add(powerUp);
//    }

//    /// <summary>
//    /// Removes the powerup.
//    /// </summary>
//    /// <param name="powerup">Powerup.</param>
//    public void RemovePowerup(BoxBase powerup)
//    {
//        m_PowerupList.Remove(powerup);
//    }

//    /// <summary>
//    /// Cleanups the powerups
//    /// </summary>
//    private void CleanupPowerups()
//    {
//        for (int i = (m_PowerupList.Count - 1); i >= 0; i--)
//        {
//            if (m_PowerupList[i] != null)
//            {
//                // TODO:NetworkServer.Destroy(m_PowerupList[i].gameObject);
//                Destroy(m_PowerupList[i].gameObject);
//            }
//        }
//    }
//#endregion

//}
