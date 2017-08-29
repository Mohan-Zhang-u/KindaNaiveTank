using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityRandom = UnityEngine.Random;

public class GameManagerBase : MonoBehaviour
{

    //Pickups
    private BoxLibrary BoxLibraryScript;
    private List<BoxBase> m_PowerupList;  // this is used to contain all instantiated boxes, and ezly destroy them afterwards.
    private List<BoxTypeDefinition> _AllowedBoxTypes;

    public string[] IdOfAllowedBoxTypes; //this is to fill in when constructing scenes.
    private int totalweight;

    // this is public because inited in under a same gameobject
    public BoxSpawnManager _BoxSpawnManagerScript;



    private void OnEnable()
    {
        BoxLibraryScript = FindObjectOfType<BoxLibrary>();
        m_PowerupList = new List<BoxBase>();
        _AllowedBoxTypes = new List<BoxTypeDefinition>();
        // to ensure Singleton(only one instance GameManagerBase can exists)
        if (FindObjectOfType<GameManagerBase>() != this)
        {
            Destroy(this);
        }
        InitAllowedBoxTypes();
    }

    private void Update()
    {

    }

    private void InitAllowedBoxTypes()
    {
        bool success;
        BoxTypeDefinition tempBox;
        foreach (string id in IdOfAllowedBoxTypes)
        {
            tempBox = BoxLibraryScript.GetBoxDataForName(id, out success);
            if (!success)
            {
                Debug.Log("<color = red> Cannot Find Box definition with such id! check your IdOfAllowedBoxTypes in GameManagerBase!</color>");
            }
            else
            {
                _AllowedBoxTypes.Add(tempBox);
                totalweight += tempBox.dropWeighting;
            }
        }
    }

    #region PowerUpRelated
    /// <summary>
    /// Adds the powerup.
    /// </summary>
    /// <param name="powerUp">Power up.</param>
    public void AddPowerup(BoxBase powerUp)
    {
        m_PowerupList.Add(powerUp);
    }

    /// <summary>
    /// Removes the powerup.
    /// </summary>
    /// <param name="powerup">Powerup.</param>
    public void RemovePowerup(BoxBase powerup)
    {
        m_PowerupList.Remove(powerup);
    }

    /// <summary>
    /// Cleanups the powerups
    /// </summary>
    private void CleanupPowerups()
    {
        for (int i = (m_PowerupList.Count - 1); i >= 0; i--)
        {
            if (m_PowerupList[i] != null)
            {
                // TODO:NetworkServer.Destroy(m_PowerupList[i].gameObject);
                Destroy(m_PowerupList[i].gameObject);
            }
        }
    }
    #endregion

    // My functions.-----------------------------------------------------------------
    public BoxTypeDefinition GetRandomBox()
    {
        if (totalweight <= 0)
        {
            throw new ArgumentException("WeightSum should be a positive value", "weightSum");
        }

        int selectionIndex = 0;
        int selectionWeightIndex = UnityRandom.Range(0, totalweight);
        int elementCount = _AllowedBoxTypes.Count;

        if (elementCount == 0)
        {
            throw new InvalidOperationException("Cannot perform selection on an empty collection");
        }

        int itemWeight = _AllowedBoxTypes[selectionIndex].dropWeighting;
        while (selectionWeightIndex >= itemWeight)
        {
            selectionWeightIndex -= itemWeight;
            selectionIndex++;

            if (selectionIndex >= elementCount)
            {
                throw new ArgumentException("Weighted selection exceeded indexable range. Is your weightSum correct?", "weightSum");
            }

            itemWeight = _AllowedBoxTypes[selectionIndex].dropWeighting;
        }

        return _AllowedBoxTypes[selectionIndex];
    }

}



