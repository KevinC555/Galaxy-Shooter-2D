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
    private float _speed = 3.5f;
    private float _speedMultiplier = 2;
    [SerializeField]
    private GameObject _laserPrefab;
    [SerializeField]
    private GameObject _tripleShotPrefab;
    [SerializeField]
    private float _fireRate = 0.5f;
    private float _canFire = -1f;
    [SerializeField]
    private int _lives = 3;
    private SpawnManager _spawnManager;
    
    private bool _isTripleShotActive = false;
    private bool _isSpeedBoostActive = false;
    private bool _isShieldsActive = false;
    private int _shieldLives = 3;

    [SerializeField]
    private GameObject _shieldVisualizer;
    [SerializeField]
    private GameObject _rightDamageVisualizer;
    [SerializeField]
    private GameObject _leftDamageVisualizer;

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


    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector3(0, 0, 0);
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
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
        else
        {
            _audioSource.clip = _laserAudio;
        }
    }

    // Update is called once per frame
    void Update()
    {
        CalculateMovement();

        Thrusters();

        if (Input.GetKeyDown(KeyCode.Space) && Time.time > _canFire && _ammo > 0)
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

        Vector3 direction = new Vector3(horizontalInput, verticalInput, 0);

        if (_isSpeedBoostActive)
        {
            transform.Translate(direction * _speed * _speedMultiplier * Time.deltaTime);
        }
        else
        {
            transform.Translate(direction * _speed * Time.deltaTime);
        }
      
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
        else
        {
            Instantiate(_laserPrefab, transform.position + new Vector3(0, 1.05f, 0), Quaternion.identity);
        }

        _ammo--;
        _uiManager.UpdateAmmo(_ammo);

        _audioSource.Play();
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

    public void AddScore(int points)
    {
        _score += points;
        _uiManager.UpdateScore(_score);
    }

    public void AddAmmo(int bullets)
    {
        _ammo += bullets;
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

    void Thrusters()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            _speed = 7.0f;
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            _speed = 5.0f;
        }
    }
}
