using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpreadShellHandler : ShellHandlerAbstractClass {
    override public void Explode(Collider other)
    {
        // Collect all the colliders in a sphere from the shell's current position to a radius of the explosion radius.
        Collider[] TankColliders = Physics.OverlapSphere(transform.position, ExplosionRadius, TankMask);
        Collider[] WallColliders = Physics.OverlapSphere(transform.position, ExplosionRadius, WallMask);
        Collider[] ShellColliders = Physics.OverlapSphere(transform.position, ExplosionRadius, ShellMask);
        Collider[] GeneralItemsColliders = Physics.OverlapSphere(transform.position, ExplosionRadius, GeneralItemsMask);
        Collider[] ExplosiveItemColliders = Physics.OverlapSphere(transform.position, ExplosionRadius, ExplosiveItemsMask);
        Collider[] GroundColliders = Physics.OverlapSphere(transform.position, ExplosionRadius, GroundMask);

        // Go through all the colliders...
        for (int i = 0; i < TankColliders.Length; i++)
        {
            CollideWithTanks(TankColliders[i]);
        }
        for (int i = 0; i < WallColliders.Length; i++)
        {
            CollideWithWalls(WallColliders[i]);
        }
        for (int i = 0; i < ShellColliders.Length; i++)
        {
            CollideWithShells(ShellColliders[i]);
        }
        for (int i = 0; i < GeneralItemsColliders.Length; i++)
        {
            CollideWithGeneralItem(GeneralItemsColliders[i]);
        }
        for (int i = 0; i < ExplosiveItemColliders.Length; i++)
        {
            CollideWithExplosiveItems(GeneralItemsColliders[i]);
        }
        for (int i = 0; i < GroundColliders.Length; i++)
        {
            CollideWithGround(GroundColliders[i]);
        }

        // now, finialize explosion. perform Particles.
        // prepare the explosion system
        ExplosionParticles = Instantiate(ShellExplosion).GetComponent<ParticleSystem>();
        if (ExplosionAudio == null)
            ExplosionAudio = ExplosionParticles.GetComponent<AudioSource>();
        ExplosionParticles.transform.position = transform.position;
        ExplosionParticles.gameObject.SetActive(true);
        if (ExplosionParticles)
        {
            // Unparent the particles from the shell.
            ExplosionParticles.transform.parent = null;

            // Play the particle system.
            ExplosionParticles.Play();
            // Play the explosion sound effect.
            if (ExplosionAudio)
            {
                ExplosionAudio.volume = UnityEngine.Random.Range(0.1f, 0.25f);
                ExplosionAudio.Play();
            }
                
            

            // Once the particles have finished, destroy the gameobject they are on.
            ParticleSystem.MainModule mainModule = ExplosionParticles.main;
            Destroy(ExplosionParticles.gameObject, mainModule.duration);
        }

        Destroy(gameObject);
    }
}
