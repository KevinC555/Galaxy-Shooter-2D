using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private Text _scoreText;
    [SerializeField]
    private Text _ammoText;
    [SerializeField]
    private Text _missileText;
    [SerializeField]
    private Image _LivesImg;
    [SerializeField]
    private Sprite[] _liveSprites;
    [SerializeField]
    private Text _gameOverText;
    [SerializeField]
    private Text _restartText;
    [SerializeField]
    private Text _waveText;

    private GameManager _gameManager;

    [SerializeField]
    private Slider _fuelGauge;
    [SerializeField]
    private Text _fuelText;
    [SerializeField]
    private string _fuelGaugeString = "Full";
    [SerializeField]
    private string _emptyGaugeString = "Emptying";

    private SpawnManager _spawnManager;

    // Start is called before the first frame update
    void Start()
    {
        _scoreText.text = "Score: " + 0;
        _ammoText.text = "Ammo: " + 15 + " / 15";
        _missileText.text = "Missiles: " + 3 + " / 3";
        _gameOverText.gameObject.SetActive(false);
        _gameManager = GameObject.Find("Game_Manager").GetComponent<GameManager>();
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();

        if (_gameManager == null)
        {
            Debug.LogError("GameManager is NULL.");
        }

        if (_spawnManager == null)
        {
            Debug.LogError("SpawnManager is null.");
        }
    }   

    public void UpdateScore(int playerScore)
    {
        _scoreText.text = "Score: " + playerScore.ToString();
    }
     
    public void UpdateAmmo(int ammoCount)
    {
        _ammoText.text = "Ammo: " + ammoCount + " / 15";
    }

    public void UpdateMissiles(int missileCount)
    {
        _missileText.text = "Missiles: " + missileCount + " / 3";
    }

    public void UpdateLives(int currentLives)
    {
        if (currentLives < 0)
        {
            Debug.Log("Lives were les than 0");
        }
        else
        {
            _LivesImg.sprite = _liveSprites[currentLives];
        }

        if (currentLives <= 0)
        {
            GameOverSequence();
        }
    }

    public void UpdateFuelGauge(float gaugeAmount) 
    {
        _fuelGauge.value = gaugeAmount;

        if (gaugeAmount >= 1)
        {
            _fuelText.text = _fuelGaugeString;
        }

        if (gaugeAmount <= 0)
        {
            _fuelText.text = _emptyGaugeString;
        }
    }

    public void UpdateWave(int _currentWave)
    {
        _waveText.gameObject.SetActive(true);
        _waveText.text = "Wave: " + _currentWave;
        StartCoroutine(WaveTextInactive());
    }

    IEnumerator WaveTextInactive()
    {
        if (_waveText.gameObject.activeInHierarchy)
        {
            yield return new WaitForSeconds(3);
            _waveText.gameObject.SetActive(false);
        }
    }

    void GameOverSequence()
    {
        _gameManager.GameOver();
        _gameOverText.gameObject.SetActive(true);
        _restartText.gameObject.SetActive(true);
        StartCoroutine(GameOverFlickerRoutine());
    }

    IEnumerator GameOverFlickerRoutine()
    {
        while (true)
        {
            _gameOverText.text = "GAME OVER";
            yield return new WaitForSeconds(0.5f);
            _gameOverText.text = " ";
            yield return new WaitForSeconds(0.5f);
        }
    }
}
