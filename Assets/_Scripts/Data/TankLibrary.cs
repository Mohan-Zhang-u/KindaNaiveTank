﻿using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

//same as TankLibrary.cs TankTypeDefinition
[Serializable]
public struct TankTypeDefinition
{
	//Unique ID to reference tank internally
	public string id;

	//The displayed name of the tank
	public string name;

	//A short blurb describing the tank
	public string description;

	//Unique stats to customize tank handling and fire rate
	public float speed;
	public float rotationSpeed;//public float turnRate;
	public float fireRateMultiplier;//default 1f

	public float StartHealth; //default 100f
	public float armour;//default 1f

	//The display prefab to be instantiated to represent this tank in the menu and in-game
	public GameObject[] displayPrefab;

	//How much this tank costs to unlock in the game's internal currency.
	public int[] cost;

	[HeaderAttribute("Selection For parent RigidBody")]

	public float TankMass; // default 1
	public float TankDrag; // default 0
	public float TankAngularDrag; // default 0.05

	[HeaderAttribute("AudioClips And ParticleSystems (used for TankShooting)")]

	public AudioClip m_ChargingClip;  // default ShotCharging          // Audio that plays when each shot is charging up.
	public AudioClip m_FireClip;  // default ShotFiring              // Audio that plays when each shot is fired.

	[HeaderAttribute("AudioClips And ParticleSystems (used for TankMovement)")]
	public AudioClip m_EngineIdling;  // default EngineIdle         // Audio to play when the tank isn't moving.
	public AudioClip m_EngineDriving; // default EngineDriving          // Audio to play when the tank is moving.
	public float m_PitchRange; //default = 0.2          // The amount by which the pitch of the engine noises can vary.
	public ParticleSystem[] m_particleSystems;

	[HeaderAttribute("AudioClips And ParticleSystems (used for TankHealth)")]
	public GameObject TankExplosionPrefab; // default CompleteTankExplosion

	//How many stars are displayed per stat for this tank in the customization screen.
	[HeaderAttribute("Selection Star Rating")]
	public int armourRating;
	public int speedRating;
	public int refireRating;
}


public class TankLibrary :PersistentSingleton<TankLibrary> {

	//An array of TankTypeDefinitions. These determine which tanks are available in the game and their properties.
	[SerializeField]
	private TankTypeDefinition[] tankDefinitions;

	protected override void Awake()
	{
		base.Awake();

		if (tankDefinitions.Length == 0)
		{
			Debug.Log("<color=red>WARNING: No tanks have been defined in the Tank Library!</color>");
		}
	}

	//Returns the TankTypeDefinition for a given array index. Provides a helpful error if an invalid index is used.
	public TankTypeDefinition GetTankDataForIndex(int index)
	{
		if ((index < 0) || ((index + 1) > tankDefinitions.Length))
		{
			Debug.Log("<color=red>WARNING: Requested tank data index exceeds definition array bounds.</color>");
		}

		return tankDefinitions[index];
	}

	public TankTypeDefinition GetTankDataForName(string tankName)
	{
		foreach (TankTypeDefinition tank in tankDefinitions){
			if(tank.name == tankName){
				return tank;
			}
		}

		Debug.Log("<color=red>WARNING: Requested tank name does not exist.</color>");
		return tankDefinitions[0];

	}

	// // Returns an array containing data for tanks that have been unlocked.
	// public List<TankTypeDefinition> GetUnlockedTanks()
	// {
	// 	List<TankTypeDefinition> unlockedTanks = new List<TankTypeDefinition>();
	// 	int length = tankDefinitions.Length;
	// 	for (int i = 0; i < length; i++)
	// 	{
	// 		if (PlayerDataManager.s_Instance.IsTankUnlocked(i))
	// 		{
	// 			unlockedTanks.Add(tankDefinitions[i]);
	// 		}
	// 	}
	// 	return unlockedTanks;
	// }

	// //Returns the quantity of tanks that have been unlocked.
	// public int GetNumberOfUnlockedTanks()
	// {
	// 	return GetUnlockedTanks().Count;
	// }

	//Returns how many tank definitions we have in our library array.
	public int GetNumberOfDefinitions()
	{
		return tankDefinitions.Length;
	}

}
