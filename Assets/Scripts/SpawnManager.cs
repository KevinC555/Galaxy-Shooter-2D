using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _enemyContainer;
    private bool _stopSpawning = false;

    [SerializeField]
    private GameObject[] _wave1;
    [SerializeField]
    private GameObject[] _wave2;
    [SerializeField]
    private GameObject[] _wave3;
    [SerializeField]
    private GameObject[] _wave4;
    [SerializeField]
    private GameObject[] _wave5;

    private int _currentWave = 1;

    private UIManager _uiManager;

    [SerializeField]
    private GameObject[] _commonPowerups;
    [SerializeField]
    private GameObject[] _rarePowerups;
    [SerializeField]
    private GameObject[] _extraRarePowerups;

    [SerializeField]
    private GameObject[] _enemyPrefabs;

    [SerializeField]
    private GameObject _bossPrefab;

    private void Start()
    {
        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();

        if (_uiManager == null)
        {
            Debug.LogError("The UI Manager is null.");
        }
    }

    public void StartSpawning()
    {
        StartCoroutine(SpawnEnemies());
        StartCoroutine(SpawnPowerupRoutine());
    }

    IEnumerator SpawnEnemies()
    {
        while (_stopSpawning == false)
        {
            switch (_currentWave)
            {
                case 1:
                    _uiManager.UpdateWave(_currentWave);
                    _currentWave++;
                    foreach (GameObject enemy in _wave1)
                    {
                        SpawnEnemy();
                        yield return new WaitForSeconds(1.0f);
                    }
                    break;
                case 2:
                    _uiManager.UpdateWave(_currentWave);
                    _currentWave++;
                    foreach (GameObject enemy in _wave2)
                    {
                        SpawnEnemy();
                        yield return new WaitForSeconds(1.0f);
                    }
                    break;
                case 3:
                    _uiManager.UpdateWave(_currentWave);
                    _currentWave++;
                    foreach (GameObject enemy in _wave3)
                    {
                        SpawnEnemy();
                        yield return new WaitForSeconds(1.0f);
                    }
                    break;
                case 4:
                    _uiManager.UpdateWave(_currentWave);
                    _currentWave++;
                    foreach (GameObject enemy in _wave4)
                    {
                        SpawnEnemy();
                        yield return new WaitForSeconds(1.0f);
                    }
                    break;
                case 5:
                    _uiManager.UpdateWave(_currentWave);
                    _currentWave++;
                    foreach (GameObject enemy in _wave5)
                    {
                        SpawnEnemy();
                        yield return new WaitForSeconds(1.0f);
                    }
                    break;
                case 6:
                    _uiManager.UpdateWave(_currentWave);
                    _uiManager.SetBossHealth(100);
                    _currentWave++;
                    StopCoroutine(SpawnEnemies());
                    SpawnBoss();
                    break;
                default:
                    Debug.Log("All out of waves.");
                    break;
            }
            yield return new WaitForSeconds(11.0f);
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

    private void SpawnEnemy()
    {
        int _randomEnemyPrefab = Random.Range(0, _enemyPrefabs.Length);
        Vector3 posToSpawn = new Vector3(Random.Range(-8f, 8f), 9, 0);
        GameObject newEnemy = Instantiate(_enemyPrefabs[_randomEnemyPrefab], posToSpawn, Quaternion.identity);
        newEnemy.transform.parent = _enemyContainer.transform;
    }

    private void SpawnBoss()
    {
        Vector3 posToSpawn = new Vector3(0, 10f, 0);
        GameObject newBoss = Instantiate(_bossPrefab, posToSpawn, Quaternion.identity);
        newBoss.transform.parent = _enemyContainer.transform;
    }

    public void OnPlayerDeath()
    {
        _stopSpawning = true;
        DestroyAll("Enemy", "Powerup", "EnemyLaser");
        StopAllCoroutines();
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
}
