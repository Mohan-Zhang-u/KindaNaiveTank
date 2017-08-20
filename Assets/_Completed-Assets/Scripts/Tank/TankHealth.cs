//TODO: RPC and INIT,  change STARTING HEALTH AND ARMOR accordingly.!!!!!!!!!!!!!!!!!!!!!!!!!!!!
// set SetHealthAndShieldUI!!!!!!!!!!!!!!!!
// SetDamagedBy still not finished.
// SetTankActive 
// a lot of tankmanager thing... TankManager
// From notepad.


using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.Networking;

namespace Complete
{
	public class TankHealth : NetworkBehaviour
    {
        public float m_StartingHealth = 100f;               // The amount of health each tank starts with.
        public Color m_FullHealthColor = Color.green;       // The color the health bar will be when on full health.
        public Color m_ZeroHealthColor = Color.red;         // The color the health bar will be when on no health.
        public GameObject m_ExplosionPrefab;                // A prefab that will be instantiated in Awake, then used whenever the tank dies.
        
        
        private AudioSource m_ExplosionAudio;               // The audio source to play when the tank explodes.
        private ParticleSystem m_ExplosionParticles;        // The particle system the will play when the tank is destroyed.
		[SyncVar(hook = "OnCurrentHealthChanged")]
        private float m_CurrentHealth;                      // How much health the tank currently has.
		[SyncVar(hook = "OnShieldLevelChanged")]
		private float m_ShieldLevel;						//The current shield level of the tank.
		[SyncVar]
		private bool m_ZeroHealthHappened;                                // Has the tank been reduced beyond zero health yet? same as private bool !!!!m_ZeroHealthHappened;

		private Slider m_Slider;                             // The slider to represent how much health the tank currently has.
		private Image m_FillImage;                           // The image component of the slider.
		private TankDisplay m_TankDisplay;
		// Used so that the tank doesn't collide with anything when it's dead.
		private BoxCollider m_Collider;
		//Internal reference to the spawn point where this tank is.
		private SpawnPoint m_CurrentSpawnPoint;
		//TODO: syncVar used for NetWorkBehavior.s
//		[SyncVar(hook = "OnCurrentHealthChanged")]
//		// How much health the tank currently has.*
//		private float m_CurrentHealth;
//
//		[SyncVar(hook = "OnShieldLevelChanged")]
//		//The current shield level of the tank.
//		private float m_ShieldLevel;
//		[SyncVar]




		//!!!!Now from tanks!!Ref!!!!!!!!!!
		// the same functionality used for m_ExplosionPrefab.
		//The parameters for the explosion to be spawned on tank death.
//		[SerializeField]
//		protected ExplosionSettings m_DeathExplosion;

//		//Implementation for IDamageObject
		public bool isAlive { get { return m_CurrentHealth > 0; } } 

		//Field to set the tank as invulnerable. Mainly used in the shooting range.
		public bool invulnerable
		{
			get;
			set;
		}
	


		//Events that fire when specific conditions are reached. Mainly used for the HUD to tie into.
		public event Action<float> healthChanged;
		public event Action<float> shieldChanged;
		public event Action playerDeath;
		public event Action playerReset;
		//This constant defines the player index used to represent player suicide in the damage parsing system.
		public const int TANK_SUICIDE_INDEX = -1;

	
		public SpawnPoint currentSpawnPoint
		{
			get { return m_CurrentSpawnPoint; }
			set { m_CurrentSpawnPoint = value; }
		}


		public void NullifySpawnPoint(SpawnPoint point)
		{
			//Make sure we don't nullify a point if the currentPoint has changed
			if (m_CurrentSpawnPoint == point)
			{
				m_CurrentSpawnPoint = null;
			}
		}


		//Field that stores the index of the last player to do damage to this tank.
		private int m_LastDamagedByPlayerNumber = -1;
		public int lastDamagedByPlayerNumber
		{
			get
			{
				return m_LastDamagedByPlayerNumber;
			}
		}


		//Field that stores the last explosion ID to damage this tank. Used for analytics purposes.
		private string m_LastDamagedByExplosionId;
		public string lastDamagedByExplosionId
		{
			get
			{
				return m_LastDamagedByExplosionId;
			}
		}


		public Vector3 GetPosition()
		{
			return transform.position;
		}






        private void Awake ()
        {
            // Instantiate the explosion prefab and get a reference to the particle system on it.
			LazySetVariablesUp();
            m_ExplosionParticles = Instantiate (m_ExplosionPrefab).GetComponent<ParticleSystem> ();

            // Get a reference to the audio source on the instantiated prefab.
            m_ExplosionAudio = m_ExplosionParticles.GetComponent<AudioSource> ();

            // Disable the prefab so it can be activated when it's required.
            m_ExplosionParticles.gameObject.SetActive (false);
        }


        private void OnEnable()
        {
            // When the tank is enabled, reset the tank's health and whether or not it's dead.
			LazySetVariablesUp();
            m_CurrentHealth = m_StartingHealth;
			m_ShieldLevel = 0f;
			m_TankDisplay.SetShieldBubbleActive(false);
            m_ZeroHealthHappened = false;

            // Update the health slider's value and color.
            SetHealthAndShieldUI();

			//i dont think its nesessary, tho.
			SetTankActive(true);
        }

		private void LazySetVariablesUp(){
			if (m_Slider == null) {
				Slider[] Sliders = transform.GetComponentsInChildren<Slider> (true);
				foreach (Slider s in Sliders) {
					if (s.name == "HealthSlider")
						m_Slider = s;
				}
			}
			if (m_FillImage == null) {
				Image[] Images = m_Slider.GetComponentsInChildren<Image> (true);
				foreach (Image m in Images) {
					if (m.name == "Fill")
						m_FillImage = m;
				}
			}
			if (m_TankDisplay == null)
				m_TankDisplay = transform.GetComponentInChildren<TankDisplay> (true);
			if (m_Slider == null)
				Debug.Log ("<color = red>HealthSlider init error</color>");
//			else
//				Debug.Log (m_Slider.name);
			if (m_FillImage == null)
				Debug.Log ("<color = red>m_FillImage init error</color>");
			if (m_TankDisplay == null)
				Debug.Log ("<color = red>m_TankDisplay init error</color>");


		}

		// This function is called at the start of each round to make sure each tank is set up correctly.
		public void SetDefaults()
		{
			m_CurrentHealth = m_StartingHealth;
			m_ShieldLevel = 0f;
			m_TankDisplay.SetShieldBubbleActive(false);
			m_ZeroHealthHappened = false;
			SetHealthAndShieldUI();
			SetTankActive(true);

			if (playerReset != null)
			{
				playerReset();
			}
		}

		public bool IsPlayerDead()
		{
			return m_ZeroHealthHappened;
		}

		public void TakeDamage(float amount){
			Debug.Log ("shouldn't be used!!!!!!!!!");
		}

		// This is called whenever the tank takes damage. Implements IDamageObject. !!!!!!!!!also called Damage
		public void Damage(float amount)
		{
			if (invulnerable)
			{
				return;
			}

//			RpcDamageFlash(m_LastDamagedByPlayerNumber); 

			//If we have shields, ensure that these are reduced before applying damage to the tank's main health.
			if (m_ShieldLevel > 0)
			{
				m_ShieldLevel -= amount;

				//If shields have dropped below zero, transfer the balance of the damage to the tank's main health.
				if (m_ShieldLevel <= 0)
				{
					amount = Mathf.Abs(m_ShieldLevel);
					m_ShieldLevel = 0;
				}
				else
				{
					amount = 0;
				}
			}

			// Reduce current health by the amount of damage done.
			m_CurrentHealth -= amount;

			// Change the UI elements appropriately.
			SetHealthAndShieldUI ();

			// If the current health is at or below zero and it has not yet been registered, call OnZeroHealth.

			if (m_CurrentHealth <= 0f && !m_ZeroHealthHappened)
			{
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
			// Set the flag so that this function is only called once.
			m_ZeroHealthHappened = true;

			RpcOnZeroHealth ();

			//TODO: what the fuck is this?
//			if (isServer)
//			{
//				GameManager.s_Instance.rulesProcessor.TankDies(m_Manager);
//			} 
		}

		//Assigns internal damage variables from explosion.
		public void SetDamagedBy(int playerNumber, string explosionId)
		{
			//If we've received the tank suicide index, replace it with this tank's player index to count it as a suicide.
			if (playerNumber == TANK_SUICIDE_INDEX)
			{
//				playerNumber = m_Manager.playerNumber;
			}

			Debug.LogFormat("Destroyed by playerNumber = {0}", playerNumber);
			m_LastDamagedByPlayerNumber = playerNumber;
			m_LastDamagedByExplosionId = explosionId;
		}



		//Sets the shield level to a given value. Called by the shield powerup object to enable shields.
		public void SetShieldLevel(float value)
		{
			m_ShieldLevel = value;
		}



		private void SetTankActive(bool active)
		{
			if (m_Collider == null && m_TankDisplay != null)
			{
				m_Collider = m_TankDisplay.GetComponent<BoxCollider>();
			}
			if (m_Collider != null)
			{
				m_Collider.enabled = active;
			}

			m_TankDisplay.SetVisibleObjectsActive(active);

			Debug.Log ("Related to TANKMANAGER HERE!");

//TODO:  collaborate with tankmanager.
//			if (active)
//			{
//				m_Manager.EnableControl();
//			}
//			else
//			{
//				m_Manager.DisableControl();
//			}
		}


























        private void SetHealthAndShieldUI ()
        {
            // Set the slider's value appropriately.
            m_Slider.value = m_CurrentHealth;

            // Interpolate the color of the bar between the choosen colours based on the current percentage of the starting health.
            m_FillImage.color = Color.Lerp (m_ZeroHealthColor, m_FullHealthColor, m_CurrentHealth / m_StartingHealth);

			Debug.Log ("Shield UI!!!");
        }

		//Hooked into the currenthealth syncvar. Updates whenever health changes server-side.
		void OnCurrentHealthChanged(float value)
		{
			m_CurrentHealth = value;

			if (healthChanged != null)
			{
				healthChanged(m_CurrentHealth / m_StartingHealth);
			}
		}

		//Hooked into the shield level syncvar. Updates whenever shield level changes server-side.
		void OnShieldLevelChanged(float value)
		{
			m_ShieldLevel = value;

			m_TankDisplay.SetShieldBubbleActive(m_ShieldLevel > 0);

			if (shieldChanged != null)
			{
				shieldChanged(m_ShieldLevel / m_StartingHealth);
			}
		}
        






		//  TODO:!!!!!!!!!!!!!!!!Now all RPC things.!!!!!!!!!!!!!!!!
		//		//Initializes all required references to external scripts.
		//		public void Init(TankManager manager)
		//		{
		//			m_Manager = manager;
		//			m_TankDisplay = manager.display;
		//			m_StartingHealth = manager.playerTankType.hitPoints;
		//			m_Collider = m_TankDisplay.GetComponent<BoxCollider>();
		//		}
		//
		//		[ClientRpc]
		//		public void RpcDelayedReset()
		//		{
		//			m_Manager.Reset(null);
		//		}
		//
		//		[ClientRpc]
		//		//Fired on clients to make tanks damaged by the local player flash red.
		//		private void RpcDamageFlash(int sourcePlayer)
		//		{
		//			if (sourcePlayer == GameManager.s_Instance.GetLocalPlayerId())
		//			{
		//				m_TankDisplay.StartDamageFlash();
		//			}
		//		}
		//

		//Initializes all required references to external scripts.
		public void Init(TankManager manager)
		{
//			m_Manager = manager;
//			m_TankDisplay = manager.display;
//			m_StartingHealth = manager.playerTankType.hitPoints;
			m_Collider = m_TankDisplay.GetComponent<BoxCollider>();
		}

		[ClientRpc]
		private void RpcOnZeroHealth()
		{
			//-----------------------------------original-------------------------
			// Move the instantiated explosion prefab to the tank's position and turn it on.
			m_ExplosionParticles.transform.position = transform.position;
			m_ExplosionParticles.gameObject.SetActive (true);

			// Play the particle system of the tank exploding.
			m_ExplosionParticles.Play ();

			// Play the tank explosion sound effect.
			m_ExplosionAudio.Play();

//			// Turn the tank off.
//			gameObject.SetActive (false);
			// we are gonna do above in TankDisplay.cs
			//-----------------------------------original-------------------------
			// Break off our decorations
//			m_TankDisplay.DetachDecorations();

//			if (ExplosionManager.s_InstanceExists && m_DeathExplosion != null)
//			{
//				ExplosionManager.s_Instance.SpawnExplosion(transform.position, Vector3.up, gameObject, m_Manager.playerNumber, m_DeathExplosion, false);
//			}
//
			InternalOnZeroHealth();
		}

		//Fired on clients via RPC to perform death cleanup.
		private void InternalOnZeroHealth()
		{
			//Disable any active powerup SFX
			m_ShieldLevel = 0f;
			m_TankDisplay.SetShieldBubbleActive(false);
//			m_TankDisplay.SetNitroParticlesActive(false);

			// Disable the collider and all the appropriate child gameobjects so the tank doesn't interact or show up when it's dead.
			SetTankActive(false);

			if (m_CurrentSpawnPoint != null)
			{
				m_CurrentSpawnPoint.Decrement();
				// TODO: implement this!!!
			}

			if (playerDeath != null)
			{
				playerDeath();
				// TODO: use this!
			}
		}

    }
}