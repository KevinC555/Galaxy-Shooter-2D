using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private float _speed = 4.0f;
    private float _startingXPos;
    [SerializeField]
    private GameObject _laserPrefab;

    private Player _player;

    private Animator _anim;
    private AudioSource _audioSource;
    private float _fireRate = 3.0f;
    private float _canFire = -1;

    [SerializeField]
    private GameObject _shields;
    private bool _isShieldsActive;
    private int _shieldChance;
    private int _shieldPower;

    private float _distance;
    [SerializeField]
    private float _ramSpeed = 1.5f;
    private float _attackRange = 4.0f;
    private float _ramMultiplier = 2.0f;

    private float _rayDistance = 8.0f;
    [SerializeField]
    float _rayCastRad = 0.5f;

    void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        _audioSource = GetComponent<AudioSource>();

        _isShieldsActive = false;

        if (_shields != null)
        {
            _shields.SetActive(false);
            ShieldCheck();
        }

        _startingXPos = transform.position.x;

        if (_player == null)
        {
            Debug.LogError("Player is NULL.");
        }

        _anim = GetComponent<Animator>();

        if (_anim == null)
        {
            Debug.LogError("Animator is NULL.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        CalculateMovement();
        //ZigZagMovement();Debugging
        //RamPlayer(); Debugging
        BackAttack();
        Fire();
        DestroyPowerup();
    }

    void Fire()
    {
        if (Time.time > _canFire)
        {
            _fireRate = Random.Range(3f, 7f);
            _canFire = Time.time + _fireRate;
            GameObject enemyLaser = Instantiate(_laserPrefab, transform.position, Quaternion.identity);
            Laser[] lasers = enemyLaser.GetComponentsInChildren<Laser>();

            for (int i = 0; i < lasers.Length; i++)
            {
                lasers[i].AssignEnemyLaser();
            }
        }
    }

    private void DestroyPowerup()
    {
        _rayCastRad = 2.5f;
        RaycastHit2D hit = Physics2D.CircleCast(transform.position, _rayCastRad, Vector2.down, _rayDistance, LayerMask.GetMask("collectible"));

        Debug.DrawRay(transform.position, Vector3.down * _rayCastRad * _rayDistance, Color.red);

        if (hit.collider != null)
        {
            if (hit.collider.CompareTag("Powerup") && Time.time > _canFire)
            {
                Debug.Log("Powerup Detected");
                FireAtPowerup();
            }
        }
    }

    private void FireAtPowerup()
    {
        _fireRate = Random.Range(2f, 5f);
        _canFire = Time.time + _fireRate;
        GameObject enemyLaser = Instantiate(_laserPrefab, transform.position, Quaternion.identity);
        Laser[] lasers = enemyLaser.GetComponentsInChildren<Laser>();

        for (int i = 0; i < lasers.Length; i++)
        {
            lasers[i].AssignEnemyLaser();
        }
    }

    void CalculateMovement()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);

        if (transform.position.y < -8f)
        {
            transform.position = new Vector3(Random.Range(-8f, 8f), 7, 0);
        }
    }

    void ZigZagMovement()
    {
        if (transform.position.y < 4)
        {
            if (_startingXPos > 0)
            {
                transform.Translate(Vector3.left * _speed * Time.deltaTime);
            }

            else if (_startingXPos <= 0)
            {
                transform.Translate(Vector3.right * _speed * Time.deltaTime);
            }
        }
    }

    void RamPlayer()
    {
        _distance = Vector3.Distance(_player.transform.position, this.transform.position);

        if (_distance <= _attackRange)
        {
            Vector3 direction = this.transform.position - _player.transform.position;
            direction = direction.normalized;
            this.transform.position -= direction * Time.deltaTime * (_ramSpeed * _ramMultiplier);
        }
    }

    private void BackAttack()
    {
        RaycastHit2D hit = Physics2D.CircleCast(transform.position, _rayCastRad, Vector2.up, _rayDistance, LayerMask.GetMask("Player"));

        if (hit.collider != null)
        {
            if (hit.collider.CompareTag("Player") && Time.time > _canFire)
            {
                Debug.Log("Player detected");
                FireLaserBack();
            }
        }
    }

    private void FireLaserBack()
    {
        _fireRate = Random.Range(2f, 5f);
        _canFire = Time.time + _fireRate;
        GameObject enemyLaser = Instantiate(_laserPrefab, transform.position, Quaternion.Euler(transform.rotation.x, transform.rotation.y, 180.0f));
        Laser[] lasers = enemyLaser.GetComponentsInChildren<Laser>();

        for (int i = 0; i < lasers.Length; i++)
        {
            lasers[i].AssignEnemyLaser();
        }
    }

    private void ShieldIsActive()
    {
        _shieldPower = 1;
        _isShieldsActive = true;
        _shields.SetActive(true);
    }

    private void ShieldCheck()
    {
        _shieldChance = Random.Range(0, 4);

        if (_shieldChance <= 0) 
        {
            ShieldIsActive();
        }
    }

    public void DamageEnemy()
    {
        if (_isShieldsActive == true)
        {
            _shieldPower--;
            _shields.SetActive(false);
            _isShieldsActive = false;
            return;
        }
        else
        {
            _anim.SetTrigger("OnEnemyDeath");
            _speed = 0;
            _audioSource.Play();
            if (_player != null)
            {
                _player.AddScore(10);
            }
            Destroy(GetComponent<Collider2D>());
            Destroy(this.gameObject, 2.8f);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            Player player = other.transform.GetComponent<Player>();

            if (player != null)
            {
                player.Damage();
            }
            DamageEnemy();
        }

        if (other.tag == "Laser")
        {
            Laser laser = other.transform.GetComponent<Laser>();

            if (laser != null && laser.IsEnemyLaser() == false)
            {
                Destroy(other.gameObject);
                DamageEnemy();
            }
        }
    }
}
