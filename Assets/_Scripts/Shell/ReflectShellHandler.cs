using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Complete;

public class ReflectShellHandler : ShellHandlerAbstractClass {
//	public void OnEnable(){
//		ReflectOrBounceTimes = 3;
//		ShootColdDown = 0.3f; // if ShootColdDown = 0, just like a laser sword.
//		FlyingSpeed = 30f; //if not ForceChargeable, its speed shall be a constant.
//		ForceChargeable = false; // have a shoot distance? the more the far or fast? or not?
//	}

	override public void CollideWithTanks(Collider c){
		Debug.Log ("executing collide with tank");

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

//	void OnCollisionEnter (Collision collision) {
//		Debug.Log ("OnCollisionEnter");
////		if (ReflectOrBounceTimes <= 0) {
////			return;
////		}
////		else{
////			ReflectOrBounceTimes -= 1;
////		
////			// get the point of contact
////			ContactPoint contact = collision.contacts[0];
////			Vector3 oldVelocity = gameObject.GetComponent<Rigidbody> ().velocity;
////			// reflect our old velocity off the contact point's normal vector
////			Vector3 reflectedVelocity = Vector3.Reflect(oldVelocity, contact.normal);        
////
////			// assign the reflected velocity back to the rigidbody
////			gameObject.GetComponent<Rigidbody> ().velocity = reflectedVelocity;
////			// rotate the object by the same ammount we changed its velocity
////			Quaternion rotation = Quaternion.FromToRotation(oldVelocity, reflectedVelocity);
////			gameObject.GetComponent<Rigidbody> ().rotation = rotation * transform.rotation;
////		}
//	}

	new public void OnTriggerEnter (Collider other){
		Debug.Log ("OnTriggerEnter1"+other.name);
		if (LayerMask.LayerToName (other.gameObject.layer) == "Wall") {
			if (ReflectOrBounceTimes <= 0) {
				Explode (other);
			}
			else{
				ReflectOrBounceTimes -= 1;
//				ShellReflect (other);
			}
		}

	}

	// TODO: implement. but, if added proper constraint (e.g. only move in x and z direction), should do this by itself.
//	private void ShellReflect(Collider wallc){
//		// Debug.Log ("entered111");
//	}

}
