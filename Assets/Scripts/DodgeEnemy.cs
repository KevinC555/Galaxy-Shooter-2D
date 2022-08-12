using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DodgeEnemy : MonoBehaviour
{
    [SerializeField]
    private float _speed = 4.0f;

    private Player _player;

    private Animator _anim;
    private AudioSource _audioSource;

    private int _moveDirection;
    private bool _canDodge = true;

    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        _audioSource = GetComponent<AudioSource>();

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
        DodgeShot();
    }

    private void CalculateMovement()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);

        if (transform.position.y < -8f)
        {
            transform.position = new Vector3(Random.Range(-8f, 8f), 7, 0);
        }
    }

    private void DodgeShot()
    {
        RaycastHit2D hit = Physics2D.CircleCast(transform.position, 2f, Vector2.down, 8.0f);
        Debug.DrawRay(transform.position, Vector3.down * 8.0f, Color.red);

        if (hit.collider != null)
        {
            if (hit.collider.CompareTag("Laser") && _canDodge == true)
            {
                _moveDirection = Random.Range(0, 2) == 0 ? -2 : 2;
                transform.position = new Vector3(transform.position.x - _moveDirection, transform.position.y, transform.position.z);
                _canDodge = false;
            }
        }
    }

    private void DamageEnemy()
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

        if (other.tag == "Projectile")
        {
            Destroy(other.gameObject);
            DamageEnemy();
        }
    }
}
