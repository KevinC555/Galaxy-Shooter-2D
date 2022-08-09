using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingMissile : MonoBehaviour
{
    private Transform _target;
    [SerializeField]
    private float _speed = 3f;
    private Player _player;

    private Rigidbody2D rb;
    private float _rotateSpeed = 200f;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        _player = GameObject.Find("Player").GetComponent<Player>();
        _target = _player.transform;

        if (_player == null)
        {
            Debug.LogError("Player is null.");
        }

        if (rb == null)
        {
            Debug.LogError("Rigidbody is null.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 direction = (Vector2)_target.position - rb.position;

        direction.Normalize();

        float _rotateAmount = Vector3.Cross(direction, transform.up).z;

        rb.angularVelocity = -_rotateAmount * _rotateSpeed;

        rb.velocity = transform.up * _speed;

        Destroy(this.gameObject, 5f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            Player player = other.GetComponent<Player>();

            if (player != null)
            {
                player.Damage();
            }
            Destroy(this.gameObject);
        }
    }
}
