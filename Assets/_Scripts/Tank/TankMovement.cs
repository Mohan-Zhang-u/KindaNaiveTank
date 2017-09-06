using UnityEngine;
using System.Collections;

namespace Complete
{
    public class TankMovement : MonoBehaviour
    {

        public AudioSource m_MovementAudio;         // Reference to the audio source used to play engine sounds. NB: different to the shooting audio source.

		private VirtualJoyStickScript Joystick;
		private GameObject DynamicObjectLibrary;
		private TankShooting TankShootingScript;
		private TankTypeDefinition tdef;
        private Rigidbody m_Rigidbody;              // Reference used to move the tank.
        private float m_OriginalPitch;              // The pitch of the audio source at the start of the scene.
        private ParticleSystem[] m_particleSystems; // References to all the particles systems used by the Tanks
		private float m_Speed;                 // How fast the tank moves forward and back.
		private float m_TurnSpeed;            // How fast the tank turns in degrees per second.
		private AudioClip m_EngineIdling;            // Audio to play when the tank isn't moving.
		private AudioClip m_EngineDriving;           // Audio to play when the tank is moving.
		private float m_PitchRange = 0.2f;           // The amount by which the pitch of the engine noises can vary.

		private bool EnableMove = true;

		//        private string m_MovementAxisName;          // The name of the input axis for moving forward and back.
		//        private string m_TurnAxisName;              // The name of the input axis for turning.
		//        private float m_MovementInputValue;         // The current value of the movement input.
		//        private float m_TurnInputValue;             // The current value of the turn input.
	
//        private void Awake ()
//        {
//			SetDynamicObjectLibrary ();
//			SetVirtualJoyStick ();
//			OnChangeTank ();
//        }
			
        private void OnEnable ()
        {
			SetDynamicObjectLibrary ();
			SetVirtualJoyStick ();
			OnChangeTank ();
        }

        private void OnDisable ()
        {
            // When the tank is turned off, set it to kinematic so it stops moving.
			if (m_Rigidbody != null) {
				m_Rigidbody.isKinematic = true;
			}
            // Stop all particle system so it "reset" it's position to the actual one instead of thinking we moved when spawning
			if (m_particleSystems!=null && m_particleSystems.Length > 0) {
				for (int i = 0; i < m_particleSystems.Length; ++i) {
					m_particleSystems [i].Stop ();
				}
			}
        }
			
        private void Update ()
        {
            EngineAudio ();
        }
			
		private void FixedUpdate ()
		{
			if (!EnableMove) {
				return;
			}
			// Adjust the rigidbodies position and orientation in FixedUpdate.
			Move ();
			//now do the rotation
			if (Joystick.JoyStickInputVectors != Vector3.zero) { //if joystick's not pressed, look to last direction.
				transform.rotation = Quaternion.Slerp (transform.rotation, Quaternion.LookRotation (Joystick.JoyStickInputVectors), Time.deltaTime * m_TurnSpeed);
			} 
		}

        // if successfully disabled move, return.
        public bool TryDisableMove(float Disabletime)
        {
            if (!EnableMove)
            {
                return false;
            }
            else
            {
                if (gameObject.activeSelf)
                    StartCoroutine(TryDisableMoveWithTime(Disabletime));
                return true;
            }
        }

        private IEnumerator TryDisableMoveWithTime(float Disabletime)
        {
            EnableMove = false;
            Debug.Log("now DisableMove");
            yield return new WaitForSeconds(Disabletime);
            EnableMove = true;
        }

        //private void Move()
        //{
        //    // Create a vector in the direction the tank is facing with a magnitude based on the input, speed and the time between frames.
        //    float movement = Joystick.JoyStickInputVectors.magnitude * m_Speed;
        //    m_Rigidbody.velocity = transform.forward * movement;
        //}

        private void Move()
        {
            // Create a vector in the direction the tank is facing with a magnitude based on the input, speed and the time between frames.
            Vector3 movement = Joystick.JoyStickInputVectors * m_Speed * Time.deltaTime;

            // Apply this movement to the rigidbody's position.
            m_Rigidbody.MovePosition(m_Rigidbody.position + movement);
            //m_Rigidbody.velocity = Vector3.zero;
            //m_Rigidbody.angularVelocity = Vector3.zero;
        }

        private void EngineAudio ()
		{
			// If there is no input (the tank is stationary)...
			if (Mathf.Abs (Joystick.JoyStickInputVectors.magnitude) < 0.1f)
			{
				// ... and if the audio source is currently playing the driving clip...
				if (m_MovementAudio.clip == m_EngineDriving)
				{
					// ... change the clip to idling and play it.
					m_MovementAudio.clip = m_EngineIdling;
					m_MovementAudio.pitch = Random.Range (m_OriginalPitch - m_PitchRange, m_OriginalPitch + m_PitchRange);
					m_MovementAudio.Play ();
				}
			}
			else
			{
				// Otherwise if the tank is moving and if the idling clip is currently playing...
				if (m_MovementAudio.clip == m_EngineIdling)
				{
					// ... change the clip to driving and play.
					m_MovementAudio.clip = m_EngineDriving;
					m_MovementAudio.pitch = Random.Range(m_OriginalPitch - m_PitchRange, m_OriginalPitch + m_PitchRange);
					m_MovementAudio.Play();
				}
			}
		}

		private void SetDynamicObjectLibrary () {
			DynamicObjectLibrary = GameObject.Find ("DynamicObjectLibrary");
		}

		private void SetVirtualJoyStick() {
			Joystick = GameObject.Find ("JoyStickBgImg").GetComponent<VirtualJoyStickScript> ();
		}

		private void OnChangeTank() {
			TankShootingScript = GetComponent<TankShooting> ();
			tdef = TankShootingScript.GetTankDefinition ();
			m_Rigidbody = GetComponent<Rigidbody> ();
			m_Rigidbody.mass = tdef.TankMass;
			m_Rigidbody.drag = tdef.TankDrag;
			m_Rigidbody.angularDrag = tdef.TankAngularDrag;
			m_Speed = tdef.speed;
			m_TurnSpeed = tdef.rotationSpeed;
			// start()
			m_OriginalPitch = m_MovementAudio.pitch;

			// OnEnable()
			// When the tank is turned on, make sure it's not kinematic.
			m_Rigidbody.isKinematic = false;
			if (tdef.m_particleSystems.Length != 0) {
				m_particleSystems = tdef.m_particleSystems;
				for (int i = 0; i < m_particleSystems.Length; ++i) {
					m_particleSystems [i].Play ();
				}
			}

			// Load Engine Sound
			m_EngineIdling = tdef.m_EngineIdling;
			m_EngineDriving = tdef.m_EngineDriving;
			m_PitchRange = tdef.m_PitchRange;
		}
			
		public void SetEnableTankMove(bool b){
			EnableMove = b;
		}

        // if operation success, return true. amount can be positive or negative.
        public bool AddOrDecreaseSpeed(float amount)
        {
            if (amount > 0)
            {
                m_Speed += amount;
                return true;
            }
            else
            {
                if((m_Speed + amount) <= 0)
                {
                    return false;
                }
                else
                {
                    m_Speed += amount;
                    return true;
                }
            }
        }

        // increase or decrease speed by a multiplier. the upper limit is 35.
        public void MultiplySpeed(float amount)
        {
            if (amount <= 0)
                return;

            m_Speed = m_Speed * amount;

            if (m_Speed > 35)
            {
                m_Speed = 35;
                Debug.Log("<color=green>Speed reaches upper limit!</color>");
            }
            if(m_Speed < 1)
            {
                m_Speed = 1;
                Debug.Log("<color=green>Speed reaches lower limit!</color>");
            }
        }

//        private void Turn ()
//        {
//            // Determine the number of degrees to be turned based on the input, speed and time between frames.
//            float turn = m_TurnInputValue * m_TurnSpeed * Time.deltaTime;
//
//            // Make this into a rotation in the y axis.
//            Quaternion turnRotation = Quaternion.Euler (0f, turn, 0f);
//
//            // Apply this rotation to the rigidbody's rotation.
//            m_Rigidbody.MoveRotation (m_Rigidbody.rotation * turnRotation);
//        }
    }
}