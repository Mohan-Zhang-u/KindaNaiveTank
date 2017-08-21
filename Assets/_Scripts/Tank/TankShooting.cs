using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

namespace Complete
{
	public class TankShooting : NetworkBehaviour
    {
//        public int m_PlayerNumber = 1;              // Used to identify the different players.
		public GameObject DynamicObjectLibrary;
		public Button FireButton;
        public AudioSource m_ShootingAudio;         // Reference to the audio source used to play the shooting audio. NB: different to the movement audio source.
        public AudioClip m_ChargingClip;            // Audio that plays when each shot is charging up.
        public AudioClip m_FireClip;                // Audio that plays when each shot is fired.
		public Transform m_FireTransform;           // A child of the tank where the shells are spawned.

		public bool AmountLimited;  // is shell amount limited?
		private int LimitedAmount;  // how many?
		private int CurrentAmount;  // how many shells currently in the scene?
		private bool Chargeable;
        private float m_MinLaunchForce;        // The force given to the shell if the fire button is not held.
        private float m_MaxLaunchForce;        // The force given to the shell if the fire button is held for the max charge time.
        private float m_MaxChargeTime;       // How long the shell can charge for before it is fired at max force.
		private float m_ChargeSpeed;                // How fast the launch force increases, based on the max charge time.

        private string m_FireButton;                // The input axis that is used for launching shells.
        private float m_CurrentLaunchForce;         // The force that will be given to the shell when the fire button is released.
		private Slider m_AimSlider;                  // A child of the tank that displays the current launch force.
		private ShellTypeDefinition ShellDef;
		private GameObject ShellPrefab;
		private Rigidbody ShellRigBody;
        private bool m_Fired;                       // Whether or not the shell has been launched with this button press.  

 

        private void OnEnable()
        {
			// WHY use ShellPrefab.GetComponent<ShellHandlerAbstractClass> ()? because we dont know what exact class is it, and we dont  want to use directly what in such abstractclass.
            // When the tank is turned on, reset the launch force and the UI

			LazySetFireTransform ();
			LazySetAimSlider ();
			OnChangeShellByIndex (0);
			
//			Debug.Log (ShellPrefab.GetComponent<ShellHandlerAbstractClass> ().MaxDamage);
        }

		private void LazySetAimSlider (){
			if (m_AimSlider == null) {
				Slider[] Sliders = transform.GetComponentsInChildren<Slider> (true);
				foreach (Slider s in Sliders) {
					if (s.name == "AimSlider")
						m_AimSlider = s;
				}
			}
		}

		// this is the helperfunction used in OnChangeShellByIndex and OnChangeShellByName
		private void SetFunctionForOnChangeShell(ShellTypeDefinition def){
			m_Fired = false;
			ShellPrefab = def.displayPrefab;
			ShellRigBody = ShellPrefab.GetComponent<Rigidbody>();
			Chargeable = ShellPrefab.GetComponent<ShellHandlerAbstractClass> ().ForceChargeable;
			if (Chargeable) {
				m_CurrentLaunchForce = ShellPrefab.GetComponent<ShellHandlerAbstractClass> ().MinShootForce;
				m_AimSlider.value = m_CurrentLaunchForce;
				m_MinLaunchForce = ShellPrefab.GetComponent<ShellHandlerAbstractClass> ().MinShootForce;
				m_MaxLaunchForce = ShellPrefab.GetComponent<ShellHandlerAbstractClass> ().MaxShootForce;
				m_MaxChargeTime = ShellPrefab.GetComponent<ShellHandlerAbstractClass> ().m_MaxChargeTime;
				m_ChargeSpeed = (m_MaxLaunchForce - m_MinLaunchForce) / m_MaxChargeTime;

			} else {
				m_CurrentLaunchForce = ShellPrefab.GetComponent<ShellHandlerAbstractClass> ().FlyingSpeed;
				m_AimSlider.value = 0;
				m_MinLaunchForce = 0;
				m_MaxLaunchForce = 0;
				m_MaxChargeTime = 0;
				m_ChargeSpeed = 0;
			}
			AmountLimited = ShellPrefab.GetComponent<ShellHandlerAbstractClass> ().AmountLimited;
			LimitedAmount = ShellPrefab.GetComponent<ShellHandlerAbstractClass> ().LimitedAmount;
		}

		private void OnChangeShellByIndex(int index){
			ShellDef = DynamicObjectLibrary.GetComponent<ShellLibrary> ().GetShellDataForIndex (index);
			SetFunctionForOnChangeShell (ShellDef);

		}

		private void OnChangeShellByName(string name){
			ShellDef = DynamicObjectLibrary.GetComponent<ShellLibrary> ().GetShellDataForName (name);
			SetFunctionForOnChangeShell (ShellDef);
		}

		// TODO: implement it!!!! typically used in lasersword
		public void RespawnedShellExploded(string ShellId){

		}

        private void Start ()
        {
            // The fire axis is based on the player number.
            m_FireButton = "Fire";
			LazySetFireTransform ();
			LazySetAimSlider ();
			OnChangeShellByIndex (0);
        }

		private void LazySetFireTransform (){
			if (m_FireTransform != null) {
				return;
			}
			m_FireTransform = MyLibrary.FindInDescendantByName (transform, "FireTransform");
			if (m_FireTransform == null)
				Debug.Log ("<color = red>m_FireTransform not found </color>");
		}


//		private void Update () {
//			if (Chargeable) {
//				m_AimSlider.value = m_CurrentLaunchForce;
//				if (m_CurrentLaunchForce >= m_MaxLaunchForce && !m_Fired) {
//					// ... use the max force and launch the shell.
//					m_CurrentLaunchForce = m_MaxLaunchForce;
//					Fire ();
//				}
//
//			} else {
//				//not chargeable, so always shoot.
//				if (AmountLimited) {
//					// only a limited amount of shells can be fire at the same time.
//				} else {
//
//				}
//			}
//		}
//
//        private void Update ()
//        {
//            // The slider should have a default value of the minimum launch force.
//            m_AimSlider.value = m_MinLaunchForce;
//
//            // If the max force has been exceeded and the shell hasn't yet been launched...
//            if (m_CurrentLaunchForce >= m_MaxLaunchForce && !m_Fired)
//            {
//                // ... use the max force and launch the shell.
//                m_CurrentLaunchForce = m_MaxLaunchForce;
//                Fire ();
//            }
//            // Otherwise, if the fire button has just started being pressed...
//            else if (Input.GetButtonDown (m_FireButton))
//            {
//                // ... reset the fired flag and reset the launch force.
//                m_Fired = false;
//                m_CurrentLaunchForce = m_MinLaunchForce;
//
//                // Change the clip to the charging clip and start it playing.
//                m_ShootingAudio.clip = m_ChargingClip;
//                m_ShootingAudio.Play ();
//            }
//            // Otherwise, if the fire button is being held and the shell hasn't been launched yet...
//            else if (Input.GetButton (m_FireButton) && !m_Fired)
//            {
//                // Increment the launch force and update the slider.
//                m_CurrentLaunchForce += m_ChargeSpeed * Time.deltaTime;
//
//                m_AimSlider.value = m_CurrentLaunchForce;
//            }
//            // Otherwise, if the fire button is released and the shell hasn't been launched yet...
//            else if (Input.GetButtonUp (m_FireButton) && !m_Fired)
//            {
//                // ... launch the shell.
//                Fire ();
//            }
//        }


        private void Fire ()
        {
            // Set the fired flag so only Fire is only called once.
            m_Fired = true;

            // Create an instance of the shell and store a reference to it's rigidbody.
            Rigidbody shellInstance =
                Instantiate (ShellRigBody, m_FireTransform.position, m_FireTransform.rotation) as Rigidbody;

            // Set the shell's velocity to the launch force in the fire position's forward direction.
            shellInstance.velocity = m_CurrentLaunchForce * m_FireTransform.forward; 

            // Change the clip to the firing clip and play it.
            m_ShootingAudio.clip = m_FireClip;
            m_ShootingAudio.Play ();

            // Reset the launch force.  This is a precaution in case of missing button events.
            m_CurrentLaunchForce = m_MinLaunchForce;
        }
			
    }
}