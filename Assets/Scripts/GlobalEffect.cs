using UnityEngine;
using System.Collections;

public class GlobalEffect : MonoBehaviour
{
    public static GlobalEffect Instance { get; set; }

    public GameObject bulletImpactEffectPrefab;
    // public int impactPrefabLifeTime = 6;

    public GameObject grenadeExplosionEffect;
    public GameObject smokeGrenadeEffect;

    public GameObject bloodSprayEffect;

    public int waveNumber = 0;
    // public GameObject holeLeft;
    // private bool isDestroying = false;

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

}
