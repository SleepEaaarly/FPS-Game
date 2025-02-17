using System;
using UnityEngine;

public class Throwable : MonoBehaviour
{
    [SerializeField] float delay = 3f;
    [SerializeField] float damageRadius = 20f;
    [SerializeField] float explosionForce = 1200f;

    float countdown;

    bool hasExploded = false;
    public bool hasBeenThrown = false;

    public enum ThrowableType
    {
        Grenade,
        Smoke_Grenade
    }

    public ThrowableType throwableType;

    private void Start()
    {
        countdown = delay;
    }

    private void Update()
    {
        if (hasBeenThrown)
        {
            countdown -= Time.deltaTime;
            if (countdown <= 0 && !hasExploded)
            {
                Explode();
                hasExploded = true;
            }
        }
    }

    private void Explode()
    {
        switch (throwableType)
        {
            case ThrowableType.Grenade:
                GrenadeEffect();
                break;
            case ThrowableType.Smoke_Grenade:
                SmokeGrenadeEffect();
                break;
        }

        Destroy(gameObject);
    }

    
    private void SmokeGrenadeEffect()
    {
        // Visual Effect
        GameObject smokeEffect = GlobalEffect.Instance.smokeGrenadeEffect;
        Instantiate(smokeEffect, transform.position, transform.rotation);

        // Play Sound
        SoundManager.Instance.throwablesChannel.PlayOneShot(SoundManager.Instance.smokeGrenadeSound);

        // Physical Effect 
        Collider[] colliders = Physics.OverlapSphere(transform.position, damageRadius);
        foreach (Collider collider in colliders)
        {
            Rigidbody rb = collider.GetComponent<Rigidbody>();
            if (rb != null)
            {
                // Apply blindness to enemy
            }

            // apply damage to enemy

        }
    }
    

    private void GrenadeEffect()
    {
        // Visual Effect
        GameObject explosionEffect = GlobalEffect.Instance.grenadeExplosionEffect;
        Instantiate(explosionEffect, transform.position, transform.rotation);

        // Play Sound
        SoundManager.Instance.throwablesChannel.PlayOneShot(SoundManager.Instance.grenadeSound);

        // Physical Effect 
        Collider[] colliders = Physics.OverlapSphere(transform.position, damageRadius);
        foreach (Collider collider in colliders)
        {
            Rigidbody rb = collider.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(explosionForce, transform.position, damageRadius, 0, ForceMode.Impulse);
            }

            if (collider.GetComponent<Enemy>())
            {
                collider.gameObject.GetComponent<Enemy>().TakeDamage(100);
            }

        }

    }

}
