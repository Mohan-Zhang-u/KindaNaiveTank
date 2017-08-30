using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

//same as TankLibrary.cs TankTypeDefinition
[Serializable]
public struct ShellTypeDefinition
{
	//Unique ID to reference tank internally
	public string id;

	//The displayed name of the tank
	public string name;

	//A short blurb describing the tank
	public string description;

	public bool NeedTurrentUp;
	//Unique stats to customize tank handling and fire rate (ITS NOW IN SHELLHANDLER
//	public float FlySpeed;
//	public float ShellfireRate;//default

//	public float damage;

	//Now, here to define Explosion!!!!
	//TODO:

	// does it uses gravaty, or can it reflect.
//	public bool UseGravity;
//	public int ReflectTimes;

	//The display prefab to be instantiated to represent this shell in the menu and in-game
	public GameObject displayPrefab;

	//How many stars are displayed per stat for this tank in the customization screen.
	[HeaderAttribute("Selection Star Rating")]
	public int damageRating;
	public int speedRating;
}


public class ShellLibrary :PersistentSingleton<ShellLibrary> {

	//An array of TankTypeDefinitions. These determine which tanks are available in the game and their properties.
	[SerializeField]
	private ShellTypeDefinition[] ShellDefinitions;

	protected override void Awake()
	{
		base.Awake();

		if (ShellDefinitions.Length == 0)
		{
			Debug.Log("<color=red>WARNING: No shells have been defined in the Tank Library!</color>");
		}
	}

	//Returns the ShellTypeDefinition for a given array index. Provides a helpful error if an invalid index is used.
	public ShellTypeDefinition GetShellDataForIndex(int index)
	{
		if ((index < 0) || ((index + 1) > ShellDefinitions.Length))
		{
			Debug.Log("<color=red>WARNING: Requested shell data index exceeds definition array bounds.</color>");
		}

		return ShellDefinitions[index];
	}

	public ShellTypeDefinition GetShellDataForName(string shellName)
	{
		foreach (ShellTypeDefinition shell in ShellDefinitions){
			if(shell.name == shellName){
				return shell;
			}
		}

		Debug.Log("<color=red>WARNING: Requested shell name does not exist.</color>");
		return ShellDefinitions[0];

	}

    public int GetNumberOfDefinitions()
	{
		return ShellDefinitions.Length;
	}

}
