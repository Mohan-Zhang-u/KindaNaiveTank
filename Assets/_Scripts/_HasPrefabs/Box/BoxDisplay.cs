using UnityEngine;
using System.Collections.Generic;
//using Boxs.Data;
//using Boxs.Effects;

public class BoxDisplay : MonoBehaviour {

	// //Array of particle emitters for track dust.
	// private ParticleSystem[] m_TrackTrailParticles;

	// //Array of transforms to which track trail particle emitters are to be attached.
	// [SerializeField]
	// protected Transform[] m_DustTrails;

	// //Reference to nitro effect particle emitter.
	// [SerializeField]
	// protected ParticleSystem m_NitroParticles;

	// //Reference to box shadow object.
	// [SerializeField]
	// protected MeshRenderer m_Shadow;

	// !!!!!
	//Reference to the transform that indicates where boxs will be instantiated on firing.
	[SerializeField]
	protected Transform BoxTransform;

	public bool AlreadySpawned = false;

//	//Reference to all renderers that are to be colour tinted.
//	[SerializeField]
//	protected Renderer[] m_BoxRenderers;


	//Returns the fire transform for this box.
	public Transform GetBoxTransform()
	{
		return BoxTransform;
	}

	//TODO: implement the box collect-related functions.

	//Enables/disables individual gameobjects
	protected void SetGameObjectActive(GameObject gameObject, bool active)
	{
		if (gameObject == null)
		{
			return;
		}

		gameObject.SetActive(active);
	}


//	public void SetBoxColor(Color newColor)
//	{
//		// Go through all the renderers...
//		for (int i = 0; i < m_BoxRenderers.Length; i++)
//		{
//			// ... set their material color to the color specific to this box.
//			m_BoxRenderers[i].material.color = newColor;
//		}
//	}

}
