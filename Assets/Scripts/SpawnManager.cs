using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _enemyPrefab;
    [SerializeField]
    private GameObject _rareEnemyPrefab;
    [SerializeField]
    private GameObject _enemyContainer;
    private bool _stopSpawning = false;

    [SerializeField]
    private int _waveNumber;
    [SerializeField]
    private int _waitSeconds;

    private WaveManager _waveManager;

    [SerializeField]
    private GameObject[] _commonPowerups;
    [SerializeField]
    private GameObject[] _rarePowerups;
    [SerializeField]
    private GameObject[] _extraRarePowerups;

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
        //StartCoroutine(SpawnRareEnemy()); Debugging
        StartCoroutine(SpawnPowerupRoutine());
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

            Vector3 posToSpawn = new Vector3(Random.Range(-8f, 8f), 9, 0);
            GameObject newEnemy = Instantiate(_enemyPrefab, posToSpawn, Quaternion.identity);
            newEnemy.transform.parent = _enemyContainer.transform;
            count++;

            yield return new WaitForSeconds(_waitSeconds);
        }
    }

    IEnumerator SpawnRareEnemy()
    {
        yield return new WaitForSeconds(2.0f);

        while (_stopSpawning == false)
        {
            Vector3 posToSpawn = new Vector3(Random.Range(-8f, 8f), 9, 0);
            GameObject newEnemy = Instantiate(_rareEnemyPrefab, posToSpawn, Quaternion.identity);
            newEnemy.transform.parent = _enemyContainer.transform;

            yield return new WaitForSeconds(6f);
        }
    }

    IEnumerator SpawnPowerupRoutine()
    {
        yield return new WaitForSeconds(3.0f);

        while (_stopSpawning == false)
        {
            float randomXPosition = Random.Range(-8f, 8f);
            Vector3 spawnPosition = new Vector3(randomXPosition, 7, 0);
            float randomSpawnTime = Random.Range(3f, 7f);
            int powerupChance = Random.Range(0, 11);
            int randomCommonPowerup = Random.Range(0, _commonPowerups.Length);
            int randomRarePowerup = Random.Range(0, _rarePowerups.Length);
            int randomExtraRarePowerup = Random.Range(0, _extraRarePowerups.Length);

            switch (powerupChance)
            {
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                    Instantiate(_commonPowerups[randomCommonPowerup], spawnPosition, Quaternion.identity);
                    break;
                case 7:
                case 8:
                case 9:
                    Instantiate(_rarePowerups[randomRarePowerup], spawnPosition, Quaternion.identity);
                    break;
                case 10:
                    Instantiate(_extraRarePowerups[randomExtraRarePowerup], spawnPosition, Quaternion.identity);
                    break;
                default:
                    Debug.Log("Default powerup value");
                    break;
            }
            
            yield return new WaitForSeconds(randomSpawnTime);
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
