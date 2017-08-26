using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMovementScript : MonoBehaviour {

	public float m_Speed = 12f; 
	public VirtualJoyStickScript Joystick;

	private Rigidbody m_Rigidbody;

	private void Awake ()
	{
		m_Rigidbody = GetComponent<Rigidbody> ();
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	private void FixedUpdate ()
	{
		// Adjust the rigidbodies position and orientation in FixedUpdate.
		Move ();
		//now do the rotation
		if (Joystick.JoyStickInputVectors != Vector3.zero) { //if joystick's not pressed, look to last direction.
			transform.rotation = Quaternion.Slerp (transform.rotation, Quaternion.LookRotation (Joystick.JoyStickInputVectors), Time.deltaTime * 10f);
		} 
	}

	private void Move ()
	{
		// Create a vector in the direction the tank is facing with a magnitude based on the input, speed and the time between frames.
		float movement = Joystick.JoyStickInputVectors.magnitude * m_Speed * Time.deltaTime;
        m_Rigidbody.velocity = transform.forward * movement * 40;
		// Apply this movement to the rigidbody's position.
		//m_Rigidbody.MovePosition(m_Rigidbody.position + movement);
	}


//	private void Turn ()
//	{
//		// Determine the number of degrees to be turned based on the input, speed and time between frames.
//		float turn = m_TurnInputValue * m_TurnSpeed * Time.deltaTime;
//
//		// Make this into a rotation in the y axis.
//		Quaternion turnRotation = Quaternion.Euler (0f, turn, 0f);
//
//		// Apply this rotation to the rigidbody's rotation.
//		m_Rigidbody.MoveRotation (m_Rigidbody.rotation * turnRotation);
//	}
}
