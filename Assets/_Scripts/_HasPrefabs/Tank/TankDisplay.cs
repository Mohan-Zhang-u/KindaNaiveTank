﻿using UnityEngine;
using System.Collections.Generic;
//using Tanks.Data;
//using Tanks.Effects;

public class TankDisplay : MonoBehaviour {

    [SerializeField]
    protected bool IsLocked = false;
    //Root object for all tank mesh renderer/mesh objects. Used to mass enable/disable them.
    [SerializeField]
	protected GameObject m_TankRendererParent;

	// //Array of particle emitters for track dust.
	// private ParticleSystem[] m_TrackTrailParticles;

	// //Array of transforms to which track trail particle emitters are to be attached.
	// [SerializeField]
	// protected Transform[] m_DustTrails;

	// //Reference to nitro effect particle emitter.
	// [SerializeField]
	// protected ParticleSystem m_NitroParticles;

	// //Reference to tank shadow object.
	// [SerializeField]
	// protected MeshRenderer m_Shadow;

	// !!!!!
	//Reference to the transform that indicates where shells will be instantiated on firing.
	private Transform m_FireTransform;

	[SerializeField]
	protected Transform m_FireTransformUp;

	[SerializeField]
	protected Transform m_FireTransformDown;

	//Reference to the turret root, for rotation.
	[SerializeField]
	protected Transform m_TurretTransform;

	//Reference to the shield bubble object.
	[SerializeField]
	protected GameObject m_ShieldBubble;

	//Reference to all renderers that are to be colour tinted.
	[SerializeField]
	protected Renderer[] m_TankRenderers;

	//These transforms represent the placement points for tank decorations. Their indices must correspond to the TankDecorationType enum value less 1 (ie. Hats are at index 0).
	[SerializeField]
	protected Transform[] m_DecorationPoints;

	// Settings that affect the amount of shake the tank has depending on its state
	[SerializeField]
	protected Vector3 m_ShakeDirections;
	[SerializeField]
	protected float m_IdleShakeMagnitude;
	[SerializeField]
	protected float m_IdleShakeNoiseScale;
	[SerializeField]
	protected float m_MovingShakeMagnitude;
	[SerializeField]
	protected float m_MovingShakeNoiseScale;
	[SerializeField]
	protected float m_BobFrequency;
	[SerializeField]
	protected float m_BobNoiseScale;

	public bool AlreadySpawned = false;
	//Now Myself:!!!!!!!!!!!!
	// the default value if turrent is lower. Check this!
	[SerializeField]
	protected Animation TurrentAni;


	//sync this? [syncVar]
	public bool IsTurrentLower = true;


	//Now Myself:!!!!!!!!!!!!
	public Transform RiseTurrent(){
		if (IsTurrentLower) {
			TurrentAni.PlayQueued ("TurrentRiseAni");
		}
		m_FireTransform = m_FireTransformUp;
		IsTurrentLower = false;
		return GetFireTransform ();
	}

	public Transform DownTurrent(){
		if (!IsTurrentLower) {
			TurrentAni.PlayQueued ("TurrentDownAni");
		}
		m_FireTransform = m_FireTransformDown;
		IsTurrentLower = true;
		return GetFireTransform ();
	}



	//Returns the fire transform for this tank.
	public Transform GetFireTransform()
	{
		return m_FireTransform;
	}

	//Enables/disables visible elements of the tank.
	public void SetVisibleObjectsActive(bool active)
	{
		// if (!active)
		// {
		// 	StopTrackParticles();
		// }

		SetGameObjectActive(m_TankRendererParent, active);
		// if (m_Shadow != null)
		// {
		// 	SetGameObjectActive(m_Shadow.gameObject, active);
		// }
	}

	//Enables/disables individual gameobjects
	protected void SetGameObjectActive(GameObject gameObject, bool active)
	{
		if (gameObject == null)
		{
			return;
		}

		gameObject.SetActive(active);
	}


	public void SetTankColor(Color newColor)
	{
		// Go through all the renderers...
		for (int i = 0; i < m_TankRenderers.Length; i++)
		{
			// ... set their material color to the color specific to this tank.
			m_TankRenderers[i].material.color = newColor;
		}
	}

	//Sets the shield bubble object on or off
	public void SetShieldBubbleActive(bool active)
	{
		if (m_ShieldBubble != null)
		{
			m_ShieldBubble.SetActive(active);
//			m_ShieldBubble.GetComponent<MeshRenderer> ().enabled = active;
//			Debug.Log ("seted "+active);
//			//----------------------------------------------
//			GameObject [] ga = GameObject.FindGameObjectsWithTag("Shield");
//			Debug.Log ("ga length:"+ga.Length);
//			GameObject gb = this.gameObject;
//			GameObject gbf = this.gameObject;
//			foreach (GameObject gb1 in ga) {
//				if (gb1.name == "Shield") {
//					Debug.Log ("FOUND gb");
//					gb = gb1;
//				}
//			}
////			Debug.Log ("eq:" + (gb == m_ShieldBubble).ToString ());
//			Debug.Log ("gbpos:"+gb.transform.position);
//			Debug.Log ("m_ShieldBubblepos:"+m_ShieldBubble.transform.position);
//			Debug.Log ("eq:" + (gb == m_ShieldBubble).ToString ());
//			Debug.Log ("eqparent:" + (gb.transform.parent == m_ShieldBubble.transform.parent).ToString ());
////			gb.SetActive (false);
//			m_ShieldBubble.SetActive(true);
//			Debug.Log ("afterset:" + m_ShieldBubble.activeSelf);
//			Debug.Log ("activeInHierarchy:" + gb.activeInHierarchy + "," + m_ShieldBubble.activeInHierarchy);
//			m_ShieldBubble.SetActive (true);
//			gb.transform.position = new Vector3 (-10, 2, 0);
//			m_ShieldBubble.transform.position = new Vector3 (10, 2, 0);
//			Debug.Log (m_ShieldBubble.transform.position);
//			foreach (GameObject gb1 in ga) {
//				if (gb1.name == "Shield") {
//					Debug.Log ("FOUND gbf");
//					gbf = gb1;
////					gb1.
//				}
//			}
//			//-------------------------------------------------
		}
	}


	//actually dont need. we have turrent animations.
	//Returns the turret transform for this tank.
	// public Transform GetTurretTransform()
	// {
	// 	return m_TurretTransform;
	// }

	// // Stored reference to TankManager
	// private TankManager m_TankManager;

	// // Collection of all attached decorations
	// private List<Decoration> m_AttachedDecorations;

	// // Reference to DamageOutlineFlash effect controller
	// private DamageOutlineFlash m_DamageFlash;

	// private void Awake()
	// {
	// 	m_AttachedDecorations = new List<Decoration>();
	// 	m_DamageFlash = GetComponent<DamageOutlineFlash>();
	// }

	// private void Start()
	// {
		// //Instantiate the correct track particles for this map type
		// GameObject trailObject = ThemedEffectsLibrary.s_Instance.GetTrackParticlesForMap();

		// m_TrackTrailParticles = new ParticleSystem[m_DustTrails.Length];

		// for (int i = 0; i < m_DustTrails.Length; i++)
		// {
		// 	GameObject newTrailEffect = (GameObject)Instantiate(trailObject, m_DustTrails[i].position, m_DustTrails[i].rotation, m_DustTrails[i]);
		// 	newTrailEffect.transform.localScale = Vector3.one;
		// 	m_TrackTrailParticles[i] = newTrailEffect.transform.GetComponent<ParticleSystem>();
		// }
	// }

	// private void Update()
	// {
	// 	DoShake();
	// }

	// //Shakes the tank model about as a "chugging" animation.
	// private void DoShake()
	// {
	// 	if (m_TankManager == null || m_TankManager.movement == null)
	// 	{
	// 		return;
	// 	}

	// 	bool moving = m_TankManager.movement.isMoving;
	// 	float shakeMagnitude = moving ? m_MovingShakeMagnitude : m_IdleShakeMagnitude;
	// 	float shakeScale = moving ? m_MovingShakeNoiseScale : m_IdleShakeNoiseScale;

	// 	float xNoise = (Mathf.PerlinNoise((Time.realtimeSinceStartup + 0) * shakeScale, Time.smoothDeltaTime) * 2 - 1) * shakeMagnitude;
	// 	float zNoise = (Mathf.PerlinNoise((Time.realtimeSinceStartup + 100) * shakeScale, Time.smoothDeltaTime) * 2 - 1) * shakeMagnitude;

	// 	float yNoise = Mathf.Abs(Mathf.Sin(Time.realtimeSinceStartup * Mathf.PI * m_BobFrequency)) * shakeMagnitude;
	// 	yNoise *= Mathf.PerlinNoise((Time.realtimeSinceStartup + 50) * m_BobNoiseScale, Time.smoothDeltaTime);

	// 	Vector3 offset = Vector3.Scale(m_ShakeDirections, new Vector3(xNoise, yNoise, zNoise));
	// 	m_TankRendererParent.transform.localPosition = offset;
	// }

	// public void Init(TankManager tankManager)
	// {
	// 	this.m_TankManager = tankManager;
	// 	SetTankColor(tankManager.playerColor);
	// }

	// //Stops and clears the particles for tank movement.
	// public void StopTrackParticles()
	// {
	// 	if (m_TrackTrailParticles != null)
	// 	{
	// 		for (int i = 0; i < m_TrackTrailParticles.Length; i++)
	// 		{
	// 			m_TrackTrailParticles[i].Clear();
	// 			m_TrackTrailParticles[i].Stop();
	// 		}
	// 	}
	// }

	// //Restarts the track particle emitters.
	// public void ReEnableTrackParticles()
	// {
	// 	if (m_TrackTrailParticles != null)
	// 	{
	// 		for (int i = 0; i < m_TrackTrailParticles.Length; i++)
	// 		{
	// 			m_TrackTrailParticles[i].Play();
	// 		}
	// 	}
	// }


	// public void SetTankDecoration(int newDecorationId, int newMaterialIndex, bool destroyDecorations = true)
	// {
	// 	//Get decoration data
	// 	TankDecorationDefinition itemData = TankDecorationLibrary.s_Instance.GetDecorationForIndex(newDecorationId);

	// 	//Iterate through all decoration points and clear them
	// 	if (destroyDecorations)
	// 	{
	// 		for (int i = 0; i < m_AttachedDecorations.Count; i++)
	// 		{
	// 			Decoration decoration = m_AttachedDecorations[i];
	// 			if (decoration != null)
	// 			{
	// 				Destroy(decoration.gameObject);
	// 			}
	// 		}
	// 	}

	// 	//Instantiate and place the relevant object. Since a decorationType value of 0 equates to "no items", we subtract 1 from the enum to get the intended location transform array index
	// 	if (itemData.id != "none")
	// 	{
	// 		Transform positionTransform = m_DecorationPoints[newDecorationId - 1];

	// 		Decoration newDecoration = (Decoration)Instantiate(itemData.decorationPrefab, positionTransform.position, positionTransform.rotation, positionTransform);

	// 		if (TankDecorationLibrary.s_Instance.GetMaterialQuantityForIndex(newDecorationId) > 0)
	// 		{
	// 			newDecoration.SetMaterial(TankDecorationLibrary.s_Instance.GetMaterialForDecoration(newDecorationId, newMaterialIndex));
	// 		}

	// 		newDecoration.Attach(m_TankManager);
	// 		m_AttachedDecorations.Add(newDecoration);
	// 	}
	// }

	// public void DetachDecorations()
	// {
	// 	for (int i = 0; i < m_AttachedDecorations.Count; i++)
	// 	{
	// 		Decoration decoration = m_AttachedDecorations[i];
	// 		if (decoration != null)
	// 		{
	// 			decoration.Detach();
	// 		}
	// 	}
	// }

	// //Activates or deactivates the special Nitro particle system.
	// public void SetNitroParticlesActive(bool active)
	// {
	// 	if (active)
	// 	{
	// 		if (!m_NitroParticles.isPlaying)
	// 		{
	// 			m_NitroParticles.Play();
	// 		}
	// 	}
	// 	else
	// 	{
	// 		m_NitroParticles.Stop();
	// 	}
	// }

	// //Signals the attached DamageOutlineFlash script to pulse the tank's outline in response to damage.
	// public void StartDamageFlash()
	// {
	// 	m_DamageFlash.StartDamageFlash();
	// }

	// //Gets the total bounds of this tank model and its equipped decorations for viewport fitting in the main menu.
	// public Bounds GetTankBounds()
	// {
	// 	Bounds? objectBounds = null;

	// 	foreach (Renderer rend in GetComponentsInChildren<MeshRenderer>())
	// 	{
	// 		if (rend.enabled && rend.gameObject.activeInHierarchy)
	// 		{
	// 			Bounds rendBounds = rend.bounds;
	// 			// Only on bounds with volume
	// 			if (rendBounds.size.x > 0 &&
	// 			    rendBounds.size.y > 0 &&
	// 			    rendBounds.size.z > 0)
	// 			{
	// 				if (objectBounds.HasValue)
	// 				{
	// 					Bounds boundVal = objectBounds.Value;
	// 					boundVal.Encapsulate(rendBounds);
	// 					objectBounds = boundVal;
	// 				}
	// 				else
	// 				{
	// 					objectBounds = rend.bounds;
	// 				}
	// 			}
	// 		}
	// 	}

	// 	// Encapsulate decorations
	// 	foreach (Decoration dec in m_AttachedDecorations)
	// 	{
	// 		if (dec != null)
	// 		{
	// 			Bounds? decBounds = dec.GetDecorationBounds();
	// 			if (decBounds.HasValue)
	// 			{
	// 				Bounds boundVal = objectBounds.Value;
	// 				boundVal.Encapsulate(decBounds.Value);
	// 				objectBounds = boundVal;
	// 			}
	// 		}
	// 	}

	// 	return objectBounds.Value;
	// }

	// //Shakes the tank model about as a "chugging" animation.
	// private void DoShake()
	// {
	// 	if (m_TankManager == null || m_TankManager.movement == null)
	// 	{
	// 		return;
	// 	}

	// 	bool moving = m_TankManager.movement.isMoving;
	// 	float shakeMagnitude = moving ? m_MovingShakeMagnitude : m_IdleShakeMagnitude;
	// 	float shakeScale = moving ? m_MovingShakeNoiseScale : m_IdleShakeNoiseScale;

	// 	float xNoise = (Mathf.PerlinNoise((Time.realtimeSinceStartup + 0) * shakeScale, Time.smoothDeltaTime) * 2 - 1) * shakeMagnitude;
	// 	float zNoise = (Mathf.PerlinNoise((Time.realtimeSinceStartup + 100) * shakeScale, Time.smoothDeltaTime) * 2 - 1) * shakeMagnitude;

	// 	float yNoise = Mathf.Abs(Mathf.Sin(Time.realtimeSinceStartup * Mathf.PI * m_BobFrequency)) * shakeMagnitude;
	// 	yNoise *= Mathf.PerlinNoise((Time.realtimeSinceStartup + 50) * m_BobNoiseScale, Time.smoothDeltaTime);

	// 	Vector3 offset = Vector3.Scale(m_ShakeDirections, new Vector3(xNoise, yNoise, zNoise));
	// 	m_TankRendererParent.transform.localPosition = offset;
	// }
}
