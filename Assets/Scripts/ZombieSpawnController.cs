using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using TMPro;
using UnityEngine;

public class ZombieSpawnController : MonoBehaviour
{
    public int initialZombiesPerWave = 5;
    public int currentZombiesPerWave;

    public float spawnDelay = 0.5f;                 // Interval between spawning each zombie in one wave

    public int currentWave = 0;
    public float waveCooldown = 10f;

    public bool inCooldown;
    public float cooldownCounter = 0;

    public List<Enemy> currentZombiesAlive;

    public GameObject zombiePrefab;

    public TextMeshProUGUI waveOverUI;
    public TextMeshProUGUI countDownUI;
    public TextMeshProUGUI currentWaveUI;

    private void Start()
    {
        currentZombiesPerWave = initialZombiesPerWave;

        GlobalEffect.Instance.waveNumber = currentWave;

        cooldownCounter = waveCooldown;

        StartNextWave();
    }

    private void StartNextWave()
    {
        currentZombiesAlive.Clear();

        currentWave++;

        GlobalEffect.Instance.waveNumber = currentWave;

        currentWaveUI.text = "Wave: " + currentWave.ToString();

        StartCoroutine(SpawnWave());
    }

    private IEnumerator SpawnWave()
    {
        for (int i = 0; i < currentZombiesPerWave; i++)
        {
            Vector3 spawnOffset = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f));
            Vector3 spawnPosition = transform.position + spawnOffset;

            GameObject zombie = Instantiate(zombiePrefab, spawnPosition, Quaternion.identity);

            Enemy enemyScript = zombie.GetComponent<Enemy>();
            currentZombiesAlive.Add(enemyScript);

            yield return new WaitForSeconds(spawnDelay);
        }
    }

    private void Update()
    {
        // Get all dead Zombies
        List<Enemy> zombiesToRemove = new List<Enemy>();
        foreach (Enemy zombie in currentZombiesAlive)
        {
            if (zombie.isDead)
            {
                zombiesToRemove.Add(zombie);
            }
        }

        // Remove all dead zombie
        foreach (Enemy zombie in zombiesToRemove)
        {
            currentZombiesAlive.Remove(zombie);
        }

        zombiesToRemove.Clear();

        // Start cooldown when all zombies are dead
        if (currentZombiesAlive.Count == 0 && inCooldown == false)
        {
            StartCoroutine(WaveCooldown());
        }

        if (inCooldown)
        {
            cooldownCounter -= Time.deltaTime;
        } 
        else
        {
            cooldownCounter = waveCooldown;
        }

        countDownUI.text = cooldownCounter.ToString("F0");
    }

    private IEnumerator WaveCooldown()
    {
        inCooldown = true;
        waveOverUI.gameObject.SetActive(true);

        yield return new WaitForSeconds(waveCooldown);

        inCooldown = false;
        waveOverUI.gameObject.SetActive(false);
        currentZombiesPerWave += 5;

        StartNextWave();
    }

}
