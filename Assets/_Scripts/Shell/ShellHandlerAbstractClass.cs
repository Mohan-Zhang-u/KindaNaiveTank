using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Complete;

public abstract class ShellHandlerAbstractClass : MonoBehaviour {

	public string ShellId;
	// IDs.
    [HideInInspector]
	public int FireByTankId = -2;
	public string ExplosionId;

	public bool AmountLimited = false;
    [HeaderAttribute("If not AmountLimited, ignore.")]
    public int LimitedAmount;
	public int ReflectOrBounceTimes; // bounce like refectshell? or not?
	public float ShootColdDown; // if ShootColdDown = 0, just like a laser sword.
    [HeaderAttribute("If ForceChargeable, ignore.")]
    public float FlyingSpeed; //if not ForceChargeable, its speed shall be a constant.
    public bool ForceChargeable; // have a shoot distance? the more the far or fast? or not?
                                 /// <summary>
                                 /// if the shell is forceChargeable, press -> shoot once.
                                 /// else, press -> always shoot.
                                 /// </summary>
    // ----------need if ForceChargeable--------------
    [HeaderAttribute("If not ForceChargeable, ignore.")]
    public float MinShootForce;
	public float MaxShootForce;
	public float m_MaxChargeTime;


	public float MaxDamage;
	public float ExplosionForce = 1000f;              // The amount of force added to a tank at the centre of the explosion.
	public float MaxLifeTime = 2f;                    // The time in seconds before the shell is removed.
	public float ExplosionRadius = 5f;                // The maximum distance away from the explosion tanks can be and are still affected.
	public GameObject ShellExplosion;
	[HideInInspector]
	public ParticleSystem ExplosionParticles;         // Reference to the particles that will play on explosion.
	[HideInInspector]
	public AudioSource ExplosionAudio;                // Reference to the audio that will play on explosion.
	public GameObject ShellPrefab;

	public LayerMask TankMask;
	public LayerMask WallMask;
	public LayerMask ShellMask;
	public LayerMask GeneralItemsMask;
	public LayerMask ExplosiveItemsMask;
	public LayerMask GroundMask;
    public LayerMask ObstacleItemMask;

//	public void Awake(){
//		TankMask = LayerMask.NameToLayer ("TankToSpawn");
//		WallMask= LayerMask.NameToLayer ("Wall");
//		ShellMask= LayerMask.NameToLayer ("Shell");
//		GeneralItemsMask= LayerMask.NameToLayer ("GeneralItem");
//		ExplosiveItemsMask= LayerMask.NameToLayer ("ExplosiveItem");
//		GroundMask = LayerMask.NameToLayer ("Ground");
//	}

	// This function dont need to be overwrite
	public void Start ()
	{
		// If it isn't destroyed by then, destroy the shell after it's lifetime.
		Destroy (gameObject, MaxLifeTime);
	}

	//  ezly copy from ProjectileShellHandler.cs
	#region CopyAndPaste 
	///-----------------------------------------------NOW, START COPY AND PASTE!!!!!!-------------------------------------------------------------
	//its shared all accross!!!!!!!!!!!!!!!!!!!! 
	/// <summary>
	/// it is the ugliest part in C#. because, since this is a parent abstract class,
	/// if we dont copy and paste this code to its parent, the ExplosionParticles and ExplosionAudio
	/// will be THIS ABSTRACT CLASS' ExplosionParticles and ExplosionAudio.
	/// The only way we can use ExplosionParticles and ExplosionAudio in parent class is that 
	/// we copy and paste this function there.
	/// </summary>
	/// <param name="c">C.</param>
	// the main handler, AND THE DEFUALT IS, explode whenever hits a thing.!!!! can OPTIMIZE PERFORMANCE here. 
	virtual public void OnTriggerEnter (Collider other)
	{
		Explode (other);
	}

	// TODO: when it is Client side, we shall only apply the force. WHEN IT IS SERVER SIDE, moreover, we shall deal the damage.
	/// <summary>
	/// it is the ugliest part in C#. because, since this is a parent abstract class,
	/// if we dont copy and paste this code to its parent, the ExplosionParticles and ExplosionAudio
	/// will be THIS ABSTRACT CLASS' ExplosionParticles and ExplosionAudio.
	/// The only way we can use ExplosionParticles and ExplosionAudio in parent class is that 
	/// we copy and paste this function there.
	/// </summary>
	/// <param name="c">C.</param>
	virtual public void Explode (Collider other) {
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

		// now, finialize explosion. perform Particles.
		// prepare the explosion system
		ExplosionParticles = Instantiate (ShellExplosion).GetComponent<ParticleSystem> ();
		if (ExplosionAudio == null)
			ExplosionAudio = ExplosionParticles.GetComponent<AudioSource> ();
		ExplosionParticles.transform.position = transform.position;
		ExplosionParticles.gameObject.SetActive (true);
		if (ExplosionParticles) {
			// Unparent the particles from the shell.
			ExplosionParticles.transform.parent = null;

			// Play the particle system.
			ExplosionParticles.Play ();
			// Play the explosion sound effect.
			if (ExplosionAudio)
				ExplosionAudio.Play();

			// Once the particles have finished, destroy the gameobject they are on.
			ParticleSystem.MainModule mainModule = ExplosionParticles.main;
			Destroy (ExplosionParticles.gameObject, mainModule.duration);
		}

		Destroy (gameObject);
	}

	// its shared all accross!!!!!!!!!!!!!!!!!!!!
	/// <summary>
	/// it is the ugliest part in C#. because, since this is a parent abstract class,
	/// if we dont copy and paste this code to its parent, the ExplosionParticles and ExplosionAudio
	/// will be THIS ABSTRACT CLASS' ExplosionParticles and ExplosionAudio.
	/// The only way we can use ExplosionParticles and ExplosionAudio in parent class is that 
	/// we copy and paste this function there.
	/// </summary>
	/// <param name="c">C.</param>
	virtual public void CollideWithTanks(Collider c){
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
	}

	//TODO: im not pretty sure whether its is CORRECT!!!!!!!!!!!!!!!!!!!!
	virtual public void CollideWithShells(Collider c){
        // recursive call, will cause stackoverflow.
		//if (c.isTrigger) {
		//	c.GetComponent<ShellHandlerAbstractClass> ().Explode (ShellPrefab.GetComponent<Collider> ());
		//}
	}

	// targetPosition is tank's position. the function calculates according to tank's position and the explosive(shell's) position
	virtual public float CalculateDamage (Vector3 targetPosition)
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
		
	public virtual void CollideWithWalls(Collider c){

	}



	public virtual void CollideWithGeneralItem(Collider c){

	}

	//TODO: We Assumes that It Have Explode.
	public virtual void CollideWithExplosiveItems(Collider c){
		c.GetComponent<ShellHandlerAbstractClass> ().Explode (ShellPrefab.GetComponent<Collider> ());
	}

	public virtual void CollideWithGround(Collider c){

	}

	///-----------------------------------------------NOW, END COPY AND PASTE!!!!!!-------------------------------------------------------------
	#endregion
		
}
