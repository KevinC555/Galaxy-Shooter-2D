using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    private float _speed = 3f;

    [SerializeField]
    private int _bossHealth;

    private UIManager _uiManager;

    [SerializeField]
    private GameObject _explosionPrefab;
    [SerializeField]
    private GameObject _leftLaserPrefab;
    [SerializeField]
    private GameObject _rightLaserPrefab;
    [SerializeField]
    private GameObject _leftMissilePrefab;
    [SerializeField]
    private GameObject _rightMissilePrefab;

    
    private float _leftLaserfireRate = 3.0f;
    private float _rightLaserfireRate = 3.0f;
    private float _leftMissilefireRate = 3.0f;
    private float _rightMissilefireRate = 3.0f;
    private float _canFireLeftLaser = -1;
    private float _canFireRightLaser = -1;
    private float _canFireLeftMissile = -1;
    private float _canFireRightMissile = -1;

    // Start is called before the first frame update
    void Start()
    {
        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();

        if (_uiManager == null)
        {
            Debug.LogError("The UI Manager is null.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        MoveToStart();
        BossDeath();
        FireLeftLaser();
        FireRightLaser();
        FireLeftMissile();
        FireRightMissile();
    }

    private void MoveToStart()
    {
        if (transform.position.y >= 4.0f)
        {
            transform.Translate(Vector3.down * _speed * Time.deltaTime);
        }
    }

    private void FireLeftLaser()
    {
        if (Time.time > _canFireLeftLaser)
        {
            _leftLaserfireRate = Random.Range(3f, 7f);
            _canFireLeftLaser = Time.time + _leftLaserfireRate;
            GameObject bossLaser = Instantiate(_leftLaserPrefab, transform.position, Quaternion.identity);
            Laser lasers = bossLaser.GetComponentInChildren<Laser>();
            lasers.AssignEnemyLaser();
        }
    }

    private void FireRightLaser()
    {
        if (Time.time > _canFireRightLaser)
        {
            _rightLaserfireRate = Random.Range(3f, 7f);
            _canFireRightLaser = Time.time + _rightLaserfireRate;
            GameObject bossLaser = Instantiate(_rightLaserPrefab, transform.position, Quaternion.identity);
            Laser lasers = bossLaser.GetComponentInChildren<Laser>();
            lasers.AssignEnemyLaser();
        }
    }

    private void FireLeftMissile()
    {
        if (Time.time > _canFireLeftMissile && _bossHealth < 50)
        {
            _leftMissilefireRate = Random.Range(3f, 7f);
            _canFireLeftMissile = Time.time + _leftMissilefireRate;
            Instantiate(_leftMissilePrefab, transform.position, Quaternion.identity);
        }
    }

    private void FireRightMissile()
    {
        if (Time.time > _canFireRightMissile && _bossHealth < 50)
        {
            _rightMissilefireRate = Random.Range(3f, 7f);
            _canFireRightMissile = Time.time + _rightMissilefireRate;
            Instantiate(_rightMissilePrefab, transform.position, Quaternion.identity);
        }
    }

    private void HitByPlayer(int damage)
    {
        _bossHealth -= damage;
        _uiManager.DamageBoss(_bossHealth);
    }

    private void BossDeath()
    {
        if (_bossHealth <= 0)
        {
            Destroy(this.gameObject);
            _uiManager.TurnOffHealthBar();
            Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Laser")
        {
            GameObject newExplosion = Instantiate(_explosionPrefab, other.transform.position, Quaternion.identity);
            newExplosion.transform.localScale = new Vector3(0.5f, 0.5f, 1);
            Destroy(other.gameObject);
            HitByPlayer(5);
        }
        else if (other.tag == "Projectile")
        {
            GameObject newExplosion = Instantiate(_explosionPrefab, other.transform.position, Quaternion.identity);
            newExplosion.transform.localScale = new Vector3(0.5f, 0.5f, 1);
            Destroy(other.gameObject);
            HitByPlayer(10);
        }
    }
}
