using UnityEngine;
using System.Collections;
using System;

public class Bullet : MonoBehaviour
{
    public int bulletDamage;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Target"))
        {
            print("hit " + collision.gameObject.name + " !");

            CreateBulletImpactEffect(collision);

            Destroy(gameObject);
        }

        if (collision.gameObject.CompareTag("Wall"))
        {
            print("hit Wall !");

            CreateBulletImpactEffect(collision);

            Destroy(gameObject);
        }

        if (collision.gameObject.CompareTag("Enemy"))
        {
            print("hit zombie");

            if (collision.gameObject.GetComponent<Enemy>().isDead == false)
            {
                collision.gameObject.GetComponent<Enemy>().TakeDamage(bulletDamage);
            }

            CreateBloodSprayEffect(collision);

            Destroy(gameObject);
        }
    }

    private void CreateBloodSprayEffect(Collision collision)
    {
        ContactPoint contact = collision.contacts[0];
        GameObject bloodSprayPrefab = Instantiate(
            GlobalEffect.Instance.bloodSprayEffect,
            contact.point,
            Quaternion.LookRotation(contact.normal)
            );
        // WeaponEffect.Instance.holeLeft = hole;

        bloodSprayPrefab.transform.SetParent(collision.gameObject.transform);
    }

    void CreateBulletImpactEffect(Collision collision)
    {
        ContactPoint contact = collision.contacts[0];
        GameObject hole = Instantiate(
            GlobalEffect.Instance.bulletImpactEffectPrefab,
            contact.point,
            Quaternion.LookRotation(contact.normal)
            );
        // WeaponEffect.Instance.holeLeft = hole;

        hole.transform.SetParent(collision.gameObject.transform);
    }

}
