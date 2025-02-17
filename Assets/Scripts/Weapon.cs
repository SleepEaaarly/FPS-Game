using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public bool isActiveWeapon;
    public int weaponDamage;

    // Shooting
    public bool isShooting, alreadyReset;
    bool allowReset = true;
    public float shootingDelay = 2f;

    // Burst
    public int bulletsPerBurst = 3;

    // Spread
    public float spreadIntensity;

    // Bullet
    public GameObject bulletPrefab;
    public Transform bulletSpawn;
    public float bulletVelocity = 30;
    public float bulletPrefabLifeTime = 3f;

    // muzzle & animation
    public GameObject muzzleEffect;
    internal Animator animator;

    public enum WeaponModel
    {
        M1911,
        AK74,
        M4
    }

    public WeaponModel thisWeaponModel;

    // shootingMode
    public enum ShootingMode
    {
        Single, 
        Burst,
        Auto
    }

    public ShootingMode currentShootingMode;

    // loading
    public float reloadTime;
    public int magazineSize, bulletsLeft;
    public bool isReloading;

    public Vector3 spawnPosition;
    public Vector3 spawnRotation;

    private void Awake()
    {
        alreadyReset = true;
        animator = GetComponent<Animator>();
        isReloading = false;

        bulletsLeft = magazineSize;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isActiveWeapon) { 
            foreach (Transform child in transform)
            {
                child.gameObject.layer = LayerMask.NameToLayer("Default");
            }
            return;
        }

        foreach (Transform child in transform)
        {
            child.gameObject.layer = LayerMask.NameToLayer("WeaponRender");
        }

        GetComponent<Outline>().enabled = false;

        if (currentShootingMode == ShootingMode.Auto)
        {
            // Holding down left mouse button
            isShooting = Input.GetKey(KeyCode.Mouse0);
        }
        else if (currentShootingMode == ShootingMode.Single ||
            currentShootingMode == ShootingMode.Burst)
        {
            isShooting = Input.GetKeyDown(KeyCode.Mouse0);
        }

        if (bulletsLeft == 0 && isShooting)
        {
            SoundManager.Instance.emptyMagazineSound.Play();
        }

        if (Input.GetKeyDown(KeyCode.R) && bulletsLeft < magazineSize && !isReloading && WeaponManager.Instance.CheckAmmoLeftFor(thisWeaponModel) > 0)
        {
            Reload();
        }

        if (bulletsLeft <= 0 && !isReloading && allowReset && WeaponManager.Instance.CheckAmmoLeftFor(thisWeaponModel) > 0)
        {
            Reload();
        }

        if (alreadyReset && isShooting && !isReloading && bulletsLeft > 0) 
        {
            FireWeapon();
        }

        //if (AmmoManager.Instance.ammoDisplay != null) 
        //{
        //    AmmoManager.Instance.ammoDisplay.text = $"{bulletsLeft/bulletsPerBurst}/{magazineSize/bulletsPerBurst}";
        //}
    }

    private void shootBullet()
    {
        Vector3 shootingDirection = CalculateDirectionAndSpread().normalized;

        // Instantiate the bullet
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.position, Quaternion.identity);

        Bullet bul = bullet.GetComponent<Bullet>();
        bul.bulletDamage = weaponDamage;

        // Pointing the bullet to face the shooting direction
        bullet.transform.forward = shootingDirection;

        // Shoot the bullet
        bullet.GetComponent<Rigidbody>().AddForce(shootingDirection * bulletVelocity, ForceMode.Impulse);

        // Destory the bullet after some time
        StartCoroutine(DestroyBulletAfterTime(bullet, bulletPrefabLifeTime));
    }

    private void FireWeapon()
    {
        bulletsLeft--;

        muzzleEffect.GetComponent<ParticleSystem>().Play();
        
        animator.SetTrigger("RECOIL");

        SoundManager.Instance.PlayShootingSound(thisWeaponModel);

        alreadyReset = false;

        if (currentShootingMode == ShootingMode.Burst) 
        {
            for (int i = 0; i < bulletsPerBurst; i++)
            {
                shootBullet();
            }
        }
        else
        {
            shootBullet();
        }

        // Checking if we are done shooting
        if (allowReset)     // variant "allowReset" is to invoke function "ResetShot" only once in "Burst" mode
        {
            Invoke("ResetShot", shootingDelay);
            allowReset = false;
        }

    }

    private void Reload()
    {
        // SoundManager.Instance.reloadingSoundM1911.Play();
        SoundManager.Instance.PlayReloadSound(thisWeaponModel);

        animator.SetTrigger("RELOAD");

        isReloading = true;
        Invoke("ReloadCompleted", reloadTime);
    }

    private void ReloadCompleted()
    {
        int ammoLeft = WeaponManager.Instance.CheckAmmoLeftFor(thisWeaponModel);
        int currentBulletsLeft = bulletsLeft;
        if (ammoLeft + currentBulletsLeft > magazineSize)
        {
            bulletsLeft = magazineSize;
            WeaponManager.Instance.DecreaseTotalAmmo(magazineSize - currentBulletsLeft, thisWeaponModel);
        }
        else
        {
            bulletsLeft = ammoLeft + currentBulletsLeft;
            WeaponManager.Instance.DecreaseTotalAmmo(ammoLeft, thisWeaponModel);
        }

        isReloading = false;
    }

    private void ResetShot()
    {
        // Debug.Log("Reset shot");
        alreadyReset = true;
        allowReset = true;
    }

    private Vector3 CalculateDirectionAndSpread()
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        Vector3 targetPoint;
        if (Physics.Raycast(ray, out hit))
        {
            targetPoint = hit.point;
        }
        else
        {
            targetPoint = ray.GetPoint(100);
        }

        Vector3 direction = targetPoint - bulletSpawn.position;
        
        float x = UnityEngine.Random.Range(-spreadIntensity, spreadIntensity);
        float y = UnityEngine.Random.Range(-spreadIntensity, spreadIntensity);

        // return shooting direction and spread
        return direction + new Vector3(x, y, 0);
    }

    private IEnumerator DestroyBulletAfterTime(GameObject bullet, float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(bullet);
    }
}
