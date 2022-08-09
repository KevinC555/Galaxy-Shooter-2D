using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _enemyPrefab;
    [SerializeField]
    private GameObject[] _powerups;
    [SerializeField]
    private GameObject _enemyContainer;
    private bool _stopSpawning = false;

    [SerializeField]
    private int _waveNumber;
    [SerializeField]
    private int _waitSeconds;

    private WaveManager _waveManager;

    public void Start()
    {
        _waveNumber = 1;

        _waveManager = GameObject.Find("Wave_Manager").GetComponent<WaveManager>();

        if (_waveManager == null)
        {
            Debug.LogError("WaveManager is null.");
        }
    }

    public void StartSpawning()
    {
        StartCoroutine(SpawnEnemyRoutine());
        StartCoroutine(SpawnPowerupRoutine());
        StartCoroutine(SpawnSecondaryFire());
    }

    IEnumerator SpawnEnemyRoutine()
    {
        int count = 0;

        yield return new WaitForSeconds(2.0f);

        while (_stopSpawning == false && count < 5)
        {
            if (_waveNumber < 6)
            {
                _waitSeconds = 6;
                _waitSeconds -= _waveNumber;
            }
            else
            {
                _waitSeconds = 1;
            }

            Vector3 posToSpawn = new Vector3(Random.Range(-8f, 8f), 7, 0);
            GameObject newEnemy = Instantiate(_enemyPrefab, posToSpawn, Quaternion.identity);
            newEnemy.transform.parent = _enemyContainer.transform;
            count++;

            yield return new WaitForSeconds(_waitSeconds);
        }
    }

    IEnumerator SpawnPowerupRoutine()
    {
        yield return new WaitForSeconds(3.0f);

        while (_stopSpawning == false)
        {
            Vector3 posToSpawn = new Vector3(Random.Range(-8f, 8f), 7, 0);
            int randomPowerup = Random.Range(0, 7);
            Instantiate(_powerups[randomPowerup], posToSpawn, Quaternion.identity);
            yield return new WaitForSeconds(Random.Range(3, 8));
        }
    }

    IEnumerator SpawnSecondaryFire()
    {
        yield return new WaitForSeconds(3.0f);

        while (_stopSpawning == false)
        {
            yield return new WaitForSeconds(Random.Range(40f, 80f));
            Vector3 posToSpawn = new Vector3(Random.Range(-8f, 8f), 7, 0);
            Instantiate(_powerups[5], posToSpawn, Quaternion.identity);
        }
    }

    public void OnPlayerDeath()
    {
        _stopSpawning = true;
        DestroyAll("Enemy", "Powerup", "EnemyLaser");
    }

    public void DestroyAll(string Enemy, string Powerup, string EnemyLaser)
    {
        GameObject[] enemy = GameObject.FindGameObjectsWithTag(Enemy);
        foreach (GameObject target in enemy)
        {
            GameObject.Destroy(target);
        }
        GameObject[] powerup = GameObject.FindGameObjectsWithTag(Powerup);
        foreach (GameObject target in powerup)
        {
            GameObject.Destroy(target);
        }
        GameObject[] laser = GameObject.FindGameObjectsWithTag(EnemyLaser);
        foreach (GameObject target in laser)
        {
            GameObject.Destroy(target);
        }
    }

    public void OnNextWave()
    {
        _waveNumber = _waveManager.GetWave();
        _stopSpawning = false;
    }
}
