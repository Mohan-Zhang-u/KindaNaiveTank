/// TODO: RPC and INIT,  change STARTING HEALTH AND ARMOR accordingly.!!!!!!!!!!!!!!!!!!!!!!!!!!!!
/// set SetHealthAndShieldUI!!!!!!!!!!!!!!!!
/// Death, not finished, deal with damage source!!!!!!!!!!!!!!!!
/// SetDamagedBy still not finished.
/// SetDestoryedBy still not finished.
/// SetTankActive 
/// a lot of tankmanager thing... TankManager
/// From notepad.
/// !!!!!!!!!!!All tank's max shieldlevel is 100f.


using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.Networking;
using System.Collections.Generic;
using System.Collections;

public struct DamageSource {
	float amount; int playerNumber; string explosionId;

	public DamageSource(float p1, int p2, string p3){
		amount = p1;
		playerNumber = p2;
		explosionId = p3;
	}
}

namespace Complete
{
	public class TankHealth : NetworkBehaviour
    {
		private GameObject DynamicObjectLibrary;
		private TankShooting TankShootingScript;
		public GameObject CompleteTank;
		private TankTypeDefinition tdef;

        private float StartingHealth = 100f;               // The amount of health each tank starts with.
        public Color FullHealthColor = Color.green;       // The color the health bar will be when on full health.
        public Color ZeroHealthColor = Color.red;         // The color the health bar will be when on no health.
        
		private GameObject ExplosionPrefab;                // A prefab that will be instantiated in Awake, then used whenever the tank dies.
        private AudioSource ExplosionAudio;               // The audio source to play when the tank explodes.
        private ParticleSystem ExplosionParticles;        // The particle system the will play when the tank is destroyed.
        [SyncVar(hook = "OnCurrentHealthChanged")]
        private float CurrentHealth;                      // How much health the tank currently has.
		[SyncVar(hook = "OnShieldLevelChanged")]
		private float ShieldLevel;						//The current shield level of the tank.
		[SyncVar]
		private bool ZeroHealthHappened;                                // Has the tank been reduced beyond zero health yet? same as private bool !!!!ZeroHealthHappened;

		private Slider HealthSlider;                             // The slider to represent how much health the tank currently has.
        private Slider ShieldSlider;
        private Image HealthFillImage;                           // The image component of the slider.
        private Image ShieldFillImage;
        private TankDisplay TDS;
		// Used so that the tank doesn't collide with anything when it's dead.
		private BoxCollider Collider;
		//Internal reference to the spawn point where this tank is.
		//private SpawnPoint CurrentSpawnPoint;
		private List<DamageSource> DamageSourceList;
		// Events that fire when specific conditions are reached. Mainly used for the HUD to tie into.

		//Field to set the tank as invulnerable. Mainly used in the shooting range.
		private bool invulnerable;
		// !!!!!!!!! idk why but it uses DIVISION!!!!!!!!!!!
		public event Action<float> healthChanged;
		public event Action<float> shieldChanged;
		public event Action playerDeath;
		public event Action playerReset;
		//This constant defines the player index used to represent player suicide in the damage parsing system.
		public const int TANK_SUICIDE_INDEX = -1;
        //This constant defines the player index used to represent player damaged by the nutural environment in the damage parsing system.
        public const int TANK_ENVIRONMNETDMG_INDEX = -2;

/*	TODO: syncVar used for NetWorkBehavior.s
		[SyncVar(hook = "OnCurrentHealthChanged")]
		How much health the tank currently has.*
		private float CurrentHealth;

		[SyncVar(hook = "OnShieldLevelChanged")]
		The current shield level of the tank.
		private float ShieldLevel;
		[SyncVar]
 		the same functionality used for ExplosionPrefab.
		The parameters for the explosion to be spawned on tank death.
		[SerializeField]
		protected ExplosionSettings DeathExplosion;
		!!!!Now from tanks!!Ref!!!!!!!!!!
*/

//		private void Awake (){
//			SetDynamicObjectLibrary ();
//			DamageSourceList = new List<DamageSource> ();
//		}

		private void OnEnable()
		{
			SetDynamicObjectLibrary ();
			DamageSourceList = new List<DamageSource> ();
			// setDefaults calls OnChangeTank;
			OnChangeTank();
//			CurrentHealth = StartingHealth;
//			ShieldLevel = 0f;
//			TDS.SetShieldBubbleActive(false);
//			ZeroHealthHappened = false;
//			// Update the health slider's value and color.
//			SetHealthAndShieldUI();
//			//i dont think its nesessary, tho.
//			SetTankActive(true);
		}

//		private void Update(){
//			Debug.Log (TDS.transform.position);
//		}

		// This function is called at the start of each round to make sure each tank is set up correctly.
		public void SetDefaults()
		{
			CurrentHealth = StartingHealth;
			ShieldLevel = 0f;
			TDS.SetShieldBubbleActive(false);
			ZeroHealthHappened = false;
			SetHealthAndShieldUI();
			SetTankActive(true);
			if (playerReset != null)
			{
				playerReset();
			}

		}

		// sets Slider HealthFillImage TDS
		private void OnChangeTank(){
			Slider[] Sliders = transform.GetComponentsInChildren<Slider> (true);
			foreach (Slider s in Sliders) {
				if (s.name == "HealthSlider")
                    HealthSlider = s;
                if (s.name == "ShieldSlider")
                    ShieldSlider = s;

            }
			Image[] Images = HealthSlider.GetComponentsInChildren<Image> (true);
			foreach (Image m in Images) {
				if (m.name == "HealthFill")
					HealthFillImage = m;

			}
            Images = ShieldSlider.GetComponentsInChildren<Image>(true);
            foreach (Image m in Images)
            {
                if (m.name == "ShieldFill")
                    ShieldFillImage = m;

            }
            TDS = CompleteTank.GetComponentInChildren<TankDisplay> (true);
			if (HealthSlider == null)
				Debug.Log ("<color = red>HealthSlider init error</color>");
            if (ShieldSlider == null)
                Debug.Log("<color = red>ShieldSlider init error</color>");
            if (HealthFillImage == null)
				Debug.Log ("<color = red>HealthFillImage init error</color>");
            if (ShieldFillImage == null)
                Debug.Log("<color = red>ShieldFillImage init error</color>");
            if (TDS == null)
				Debug.Log ("<color = red>TDS init error</color>");
            TankShootingScript = GetComponent<TankShooting> ();
			tdef = TankShootingScript.GetTankDefinition ();
			ExplosionPrefab = tdef.TankExplosionPrefab;
			StartingHealth = tdef.StartHealth;
            // now, set HealthSlider's max and min value
            HealthSlider.maxValue = StartingHealth;
            // !!!!! can only be set like this because after explosion, This tank wont exist.
            ExplosionParticles = Instantiate (ExplosionPrefab).GetComponent<ParticleSystem> ();
			// Get a reference to the audio source on the instantiated prefab.
			ExplosionAudio = ExplosionParticles.GetComponent<AudioSource> ();
			// Disable the prefab so it can be activated when it's required.
			ExplosionParticles.gameObject.SetActive (false);

			// reset tank to default state.
			SetDefaults ();
		}

		//Implementation for IDamageObject
		public bool isAlive { get { return CurrentHealth > 0; } } 

		//public SpawnPoint currentSpawnPoint
		//{
		//	get { return CurrentSpawnPoint; }
		//	set { CurrentSpawnPoint = value; }
		//}

		private void SetDynamicObjectLibrary () {
			DynamicObjectLibrary = GameObject.Find ("DynamicObjectLibrary");
		}

		//public void NullifySpawnPoint(SpawnPoint point)
		//{
  //          Debug.Log("<color = red> shall never use this! </color>");
		//	//Make sure we don't nullify a point if the currentPoint has changed
		//	if (CurrentSpawnPoint == point)
		//	{
		//		CurrentSpawnPoint = null;
		//	}
		//}


		private int _PlayerNumber = -1;
		public int PlayerNumber
		{
			get
			{
				return _PlayerNumber;
			}
			set{
				_PlayerNumber = value;
			}
		}

		//Returns the last Damage deal to this tank.
		public DamageSource lastDamage
		{
			get
			{
				if (DamageSourceList.Count == 0)
					return new DamageSource ( 0, -2, "" );
				else
					return DamageSourceList [DamageSourceList.Count - 1];
			}
		}
			
		public Vector3 GetPosition()
		{
			return transform.position;
		}





		// Awake should be reserved for loading tank by TankShooting. Shall be move to OnEnable.
//		public void SetDynamicObjectLibrary () {
//			DynamicObjectLibrary = GameObject.Find ("DynamicObjectLibrary");
//		}

		//AudioSource ParticleSystem Collider CurrentSpawnPoint

		public bool IsPlayerDead()
		{
			return ZeroHealthHappened;
		}

		public void TakeDamage(float amount){
			Debug.Log ("shouldn't be used!!!!!!!!!");
		}

		// This is called whenever the tank takes damage. Implements IDamageObject. !!!!!!!!!also called Damage
		// This can be also use to heal, right?
		public void Damage(float amount, int playerNumber, string explosionId)
		{
			if (invulnerable)
			{
				return;
			}

				if (amount < 0.01f && amount > -0.01f) {
				return;
			}

			SetDamagedBy (amount, playerNumber, explosionId);

//			RpcDamageFlash(LastDamagedByPlayerNumber); 

			//If we have shields, ensure that these are reduced before applying damage to the tank's main health.
			if (ShieldLevel > 0)
			{
				ShieldLevel -= amount;

				//If shields have dropped below zero, transfer the balance of the damage to the tank's main health.
				if (ShieldLevel <= 0)
				{
					amount = Mathf.Abs(ShieldLevel);
					ShieldLevel = 0;
				}
				else
				{
					amount = 0;
				}
			}

			// Reduce current health by the amount of damage done.
			CurrentHealth -= amount;

			// Change the UI elements appropriately.
			SetHealthAndShieldUI ();

			// If the current health is at or below zero and it has not yet been registered, call OnZeroHealth.

			if (CurrentHealth <= 0f && !ZeroHealthHappened)
			{
				// Set the flag so that this function is only called once.
				ZeroHealthHappened = true;
				OnZeroHealth();
			}
		}
			
		private void OnDeath ()
		{
			Debug.Log ("shouldn't be used!!!!!!!!!");
		}

		//Fires when health reaches zero on the server.
		private void OnZeroHealth()
		{
			RpcOnZeroHealth ();

			//TODO: what the fuck is this?
//			if (isServer)
//			{
//				GameManager.s_Instance.rulesProcessor.TankDies(Manager);
//			} 
		}

		//Assigns internal damage variables from explosion.
		public void SetDamagedBy(float amount, int playerNumber, string explosionId)
		{
			//If we've received the tank suicide index, replace it with this tank's player index to count it as a suicide.
			if (playerNumber == TANK_SUICIDE_INDEX)
			{
//				playerNumber = Manager.playerNumber;
			}

			DamageSourceList.Add (new DamageSource(amount, playerNumber, explosionId));
		}

		//Assigns internal damage variables from explosion.
		public void SetDestroyedBy(int playerNumber, string explosionId)
		{
			// TODO: finish this part in TankManager.
			//If we've received the tank suicide index, replace it with this tank's player index to count it as a suicide.
			if (playerNumber == TANK_SUICIDE_INDEX) {
				//				playerNumber = Manager.playerNumber;
			} else if (playerNumber == TANK_ENVIRONMNETDMG_INDEX) {
				//				playerNumber = Manager.playerNumber;
			}

			Debug.LogFormat("Destroyed by playerNumber = {0}", playerNumber);
			//TODO: finish this with TankManager.
		}
			
		private void SetTankActive(bool active)
		{
			if (Collider == null && TDS != null)
			{
				Collider = TDS.GetComponent<BoxCollider>();
			}
			if (Collider != null)
			{
				Collider.enabled = active;
			}
			TDS.SetVisibleObjectsActive(active);

			Debug.Log ("Related to TANKMANAGER HERE!");

//TODO:  collaborate with tankmanager.
//			if (active)
//			{
//				Manager.EnableControl();
//			}
//			else
//			{
//				Manager.DisableControl();
//			}
		}


		private void SetWholeTankThingsInActive(){
			CompleteTank.SetActive(false);
		}

		//Sets the shield level to a given value. Called by the shield powerup object to enable shields.
		public void SetShieldLevel(float value)
		{
			ShieldLevel = value;
            SetHealthAndShieldUI();

        }

        private void SetHealthAndShieldUI()
        {
            if (ShieldLevel > 0)
            {
                TDS.SetShieldBubbleActive(true);
            }
			if (ShieldLevel <= 0) {
				TDS.SetShieldBubbleActive (false);
                ShieldSlider.value = 0f;
            }

            ShieldSlider.value = ShieldLevel;
            // Set the slider's value appropriately.
            HealthSlider.value = CurrentHealth;

            // Interpolate the color of the bar between the choosen colours based on the current percentage of the starting health.
            HealthFillImage.color = Color.Lerp (ZeroHealthColor, FullHealthColor, CurrentHealth / StartingHealth);

        }

		//Hooked into the currenthealth syncvar. Updates whenever health changes server-side.
		void OnCurrentHealthChanged(float value)
		{
			CurrentHealth = value;

			if (healthChanged != null)
			{
				healthChanged(CurrentHealth / StartingHealth);
			}
		}

		//Hooked into the shield level syncvar. Updates whenever shield level changes server-side.
		void OnShieldLevelChanged(float value)
		{
			ShieldLevel = value;

			TDS.SetShieldBubbleActive(ShieldLevel > 0);

			if (shieldChanged != null)
			{
				shieldChanged(ShieldLevel / StartingHealth);
			}
		}

        public void AddHealth(float amount,int PlayerNumber)
        {
            if (CurrentHealth <= 0)
            {
                return;
            }
            float healthexpected = CurrentHealth + amount;
            if (healthexpected >= StartingHealth)
            {
                CurrentHealth = StartingHealth;
            }
            else
            {
                CurrentHealth = healthexpected;
            }
            SetHealthAndShieldUI();
            if (healthChanged != null)
            {
                healthChanged(CurrentHealth / StartingHealth);
            }
        }

        public void AddShield(float amount, int PlayerNumber)
        {
            if (CurrentHealth <= 0)
            {
                return;
            }
            float ShieldExpected = ShieldLevel + amount;
            if (ShieldExpected >= 100f)
            {
                ShieldLevel = 100f;
            }
            else
            {
                ShieldLevel = ShieldExpected;
            }
            SetHealthAndShieldUI();
            if (shieldChanged != null)
            {
                shieldChanged(ShieldLevel / 100f);
            }
        }

        // return false if InvincibleForSeconds not started
        public bool SetInvincibleForSeconds(bool b, float t)
        {
            if (invulnerable)
            {
                return false;
            }
            else if (!b)
            {
                invulnerable = false;
                return false;
            }
            else
            {
                StartCoroutine(InvincibleForSeconds(t));
                return true;
            }
        }

        private IEnumerator InvincibleForSeconds(float t)
        {
            invulnerable = true;
            yield return new WaitForSeconds(t);
            invulnerable = false;
        }

        //  TODO:!!!!!!!!!!!!!!!!Now all RPC things.!!!!!!!!!!!!!!!!
        //		//Initializes all required references to external scripts.
        //		public void Init(TankManager manager)
        //		{
        //			Manager = manager;
        //			TDS = manager.display;
        //			StartingHealth = manager.playerTankType.hitPoints;
        //			Collider = TDS.GetComponent<BoxCollider>();
        //		}
        //
        //		[ClientRpc]
        //		public void RpcDelayedReset()
        //		{
        //			Manager.Reset(null);
        //		}
        //
        //		[ClientRpc]
        //		//Fired on clients to make tanks damaged by the local player flash red.
        //		private void RpcDamageFlash(int sourcePlayer)
        //		{
        //			if (sourcePlayer == GameManager.s_Instance.GetLocalPlayerId())
        //			{
        //				TDS.StartDamageFlash();
        //			}
        //		}
        //

        //		//Initializes all required references to external scripts.
        //		public void Init(TankManager manager)
        //		{
        //			Manager = manager;
        //			TDS = manager.display;
        //			StartingHealth = manager.playerTankType.hitPoints;
        //			Collider = TDS.GetComponent<BoxCollider>();
        //		}


        //		[ClientRpc]
        private void RpcOnZeroHealth()
		{
			//-----------------------------------original-------------------------
			// Move the instantiated explosion prefab to the tank's position and turn it on.
			ExplosionParticles.transform.position = transform.position;
			ExplosionParticles.gameObject.SetActive (true);

			// Play the particle system of the tank exploding.
			ExplosionParticles.Play ();

			// Play the tank explosion sound effect.
			ExplosionAudio.Play();
			// Destroy explosionParticle System and handle the freaking TankDisplay.cs thing.
			ParticleSystem.MainModule mainModule = ExplosionParticles.main;
			Destroy (ExplosionParticles.gameObject, mainModule.duration);

//			// Turn the tank off.
//			gameObject.SetActive (false);
			// we are gonna do above in TankDisplay.cs
			//-----------------------------------original-------------------------
			// Break off our decorations
//			TDS.DetachDecorations();

//			if (ExplosionManager.s_InstanceExists && DeathExplosion != null)
//			{
//				ExplosionManager.s_Instance.SpawnExplosion(transform.position, Vector3.up, gameObject, Manager.playerNumber, DeathExplosion, false);
//			}
//
			InternalOnZeroHealth();
		}

		//Fired on clients via RPC to perform death cleanup.
		private void InternalOnZeroHealth()
		{
			//Disable any active powerup SFX
			ShieldLevel = 0f;
			TDS.SetShieldBubbleActive(false);
//			TankDisplay.SetNitroParticlesActive(false);

			// Disable the collider and all the appropriate child gameobjects so the tank doesn't interact or show up when it's dead.
			SetTankActive(false);

			SetWholeTankThingsInActive ();

			//if (CurrentSpawnPoint != null)
			//{
			//	CurrentSpawnPoint.Decrement();
			//	// TODO: implement this!!!
			//	// finish this with TankManager.
			//}

			if (playerDeath != null)
			{
				playerDeath();
				// TODO: use this in TankManager!
			}
		}

    }
}