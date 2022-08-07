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
    private Image _LivesImg;
    [SerializeField]
    private Sprite[] _liveSprites;
    [SerializeField]
    private Text _gameOverText;
    [SerializeField]
    private Text _restartText;
    [SerializeField]
    private Text _waveText;
    [SerializeField]
    private Text _waveTextRequirement;

    private GameManager _gameManager;

    [SerializeField]
    private Slider _fuelGauge;

    [SerializeField]
    private Text _fuelText;
    [SerializeField]
    private string _fuelGaugeString = "Full";
    [SerializeField]
    private string _emptyGaugeString = "Emptying";

    // Start is called before the first frame update
    void Start()
    {
        _scoreText.text = "Score: " + 0;
        _ammoText.text = "Ammo: " + 15 + " / 15";
        _gameOverText.gameObject.SetActive(false);
        _gameManager = GameObject.Find("Game_Manager").GetComponent<GameManager>();

        if (_gameManager == null)
        {
            Debug.LogError("GameManager is NULL.");
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

    public void UpdateLives(int currentLives)
    {
        _LivesImg.sprite = _liveSprites[currentLives];

        if (currentLives == 0)
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

    public void UpdateWave(int _waveNumber)
    {
        _waveText.gameObject.SetActive(true);
        _waveText.text = "Wave: " + _waveNumber;
        _waveTextRequirement.gameObject.SetActive(true);
        _waveTextRequirement.text = "Earn " + 50 * _waveNumber + " points to reach next Wave";
        StartCoroutine(WaveTextInactive());
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

    IEnumerator WaveTextInactive()
    {
        if (_waveText.gameObject.activeInHierarchy)
        {
            yield return new WaitForSeconds(3);
            _waveText.gameObject.SetActive(false);
            _waveTextRequirement.gameObject.SetActive(false);
        }
    }

}
