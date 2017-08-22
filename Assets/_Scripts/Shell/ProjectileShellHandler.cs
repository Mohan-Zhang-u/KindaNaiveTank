using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Complete;

public class ProjectileShellHandler : ShellHandlerAbstractClass {

//	public void OnEnable(){
//		ForceChargeable = true;
//		MinShootForce = 15f;
//		MaxShootForce = 40f;
//		m_MaxChargeTime = 0.75f;
//		ShootColdDown = 0.3f;
//	}

	#region CopyAndPaste 
	///-----------------------------------------------NOW, START COPY AND PASTE!!!!!!-------------------------------------------------------------
	//TODO: its shared all accross!!!!!!!!!!!!!!!!!!!! t
	/// <summary>
	/// it is the ugliest part in C#. because, since this is a parent abstract class,
	/// if we dont copy and paste this code to its parent, the ExplosionParticles and ExplosionAudio
	/// will be THIS ABSTRACT CLASS' ExplosionParticles and ExplosionAudio.
	/// The only way we can use ExplosionParticles and ExplosionAudio in parent class is that 
	/// we copy and paste this function there.
	/// </summary>
	/// <param name="c">C.</param>
	// the main handler, AND THE DEFUALT IS, explode whenever hits a thing.!!!! TODO: can OPTIMIZE PERFORMANCE here. 
	override public void OnTriggerEnter (Collider other)
	{
		Explode (other);
	}

	//TODO: its shared all accross!!!!!!!!!!!!!!!!!!!! t
	/// <summary>
	/// it is the ugliest part in C#. because, since this is a parent abstract class,
	/// if we dont copy and paste this code to its parent, the ExplosionParticles and ExplosionAudio
	/// will be THIS ABSTRACT CLASS' ExplosionParticles and ExplosionAudio.
	/// The only way we can use ExplosionParticles and ExplosionAudio in parent class is that 
	/// we copy and paste this function there.
	/// </summary>
	/// <param name="c">C.</param>
	override public void Explode (Collider other) {
		// Collect all the colliders in a sphere from the shell's current position to a radius of the explosion radius.
		Collider[] TankColliders = Physics.OverlapSphere (transform.position, ExplosionRadius, TankMask);
		Collider[] WallColliders = Physics.OverlapSphere (transform.position, ExplosionRadius, WallMask);
		Collider[] ShellColliders = Physics.OverlapSphere (transform.position, ExplosionRadius, ShellMask);
		Collider[] GeneralItemsColliders = Physics.OverlapSphere (transform.position, ExplosionRadius, GeneralItemsMask);
		Collider[] ExplosiveItemColliders = Physics.OverlapSphere (transform.position, ExplosionRadius, ExplosiveItemsMask);
		Collider[] GroundColliders = Physics.OverlapSphere (transform.position, ExplosionRadius, GroundMask);

		// Go through all the colliders...
		for (int i = 0; i < TankColliders.Length; i++) {
			CollideWithTanks (TankColliders[i]);
		}
		for (int i = 0; i < WallColliders.Length; i++) {
			CollideWithWalls (WallColliders[i]);
		}
		for (int i = 0; i < ShellColliders.Length; i++) {
			CollideWithShells (ShellColliders[i]);
		}
		for (int i = 0; i < GeneralItemsColliders.Length; i++) {
			CollideWithGeneralItem (GeneralItemsColliders[i]);
		}
		for (int i = 0; i < ExplosiveItemColliders.Length; i++) {
			CollideWithExplosiveItems (GeneralItemsColliders[i]);
		}
		for (int i = 0; i < GroundColliders.Length; i++) {
			CollideWithGround (GroundColliders[i]);
		}

		Destroy (gameObject);
	}

	//TODO: its shared all accross!!!!!!!!!!!!!!!!!!!! t
	/// <summary>
	/// it is the ugliest part in C#. because, since this is a parent abstract class,
	/// if we dont copy and paste this code to its parent, the ExplosionParticles and ExplosionAudio
	/// will be THIS ABSTRACT CLASS' ExplosionParticles and ExplosionAudio.
	/// The only way we can use ExplosionParticles and ExplosionAudio in parent class is that 
	/// we copy and paste this function there.
	/// </summary>
	/// <param name="c">C.</param>
	override public void CollideWithTanks(Collider c){
		Rigidbody targetRigidbody = c.GetComponentInParent<Rigidbody> ();
		if (!targetRigidbody)
			return;

		// Add an explosion force.
		targetRigidbody.AddExplosionForce (ExplosionForce, transform.position, ExplosionRadius);

		// Find the TankHealth script associated with the rigidbody.
		TankHealth targetHealth = targetRigidbody.GetComponentInParent <TankHealth> ();

		// If there is no TankHealth script attached to the gameobject, go on to the next collider.
		if (!targetHealth)
			return;

		// Calculate the amount of damage the target should take based on it's distance from the shell.
		float damage = CalculateDamage (targetRigidbody.position);

		// Deal this damage to the tank.
		targetHealth.Damage (damage, FireByTankId, ExplosionId);

		if (ExplosionParticles) {
			// Unparent the particles from the shell.
			ExplosionParticles.transform.parent = null;

			// Play the particle system.
			ExplosionParticles.Play ();

			//TODO: particle system is used in shellhandlers.
			// Once the particles have finished, destroy the gameobject they are on.
			ParticleSystem.MainModule mainModule = ExplosionParticles.main;
			Destroy (ExplosionParticles.gameObject, mainModule.duration);
		}
		// Play the explosion sound effect.
		if (ExplosionAudio)
			ExplosionAudio.Play();
	}

	//TODO: im not pretty sure whether its is CORRECT!!!!!!!!!!!!!!!!!!!!
	override public void CollideWithShells(Collider c){
		if (c.isTrigger) {
			c.GetComponent<ShellHandlerAbstractClass> ().Explode (ShellPrefab.GetComponent<Collider> ());
		}
	}

	// targetPosition is tank's position. the function calculates according to tank's position and the explosive(shell's) position
	override public float CalculateDamage (Vector3 targetPosition)
	{
		// Create a vector from the shell to the target.
		Vector3 explosionToTarget = targetPosition - transform.position;

		// Calculate the distance from the shell to the target.
		float explosionDistance = explosionToTarget.magnitude;

		// Calculate the proportion of the maximum distance (the explosionRadius) the target is away.
		float relativeDistance = (ExplosionRadius - explosionDistance) / ExplosionRadius;

		// Calculate damage as this proportion of the maximum possible damage.
		float damage = relativeDistance * MaxDamage;

		// Make sure that the minimum damage is always 0.
		damage = Mathf.Max (0f, damage);

		return damage;
	}


	///-----------------------------------------------NOW, END COPY AND PASTE!!!!!!-------------------------------------------------------------
	#endregion
		
}
