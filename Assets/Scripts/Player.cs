using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    //public or private reference
    //data type (int, float, bool, string)
    //every variable has a name
    //optional value assigned
    [SerializeField]
    private GameObject _laserPrefab;
    [SerializeField]
    private GameObject _tripleShotPrefab;
    [SerializeField]
    private GameObject _secondaryPrefab;
    [SerializeField]
    private float _fireRate = 0.5f;
    private float _canFire = -1f;
    [SerializeField]
    private int _lives = 3;
    private SpawnManager _spawnManager;
    private WaveManager _waveManager;
    private CameraShake _camera;
    
    private bool _isTripleShotActive = false;
    private bool _isSpeedBoostActive = false;
    private bool _isShieldsActive = false;
    private bool _isSecondaryActive = false;
    private int _shieldLives = 3;

    [SerializeField]
    private GameObject _shieldVisualizer;
    [SerializeField]
    private GameObject _rightDamageVisualizer;
    [SerializeField]
    private GameObject _leftDamageVisualizer;
    [SerializeField]
    private GameObject _playerLeft;
    [SerializeField]
    private GameObject _playerRight;

    [SerializeField]
    private int _score;
    [SerializeField]
    private int _ammo;

    private UIManager _uiManager;

    [SerializeField]
    private AudioClip _laserAudio;
    [SerializeField]
    private AudioClip _ammolessAudio;
    private AudioSource _audioSource;

    Material material;

    [SerializeField]
    private float _baseSpeed = 3f;
    [SerializeField]
    private float _thrusterSpeed = 6f;
    [SerializeField]
    private Transform _thrusterTransform;
    [SerializeField]
    private float _thrusterSizeMultiplier = 1.8f;
    [SerializeField]
    private float _speedPowerupMultiplier = 1.6f;
    [SerializeField]
    private float _fuelAmount = 100f;
    [SerializeField]
    private bool _emptyTank = false;
    [SerializeField]
    private float _fuelLossPerSecond = 10f;
    [SerializeField]
    private float _fuelRechargedPerSecond = 10f;
    [SerializeField]
    private float _fuelRechargedDelay = 3f;
    private bool _canRechargeFuel = false;

    [SerializeField]
    private bool _isNegativeActive;


    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector3(0, 0, 0);
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        _camera = GameObject.Find("Main Camera").GetComponent<CameraShake>();
        _waveManager = GameObject.Find("Wave_Manager").GetComponent<WaveManager>();
        _audioSource = GetComponent<AudioSource>();
        material = _shieldVisualizer.GetComponent<Renderer>().material;

        if (_spawnManager == null)
        {
            Debug.LogError("Spawn Manager is null.");
        }

        if (_uiManager == null)
        {
            Debug.LogError("The UI Manager is null.");
        }

        if (_audioSource == null)
        {
            Debug.LogError("AudioSource on the player is null.");
        }
        if (_camera == null)
        {
            Debug.LogError("Camera is null.");
        }
        if (_waveManager == null)
        {
            Debug.LogError("WaveManager is null.");
        }
        else
        {
            _audioSource.clip = _laserAudio;
        }
    }

    // Update is called once per frame
    void Update()
    {
        CalculateMovement();

        if (Input.GetKeyDown(KeyCode.Space) && Time.time > _canFire && _ammo > 0 && _isNegativeActive == false)
        {
            FireLaser();
        }
        else if (Input.GetKeyDown(KeyCode.Space) && _ammo <= 0)
        {
            AudioSource.PlayClipAtPoint(_ammolessAudio, transform.position);

        }
    }

    void CalculateMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 direction = Vector3.Normalize(new Vector3(horizontalInput, verticalInput, 0f));

        bool isMoving = direction.magnitude >= .01f;
        

        float speed = CheckThrusters(isMoving);

        if (_isSpeedBoostActive == true)
        {
            speed *= _speedPowerupMultiplier;
        }
        else
        {
            speed /= _speedPowerupMultiplier;
        }

        transform.position += direction * speed * Time.deltaTime;
      
        if (transform.position.y >= 0)
        {
            transform.position = new Vector3(transform.position.x, 0, 0);
        }
        else if (transform.position.y <= -3.8f)
        {
            transform.position = new Vector3(transform.position.x, -3.8f, 0);
        }

        if (transform.position.x >= 11.3f)
        {
            transform.position = new Vector3(-11.3f, transform.position.y, 0);
        }
        else if (transform.position.x <= -11.3f)
        {
            transform.position = new Vector3(11.3f, transform.position.y, 0);
        }
    }

    void FireLaser()
    {
        _canFire = Time.time + _fireRate;

        if (_isTripleShotActive == true)
        {
            Instantiate(_tripleShotPrefab, transform.position, Quaternion.identity);
        }
        else if (_isSecondaryActive == true)
        {
            Instantiate(_secondaryPrefab, transform.position, Quaternion.identity);
        }
        else
        {
            Instantiate(_laserPrefab, transform.position + new Vector3(0, 1.05f, 0), Quaternion.identity);
        }

        _ammo--;
        _uiManager.UpdateAmmo(_ammo);

        _audioSource.Play();
    }

    float CheckThrusters(bool isMoving)
    {
        float speed;
        if (Input.GetKey(KeyCode.LeftShift) && !_emptyTank && isMoving)
        {
            speed = _thrusterSpeed;
            _canRechargeFuel = false;
            _fuelAmount -= _fuelLossPerSecond * Time.deltaTime;
            _thrusterTransform.localScale = Vector3.one * _thrusterSizeMultiplier;
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            speed = _baseSpeed;
            StartCoroutine(RechargeDelay());
        }
        else
        {
            speed = _baseSpeed;
            
            if (_canRechargeFuel == true)
            {
                _fuelAmount += _fuelRechargedPerSecond * Time.deltaTime;
            }

            _thrusterTransform.localScale = Vector3.one;
        }

        if (_fuelAmount <= 0)
        {
            _fuelAmount = 0;
            _emptyTank = true;
        }
        else if (_fuelAmount >= 100)
        {
            _fuelAmount = 100;
            _emptyTank = false;
        }

        _uiManager.UpdateFuelGauge(_fuelAmount);

        return speed;
    }

    IEnumerator RechargeDelay()
    {
        yield return new WaitForSeconds(_fuelRechargedDelay);
        _canRechargeFuel = true;
    }

    void CheckEngineDamage()
    {
        if (_lives == 3)
        {
            _rightDamageVisualizer.SetActive(false);
            _leftDamageVisualizer.SetActive(false);
        }
        else if (_lives == 2)
        {
            _rightDamageVisualizer.SetActive(true);
            _leftDamageVisualizer.SetActive(false);
        }
        else if (_lives == 1)
        {
            _rightDamageVisualizer.SetActive(true);
            _leftDamageVisualizer.SetActive(true);
        }
    }

    public void Damage()
    {
        if (_isShieldsActive == true)
        {
            _shieldLives--;
            Color temp = material.color;
            switch (_shieldLives)
            {
                case 0:
                    material.color = new Color(temp.r, temp.g, temp.b, 1);
                    break;
                case 1:
                    material.color = new Color(temp.r, temp.g, temp.b, .5f);
                    break;
                case 2:
                    material.color = new Color(temp.r, temp.g, temp.b, .7f);
                    break;
            }
           
            if (_shieldLives <= 0)
            {
                _isShieldsActive = false;
                _shieldVisualizer.SetActive(false);
            }
            return;
        }
        _lives--;
        StartCoroutine(_camera.ShakeCamera(0.5f, 0.3f));

        CheckEngineDamage();

        _uiManager.UpdateLives(_lives);

        if (_lives < 1)
        {
            _spawnManager.OnPlayerDeath();
            Destroy(this.gameObject);
        }
    }

    public void TripleShotActive()
    {
        _isTripleShotActive = true;
        StartCoroutine(TripleShotPowerDownRoutine());
    }

    IEnumerator TripleShotPowerDownRoutine()
    {
        yield return new WaitForSeconds(5.0f);
        _isTripleShotActive = false;
    }

    public void SecondaryFire()
    {
        _isSecondaryActive = true;
        _playerLeft.SetActive(true);
        _playerRight.SetActive(true);
        StartCoroutine(SecondaryCoolDown());
    }

    IEnumerator SecondaryCoolDown()
    {
        yield return new WaitForSeconds(5.0f);
        _isSecondaryActive = false;
        _playerLeft.SetActive(false);
        _playerRight.SetActive(false);
    }

    public void SpeedBoostActive()
    {
        _isSpeedBoostActive = true;
        StartCoroutine(SpeedBoostPowerDownRoutine());
    }

    IEnumerator SpeedBoostPowerDownRoutine()
    {
        yield return new WaitForSeconds(5.0f);
        _isSpeedBoostActive = false;
    }

    public void ShieldsActive()
    {
        _isShieldsActive = true;
        _shieldVisualizer.SetActive(true);
    }

    IEnumerator CannotFire()
    {
        _isNegativeActive = true;
        yield return new WaitForSeconds(5.0f);
        _isNegativeActive = false;
    }

    public void NegativeEnabled()
    {
        StartCoroutine(CannotFire());
    }

    IEnumerator ResetPowerups()
    {
        yield return new WaitForFixedUpdate();
    }

    public void Reset()
    {
        if (_isNegativeActive == true && _isSecondaryActive == true && _isShieldsActive == true && _isSpeedBoostActive == true && _isTripleShotActive == true)
        {
            StopCoroutine(ResetPowerups());
        }
    }

    public void AddScore(int points)
    {
        _score += points;
        _uiManager.UpdateScore(_score);
        _waveManager.AddWave();
    }

    public int GetScore()
    {
        return _score;
    }

    public void AddAmmo(int bullets)
    {
        if (bullets >= _ammo)
        {
            _ammo = 15;
        }
        else
        {
            _ammo += bullets;
        }
        
        _uiManager.UpdateAmmo(_ammo);
    }

    public void AddHealth()
    {
        if (_lives < 3)
        {
            _lives++;
            _uiManager.UpdateLives(_lives);
        }

        CheckEngineDamage();
    }
}
