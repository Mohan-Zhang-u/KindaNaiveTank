using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;

namespace Complete
{
	public class TankShooting : NetworkBehaviour
    {
		private int WallMask;
		private Collider[] WallChecker;
		[HideInInspector]
        public int _PlayerNumber = 1;              // Used to identify the different players. TODO: This shall be modified in GameManager.
		private GameObject DynamicObjectLibrary;
		public GameObject CompleteTank;
		[HideInInspector]
		public bool FireButtonOnPointerDown = false;

		// -------------------------AUDIO CUSTOM-----------------------------------------
        public AudioSource m_ShootingAudio;         // Reference to the audio source used to play the shooting audio. NB: different to the movement audio source.
		private AudioClip m_ChargingClip;            // Audio that plays when each shot is charging up.
		private AudioClip m_FireClip;                // Audio that plays when each shot is fired.
		// -------------------------AUDIO CUSTOM-----------------------------------------

		private Transform m_FireTransform;           // A child of the tank where the shells are spawned.
		[HideInInspector]
		public bool AmountLimited;  // is shell amount limited?
		private int LimitedAmount;  // how many?
		private int CurrentAmount;  // how many shells currently in the scene?
		private float ShootColdDown;
		private float FlyingSpeed;
		private bool Chargeable;
        private float m_MinLaunchForce;        // The force given to the shell if the fire button is not held.
        private float m_MaxLaunchForce;        // The force given to the shell if the fire button is held for the max charge time.
        private float m_MaxChargeTime;       // How long the shell can charge for before it is fired at max force.
		private float m_ChargeSpeed;                // How fast the launch force increases, based on the max charge time.
		private bool UpdateFireColdingDown = false;     			// use to decide whether the shoot is on colddown.

//      private string m_FireButton;                // The input axis that is used for launching shells.
        private float m_CurrentLaunchForce;         // The force that will be given to the shell when the fire button is released.
		private Slider m_AimSlider;                  // A child of the tank that displays the current launch force.
		private ShellTypeDefinition ShellDef;
		private GameObject ShellPrefab;
//		private Rigidbody ShellRigBody;

        private bool m_Fired;                       // Whether or not the shell has been launched with this button press.  

		//NowTanks
		private TankTypeDefinition tdef;  
		private GameObject SubTank; // this tank has layer "TankToSpawn"
		private TankDisplay TankDisplayScript;
		private float fireRateMultiplier;
		private WaitForSeconds ColdDownWait; // this comes from  fireRateMultiplier * ShootColdDown.

		// -------------------------now, MonoBehaviour Functions.--------------------------------
		// I dont know whether we need OnEnable or not.
//        private void OnEnable()
//        {
//			// WHY use ShellPrefab.GetComponent<ShellHandlerAbstractClass> ()? because we dont know what exact class is it, and we dont  want to use directly what in such abstractclass.
//            // When the tank is turned on, reset the launch force and the UI
//			OnChangeTankByIndex(0);
//			OnChangeShellByIndex (0);
//			SetFireTransform ();
//			SetAimSlider ();
//			
////			Debug.Log (ShellPrefab.GetComponent<ShellHandlerAbstractClass> ().MaxDamage);
//        }
			
		// was start, change to onenable.
		void Awake() {
			WallMask = LayerMask.NameToLayer ("Wall");
			SetDynamicObjectLibrary ();
			OnChangeTankByIndex(0);
			OnChangeShellByIndex (0);
			OnChangeTankOrShell ();
		}

//		void OnEnable ()
//		{
//			// The fire axis is based on the player number.
////			m_FireButton = "Fire";
//			OnChangeTankByIndex(0);
//			OnChangeShellByIndex (0);
//		}

		void Update () {
			// check whether finished colddown.
			if (UpdateFireColdingDown) {
				return;
			}
			
			// check what type of shell is going to be fired.
			if (Chargeable) {
				m_AimSlider.value = m_CurrentLaunchForce;
				// If the max force has been exceeded and the shell hasn't yet been launched...
				if (m_CurrentLaunchForce >= m_MaxLaunchForce && !m_Fired) {
					// ... use the max force and launch the shell.
					m_CurrentLaunchForce = m_MaxLaunchForce;
					Fire ();
				}
				// Otherwise, if the fire button has just started being pressed...
				else if (FireButtonOnPointerDown)
				{
					// ... reset the fired flag and reset the launch force.
					m_Fired = false;
					m_CurrentLaunchForce = m_MinLaunchForce;

					// Change the clip to the charging clip and start it playing.
					m_ShootingAudio.clip = m_ChargingClip;
					m_ShootingAudio.Play ();
				}
				// Otherwise, if the fire button is being held and the shell hasn't been launched yet...
				else if (FireButtonOnPointerDown && !m_Fired)
				{
					// Increment the launch force and update the slider.
					m_CurrentLaunchForce += m_ChargeSpeed * Time.deltaTime;

					m_AimSlider.value = m_CurrentLaunchForce;
				}
				// Otherwise, if the fire button is released and the shell hasn't been launched yet...
				else if (!FireButtonOnPointerDown && !m_Fired)
				{
					// ... launch the shell.
					Fire ();
				}
			} else {
				//not chargeable, so always shoot.
				m_AimSlider.value = 0;
				m_CurrentLaunchForce = FlyingSpeed;
				if (AmountLimited) {
					if (CurrentAmount == LimitedAmount) {
						return;
					} else {
						CurrentAmount++;
						Fire ();
					}	
				} else {
					// continuously fire.
					if (FireButtonOnPointerDown) {
						Fire ();
					}
				}
			}
		}


		//---------------------------now, Setters. -------------------------------------------
		public void SetFireButtonOnPointerDown () {
			FireButtonOnPointerDown = true;
		}

		public void SetFireButtonOnPointerUp () {
			FireButtonOnPointerDown = false;
		}

		public void SetDynamicObjectLibrary () {
			DynamicObjectLibrary = GameObject.Find ("DynamicObjectLibrary");
		}
			
		public void OnChangeTankByIndex(int index){
			tdef = DynamicObjectLibrary.GetComponent<TankLibrary>().GetTankDataForIndex(index);
			SetFunctionForOnChangeTank(tdef);
		}

		public void OnChangeTankByName(string name){
			tdef = DynamicObjectLibrary.GetComponent<TankLibrary>().GetTankDataForName(name);
			SetFunctionForOnChangeTank(tdef);

		}

		// this is the helperfunction used in OnChangeTankByIndex and OnChangeTankByName
		private void SetFunctionForOnChangeTank(TankTypeDefinition def){
			// first, destroy the old tank.
			Transform[] allchilds = CompleteTank.GetComponentsInChildren <Transform>(true);
			foreach (Transform gt in allchilds) {
//				Debug.Log (LayerMask.LayerToName (gt.gameObject.layer));
				if (LayerMask.LayerToName (gt.gameObject.layer) == "TankToSpawn") {
					Destroy(gt.gameObject);
				}
			}
			// then, initialize and add a new tank.
			SubTank = (GameObject) Instantiate(def.displayPrefab, CompleteTank.transform.position , CompleteTank.transform.rotation, CompleteTank.transform);
			TankDisplayScript = SubTank.GetComponent<TankDisplay> ();
			m_FireTransform = TankDisplayScript.DownTurrent ();
			fireRateMultiplier = tdef.fireRateMultiplier;
			ColdDownWait = new WaitForSeconds (fireRateMultiplier * ShootColdDown);

			// then, deal with audioclips.
			m_ChargingClip = tdef.m_ChargingClip;            // Audio that plays when each shot is charging up.
			m_FireClip = tdef.m_FireClip;                // Audio that plays when each shot is fired.
			// then, finally outter sets.
//			SetFireTransform ();
			SetAimSlider ();

			OnChangeTankOrShell ();
		}
			
		public void OnChangeShellByIndex(int index){
			ShellDef = DynamicObjectLibrary.GetComponent<ShellLibrary> ().GetShellDataForIndex (index);
			SetFunctionForOnChangeShell (ShellDef);

		}

		public void OnChangeShellByName(string name){
			ShellDef = DynamicObjectLibrary.GetComponent<ShellLibrary> ().GetShellDataForName (name);
			SetFunctionForOnChangeShell (ShellDef);
		}

		// this is the helperfunction used in OnChangeShellByIndex and OnChangeShellByName
		private void SetFunctionForOnChangeShell(ShellTypeDefinition def){
			m_Fired = false;
			ShellPrefab = def.displayPrefab;
//			ShellRigBody = ShellPrefab.GetComponent<Rigidbody>();
			ShootColdDown = ShellPrefab.GetComponent<ShellHandlerAbstractClass> ().ShootColdDown;
			Chargeable = ShellPrefab.GetComponent<ShellHandlerAbstractClass> ().ForceChargeable;
			if (Chargeable) {
				m_CurrentLaunchForce = ShellPrefab.GetComponent<ShellHandlerAbstractClass> ().MinShootForce;
				m_AimSlider.value = m_CurrentLaunchForce;
				m_MinLaunchForce = ShellPrefab.GetComponent<ShellHandlerAbstractClass> ().MinShootForce;
				m_MaxLaunchForce = ShellPrefab.GetComponent<ShellHandlerAbstractClass> ().MaxShootForce;
				m_MaxChargeTime = ShellPrefab.GetComponent<ShellHandlerAbstractClass> ().m_MaxChargeTime;
				m_ChargeSpeed = (m_MaxLaunchForce - m_MinLaunchForce) / m_MaxChargeTime;

			} else {
				FlyingSpeed = ShellPrefab.GetComponent<ShellHandlerAbstractClass> ().FlyingSpeed;
				m_CurrentLaunchForce = FlyingSpeed;
				m_AimSlider.value = 0;
				m_MinLaunchForce = 0;
				m_MaxLaunchForce = 0;
				m_MaxChargeTime = 0;
				m_ChargeSpeed = 0;
			}
			AmountLimited = ShellPrefab.GetComponent<ShellHandlerAbstractClass> ().AmountLimited;
			LimitedAmount = ShellPrefab.GetComponent<ShellHandlerAbstractClass> ().LimitedAmount;

			DoAnimationChangeAndSetFiretransform ();

			OnChangeTankOrShell ();
		}

		// this function is in need, because ColdDownWait can only be valuable if tank and shell both loaded.
		// notice! the function is also called in both tank and shell loaded.
		private void OnChangeTankOrShell(){
			ColdDownWait = new WaitForSeconds (fireRateMultiplier * ShootColdDown);
		}
			
		// play animation if needed, set firetransform in TankDisplay and return it here.
		private void DoAnimationChangeAndSetFiretransform (){
			if (ShellDef.NeedTurrentUp){
				m_FireTransform = TankDisplayScript.RiseTurrent();
			}
			else{
				m_FireTransform = TankDisplayScript.DownTurrent();
			}
		}

		// TODO: implement it!!!! typically used in lasersword
		public void RespawnedShellExploded(){
			if (AmountLimited) {
				// only a limited amount of shells can be fire at the same time.
				if (CurrentAmount > 0) {
					CurrentAmount -= 1;
				}
			}
		}
			
		private void SetFireTransform (){
			m_FireTransform = TankDisplayScript.GetFireTransform ();
			if (m_FireTransform == null)
				Debug.Log ("<color = red>m_FireTransform not found </color>");
		}

		private void SetAimSlider (){
			Slider[] Sliders = transform.GetComponentsInChildren<Slider> (true);
			foreach (Slider s in Sliders) {
				if (s.name == "AimSlider")
					m_AimSlider = s;
			}
		}

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
			// solve the CROSS-WALL bug.
			WallChecker = Physics.OverlapSphere (m_FireTransform.position, 0.55f);
			if (WallChecker.Length > 0) {
				foreach(Collider wobj in WallChecker){
					if (wobj.gameObject.layer == WallMask) {
						return;
					}
				}
			}

			// Create an instance of the shell and store a reference to it's rigidbody.
			GameObject shellInstance = Instantiate (ShellPrefab, m_FireTransform.position, m_FireTransform.rotation) as GameObject;
			// set ShellDisplay.TankShootingScript;
			shellInstance.GetComponent<ShellDisplay> ().TankShootingScript = this;

			// ------------now the only two var that is not initialized in ShellHandlerAbstractClass
			shellInstance.GetComponent<ShellHandlerAbstractClass> ().FireByTankId = _PlayerNumber;
			// TODO: set ExplosionId
			shellInstance.GetComponent<ShellHandlerAbstractClass> ().ExplosionId = "";
					
			if (Chargeable) {
				m_Fired = true;
				// Set the shell's velocity to the launch force in the fire position's forward direction.
				shellInstance.GetComponent<Rigidbody> ().velocity = m_CurrentLaunchForce * m_FireTransform.forward;
				// shellInstance.velocity = m_CurrentLaunchForce * m_FireTransform.forward; 

				// Reset the launch force.  This is a precaution in case of missing button events.
				m_CurrentLaunchForce = m_MinLaunchForce;
			} else {

				shellInstance.GetComponent<Rigidbody> ().velocity = FlyingSpeed * m_FireTransform.forward;

				if (AmountLimited) {

				} else {

				}
			}

            // Change the clip to the firing clip and play it.
            m_ShootingAudio.clip = m_FireClip;
            m_ShootingAudio.Play ();

            
			if (fireRateMultiplier > 0.00001f) {
				StartCoroutine (PerformColdDown ());
			}
			//now, proceed the colddown.
        }

		private IEnumerator PerformColdDown(){
			UpdateFireColdingDown = true;
			yield return ColdDownWait;
			UpdateFireColdingDown = false;
		}

			public TankTypeDefinition GetTankDefinition () {
				return tdef;
			}

    }

	// ----------------------------------getters------------------------------------
//	public TankTypeDefinition GetTankDefinition () {
//		return tdef;
//	}
}