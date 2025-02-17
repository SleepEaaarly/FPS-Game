using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static Weapon;

public class WeaponManager : MonoBehaviour
{
    public static WeaponManager Instance { get; set; }

    public List<GameObject> weaponSlots;

    public GameObject activeWeaponSlot;

    [Header("Ammo")]
    public int totalPistolAmmo = 0;
    public int totalRifleAmmo = 0;
    public int totalShotgunAmmo = 0;

    [Header("Throwables")]
    public int grenades = 0;
    public float throwForce = 10f;
    public GameObject grenadePrefab;

    public GameObject throwableSpawn;
    public float forceMultiplier = 0;
    public float forceMultiplierLimit = 2;

    public int smokeGrenades = 0;
    public GameObject smokeGrenadePrefab;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

    }

    private void Start()
    {
        activeWeaponSlot = weaponSlots[0];
    }

    private void Update()
    {
        foreach (GameObject weaponSlot in weaponSlots)
        {
            if (weaponSlot == activeWeaponSlot)
            {
                weaponSlot.SetActive(true);
            }
            else
            {
                weaponSlot.SetActive(false);
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SwitchActiveSlot(0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SwitchActiveSlot(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SwitchActiveSlot(2);
        }

        if (Input.GetKey(KeyCode.G))
        {
            forceMultiplier += Time.deltaTime;

            if (forceMultiplier > forceMultiplierLimit)
            {
                forceMultiplier = forceMultiplierLimit;
            }
        }

        if (Input.GetKeyUp(KeyCode.G))
        {
            if (grenades > 0)
            {
                ThrowLethal();
            }

            forceMultiplier = 0;
        }


        if (Input.GetKey(KeyCode.T))
        {
            forceMultiplier += Time.deltaTime;

            if (forceMultiplier > forceMultiplierLimit)
            {
                forceMultiplier = forceMultiplierLimit;
            }
        }

        if (Input.GetKeyUp(KeyCode.T))
        {
            if (smokeGrenades > 0)
            {
                ThrowTactical();
            }

            forceMultiplier = 0;
        }
    }

    #region || ---- Weapon ---- ||
    public void PickupWeapon(GameObject pickedupWeapon)
    {
        AddWeaponIntoActiveSlot(pickedupWeapon);
    }

    private void AddWeaponIntoActiveSlot(GameObject pickedupWeapon)
    {
        DropCurrentWeapon(pickedupWeapon);

        pickedupWeapon.transform.SetParent(activeWeaponSlot.transform);

        Weapon weapon = pickedupWeapon.GetComponent<Weapon>();

        weapon.transform.localPosition = new Vector3(weapon.spawnPosition.x, weapon.spawnPosition.y, weapon.spawnPosition.z);
        weapon.transform.localRotation = Quaternion.Euler(weapon.spawnRotation.x, weapon.spawnRotation.y, weapon.spawnRotation.z);

        setWeaponActive(weapon);
    }

    private void DropCurrentWeapon(GameObject pickedupWeapon)
    {
        if (activeWeaponSlot.transform.childCount > 0)
        {
            var weaponToDrop = activeWeaponSlot.transform.GetChild(0).gameObject;

            setWeaponInactive(weaponToDrop.GetComponent<Weapon>());

            weaponToDrop.transform.SetParent(pickedupWeapon.transform.parent);
            weaponToDrop.transform.localPosition = pickedupWeapon.transform.localPosition;
            weaponToDrop.transform.localRotation = pickedupWeapon.transform.localRotation;
        }
    }

    public void SwitchActiveSlot(int slotNumber)
    {
        if (activeWeaponSlot.transform.childCount > 0)
        {
            Weapon currentWeapon = activeWeaponSlot.transform.GetChild(0).GetComponent<Weapon>();
            setWeaponInactive(currentWeapon);
        }

        activeWeaponSlot = weaponSlots[slotNumber];

        if (activeWeaponSlot.transform.childCount > 0)
        {
            Weapon newWeapon = activeWeaponSlot.transform.GetChild(0).GetComponent<Weapon>();
            setWeaponActive(newWeapon);
        }

    }

    private void setWeaponActive(Weapon weapon)
    {
        weapon.isActiveWeapon = true;
        weapon.animator.enabled = true;
    }

    private void setWeaponInactive(Weapon weapon)
    {
        weapon.isActiveWeapon = false;
        weapon.animator.enabled = false;
    }
    #endregion

    #region || ---- Ammo ---- ||
    internal void PickupAmmo(AmmoBox ammoBox)
    {
        switch (ammoBox.ammoType)
        {
            case AmmoBox.AmmoType.PistolAmmo:
                totalPistolAmmo += ammoBox.ammoAmount;
                break;
            case AmmoBox.AmmoType.RifleAmmo:
                totalRifleAmmo += ammoBox.ammoAmount;
                break;
            case AmmoBox.AmmoType.ShotgunAmo:
                totalShotgunAmmo += ammoBox.ammoAmount;
                break;
        }
    }

    internal void DecreaseTotalAmmo(int bulletsLeft, Weapon.WeaponModel thisWeaponModel)
    {
        switch (thisWeaponModel)
        {
            case Weapon.WeaponModel.M1911:
                totalPistolAmmo -= bulletsLeft;
                break;
            case Weapon.WeaponModel.AK74:
                totalRifleAmmo -= bulletsLeft;
                break;
            case Weapon.WeaponModel.M4:
                totalShotgunAmmo -= bulletsLeft;
                break;
        }
    }

    public int CheckAmmoLeftFor(Weapon.WeaponModel thisWeaponModel)
    {
        switch (thisWeaponModel)
        {
            case Weapon.WeaponModel.M1911:
                return WeaponManager.Instance.totalPistolAmmo;
            case Weapon.WeaponModel.AK74:
                return WeaponManager.Instance.totalRifleAmmo;
            case Weapon.WeaponModel.M4:
                return WeaponManager.Instance.totalShotgunAmmo;
            default:
                return 0;
        }
    }

    #endregion

    #region || ---- Throwable ---- ||
    public void PickupThrowable(Throwable throwable)
    {
        switch (throwable.throwableType)
        {
            case Throwable.ThrowableType.Grenade:
                PickupGrenade();
                break;
            case Throwable.ThrowableType.Smoke_Grenade:
                PickupSmokeGrenade();
                break;
        }
    }

    private void PickupSmokeGrenade()
    {
        smokeGrenades += 1;

        HUDManager.Instance.UpdateThrowables(Throwable.ThrowableType.Smoke_Grenade);
    }

    private void PickupGrenade()
    {
        grenades += 1;

        HUDManager.Instance.UpdateThrowables(Throwable.ThrowableType.Grenade);
    }

    private void ThrowLethal()
    {
        GameObject lethalPrefab = grenadePrefab;

        GameObject throwable = Instantiate(lethalPrefab, throwableSpawn.transform.position, Camera.main.transform.rotation);
        Rigidbody rb = throwable.GetComponent<Rigidbody>();

        rb.AddForce(Camera.main.transform.forward * (throwForce * forceMultiplier), ForceMode.Impulse);

        throwable.GetComponent<Throwable>().hasBeenThrown = true;

        grenades -= 1;
        HUDManager.Instance.UpdateThrowables(Throwable.ThrowableType.Grenade);
    }

    private void ThrowTactical()
    {
        GameObject tacticalPrefab = smokeGrenadePrefab;

        GameObject throwable = Instantiate(smokeGrenadePrefab, throwableSpawn.transform.position, Camera.main.transform.rotation);
        Rigidbody rb = throwable.GetComponent<Rigidbody>();

        rb.AddForce(Camera.main.transform.forward * (throwForce * forceMultiplier), ForceMode.Impulse);

        throwable.GetComponent<Throwable>().hasBeenThrown = true;

        smokeGrenades -= 1;
        HUDManager.Instance.UpdateThrowables(Throwable.ThrowableType.Smoke_Grenade);
    }

    #endregion
}
