using UnityEngine;
using System.Collections.Generic;
using Complete;
//using Shells.Data;
//using Shells.Effects;

public class ShellDisplay : MonoBehaviour {

	// //Array of particle emitters for track dust.
	// private ParticleSystem[] m_TrackTrailParticles;

	// //Array of transforms to which track trail particle emitters are to be attached.
	// [SerializeField]
	// protected Transform[] m_DustTrails;

	// //Reference to nitro effect particle emitter.
	// [SerializeField]
	// protected ParticleSystem m_NitroParticles;

	// //Reference to shell shadow object.
	// [SerializeField]
	// protected MeshRenderer m_Shadow;


	// !!!!!
	//Reference to the transform that indicates where shells will be instantiated on firing.
	[SerializeField]
	protected Transform ShellTransform;

	//Reference to all renderers that are to be colour tinted.
	[SerializeField]
	protected Renderer[] m_ShellRenderers;

	[HideInInspector]
	public TankShooting TankShootingScript; // this thing is initialized in TankShooting right after Instantiate shellInstance
//	public bool AlreadySpawned = false;

	//TODO: now give shell fire, explosion sound effects and particle effects.!!!!!!!!!!!!!!!!!!

	//Returns the fire transform for this shell.
	public Transform GetShellTransform()
	{
		return ShellTransform;
	}

	//TODO: destroy after a time lets say 10 seconds!!!!!!!!!!!!!!!!!

	//Enables/disables individual gameobjects
	protected void SetGameObjectActive(GameObject gameObject, bool active)
	{
		if (gameObject == null)
		{
			return;
		}

		gameObject.SetActive(active);
	}


	public void SetShellColor(Color newColor)
	{
		// Go through all the renderers...
		for (int i = 0; i < m_ShellRenderers.Length; i++)
		{
			// ... set their material color to the color specific to this shell.
			m_ShellRenderers[i].material.color = newColor;
		}
	}

	// when the shell is destoryed, notify TankShooting.
	public void OnDestroy () {
		TankShootingScript.RespawnedShellExploded ();
	}
}
