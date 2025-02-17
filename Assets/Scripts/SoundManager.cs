using UnityEngine;
using static Weapon;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; set; }

    public AudioSource ShootingChannel;
    public AudioClip M1911Shot;
    public AudioClip AK74Shot;
    public AudioClip M4Shot;

    public AudioSource reloadingSoundM1911;
    public AudioSource reloadingSoundAK74;
    public AudioSource reloadingSoundM4;

    public AudioSource emptyMagazineSound;

    public AudioSource throwablesChannel;
    public AudioClip grenadeSound;
    public AudioClip smokeGrenadeSound;

    public AudioClip zombieWalking;
    public AudioClip zombieChase;
    public AudioClip zombieAttack;
    public AudioClip zombieHurt;
    public AudioClip zombieDeath;
    public AudioSource zombieChannel;
    public AudioSource zombieChannel2;

    public AudioSource playerChannel;
    public AudioClip playerHurt;
    public AudioClip playerDeath;

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
    
    public void PlayShootingSound(WeaponModel weapon)
    {
        switch (weapon) 
        {
            case WeaponModel.M1911:
                ShootingChannel.PlayOneShot(M1911Shot);
                break;
            case WeaponModel.AK74:
                ShootingChannel.PlayOneShot(AK74Shot);
                break;
            case WeaponModel.M4:
                ShootingChannel.PlayOneShot(M4Shot); 
                break;
        }

    }

    public void PlayReloadSound(WeaponModel weapon)
    {
        switch (weapon)
        {
            case WeaponModel.M1911:
                reloadingSoundM1911.Play();
                break;
            case WeaponModel.AK74:
                reloadingSoundAK74.Play();
                break;
            case WeaponModel.M4:
                reloadingSoundM4.Play();
                break;
        }
    }
}
