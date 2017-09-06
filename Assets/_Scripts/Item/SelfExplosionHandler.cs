using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Complete;

public class SelfExplosionHandler : ItemHandlerAbstractClass
{
    public float SelfExplosionDelay;
    public GameObject SelfExplosionEffect;
    private WaitForSeconds ExplosionWait;
    public float SelfDamagePortion;
    public float OthersDamageAmount;

    private void OnEnable()
    {
        ExplosionWait = new WaitForSeconds(SelfExplosionDelay);
        StartCoroutine(PrepareExplosion());
    }

    private IEnumerator PrepareExplosion()
    {
        yield return ExplosionWait;
        SelfExplosionEffect.SetActive(true);
        Collider[] TankColliders = Physics.OverlapSphere(transform.position, 3f, TankMask);
        foreach(Collider c in TankColliders)
        {
            TankShooting TSScript = c.GetComponentInParent<TankShooting>();
            TankHealth THScript = c.GetComponentInParent<TankHealth>();
            if (TSScript && THScript)
            {
                int TankNumber = TSScript._PlayerNumber;
                float dmgAmount = 0;

                // check whether is the deployer.
                if(TankNumber == DeployByTankId)
                {
                    if (THScript.GetCurrentShieldLevel() >= 0f)
                    {
                        dmgAmount += THScript.GetCurrentShieldLevel();
                    }
                    // decrease half of current life.
                    if (THScript.GetCurrentHealth() >= 0f)
                    {
                        dmgAmount += THScript.GetCurrentHealth() * SelfDamagePortion;
                    }
                }
                else
                {
                    dmgAmount = OthersDamageAmount;
                }

                THScript.Damage(dmgAmount, DeployByTankId, "");
            }
        }
    }
}
