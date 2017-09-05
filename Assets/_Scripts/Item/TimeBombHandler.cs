using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Complete;

public class TimeBombHandler : ItemHandlerAbstractClass, IExplosiveItem
{
    public string ExplosionId;
    public float MaxDamage;
    public float ExplosionForce = 1000f;              // The amount of force added to a tank at the centre of the explosion.
    public float ExplosionRadius = 5f;                // The maximum distance away from the explosion tanks can be and are still affected.
    public GameObject ExplosionPrefab;
    private GameObject ItemExplosion;

    public float CountDownSeconds;
    private float CountDownTime = 0;
    private bool AlreadyExplode = false;

    private void OnEnable()
    {
        
    }

    private void Update()
    {
        if (AlreadyExplode)
        {
            return;
        }
        CountDownTime += Time.deltaTime;
        if (CountDownTime>= CountDownSeconds)
        {
            Explode();
            
        }
    }

    public void Explode()
    {
        if (AlreadyExplode)
        {
            return;
        }
        AlreadyExplode = true;
        Collider[] TankColliders = Physics.OverlapSphere(transform.position, ExplosionRadius, TankMask);
        foreach(Collider c in TankColliders)
        {
            Rigidbody targetRigidbody = c.GetComponentInParent<Rigidbody>();
            if (!targetRigidbody)
                return;
            targetRigidbody.AddExplosionForce(ExplosionForce, transform.position, ExplosionRadius);

            TankHealth targetHealth = targetRigidbody.GetComponentInParent<TankHealth>();

            // If there is no TankHealth script attached to the gameobject, go on to the next collider.
            if (!targetHealth)
                return;

            // Calculate the amount of damage the target should take based on it's distance from the shell.
            float damage = CalculateDamage(targetRigidbody.position);

            targetHealth.Damage(damage, DeployByTankId, ExplosionId);
        }

        ItemExplosion = Instantiate(ExplosionPrefab,transform.position, Quaternion.Euler(-90, 0, 0));
        ParticleSystem.MainModule mainModule = ItemExplosion.GetComponent<ParticleSystem>().main;
        Destroy(ItemExplosion, 1f);
        Destroy(gameObject);
    }

    virtual public float CalculateDamage(Vector3 targetPosition)
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
        damage = Mathf.Max(0f, damage);

        return damage;
    }

}
