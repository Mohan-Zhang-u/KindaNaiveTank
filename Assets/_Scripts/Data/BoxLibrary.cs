using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

//same as TankLibrary.cs TankTypeDefinition
[Serializable]
public struct BoxTypeDefinition
{
	//Unique ID to reference box internally
	public string id;

	//The displayed name of the box
	public string name;

    //A short blurb describing the box     powerupPrefab
    public string description;

    //The relative probability of this object spawning. 用来加权平均?
    public int dropWeighting;

    //The display prefab to be instantiated to represent this box in the menu and in-game, the prefab shall has scripts attached to.
    public GameObject displayPrefab;
}


public class BoxLibrary :PersistentSingleton<BoxLibrary> {

    //Reference to the ScriptableObject defining the explosion when powerups spawn.
    public ExplosionSettings m_SpawnExplosion;

    //The prefab to spawn to indicate an incoming drop, and a temporary reference variable so we can early-out it if necessary.
    public GameObject m_HotdropEffectPrefab;

    public GameObject BoxExplosionEffect;

    //An array of TankTypeDefinitions. These determine which boxs are available in the game and their properties.
    [SerializeField]
	private BoxTypeDefinition[] BoxDefinitions;

	protected override void Awake()
	{
		base.Awake();

		if (BoxDefinitions.Length == 0)
		{
			Debug.Log("<color=red>WARNING: No boxes have been defined in the Tank Library!</color>");
		}
	}

	//Returns the boxTypeDefinition for a given array index. Provides a helpful error if an invalid index is used.
	public BoxTypeDefinition GetBoxDataForIndex(int index)
	{
		if ((index < 0) || ((index + 1) > BoxDefinitions.Length))
		{
			Debug.Log("<color=red>WARNING: Requested box data index exceeds definition array bounds.</color>");
		}

		return BoxDefinitions[index];
	}

	public BoxTypeDefinition GetBoxDataForName(string boxName, out bool success)
	{
		foreach (BoxTypeDefinition box in BoxDefinitions){
			if(box.name == boxName){
                success = true;
				return box;
			}
		}
        success = false;
        Debug.Log("<color=red>WARNING: Requested box name does not exist, with name:</color>"+ boxName);
		return BoxDefinitions[0];

	}

	public int GetNumberOfDefinitions()
	{
		return BoxDefinitions.Length;
	}
}
