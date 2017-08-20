using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

//same as TankLibrary.cs TankTypeDefinition
[Serializable]
public struct BoxTypeDefinition
{
	//Unique ID to reference tank internally
	public string id;

	//The displayed name of the tank
	public string name;

	//A short blurb describing the tank
	public string description;

	//The display prefab to be instantiated to represent this box in the menu and in-game
	public GameObject displayPrefab;

}


public class BoxLibrary :PersistentSingleton<BoxLibrary> {

	//An array of TankTypeDefinitions. These determine which tanks are available in the game and their properties.
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

	public BoxTypeDefinition GetBoxDataForName(string boxName)
	{
		foreach (BoxTypeDefinition box in BoxDefinitions){
			if(box.name == boxName){
				return box;
			}
		}

		Debug.Log("<color=red>WARNING: Requested box name does not exist.</color>");
		return BoxDefinitions[0];

	}

	public int GetNumberOfDefinitions()
	{
		return BoxDefinitions.Length;
	}

}
