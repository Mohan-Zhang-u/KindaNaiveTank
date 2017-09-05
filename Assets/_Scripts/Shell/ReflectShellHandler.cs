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
	public Vector3 oldVelocity;

	void FixedUpdate () {
		// because we want the velocity after physics, we put this in fixed update
		oldVelocity = gameObject.GetComponent<Rigidbody>().velocity;
	}

	void OnCollisionEnter (Collision collisionc) {
		if (LayerMask.LayerToName (collisionc.gameObject.layer) != "Wall") {
			Explode (collisionc.collider);
		} else {
			if (ReflectOrBounceTimes <= 0) {
				Explode (collisionc.collider);
			} else {
				ReflectOrBounceTimes -= 1;
			
				// get the point of contact
				ContactPoint contact = collisionc.contacts [0];
				// reflect our old velocity off the contact point's normal vector
				Vector3 reflectedVelocity = Vector3.Reflect (oldVelocity, contact.normal);        

				// assign the reflected velocity back to the rigidbody
				gameObject.GetComponent<Rigidbody> ().velocity = reflectedVelocity;
				// rotate the object by the same ammount we changed its velocity
				Quaternion rotation = Quaternion.FromToRotation (oldVelocity, reflectedVelocity);
				gameObject.GetComponent<Rigidbody> ().rotation = rotation * transform.rotation;
			}
		}
	}

//	new public void OnTriggerEnter (Collider other){
//		Debug.Log ("OnTriggerEnter1"+other.name);
////		if (LayerMask.LayerToName (other.gameObject.layer) == "Wall") {
////			if (ReflectOrBounceTimes <= 0) {
////				Explode (other);
////			}
////			else{
////				ReflectOrBounceTimes -= 1;
//////				ShellReflect (other);
////			}
////		}
////
//	}
//

}
