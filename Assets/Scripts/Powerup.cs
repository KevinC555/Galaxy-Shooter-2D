using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour
{
    [SerializeField]
    private float _speed = 3.0f;
    [SerializeField]
    private int powerupID;

    private GameObject Player;
    private float _magnetSpeed = 5.0f;

    [SerializeField]
    private AudioClip _clip;

    private void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");

        if (Player == null)
        {
            Debug.LogError("The Player is null.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);

        if (transform.position.y < -6)
        {
            Destroy(this.gameObject);
        }

        if (Input.GetKey(KeyCode.C))
        {
            Magnet();
        }
    }

    private void Magnet()
    {
        transform.position = Vector3.Lerp(this.transform.position, Player.transform.position, _magnetSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            Player player = other.transform.GetComponent<Player>();

            AudioSource.PlayClipAtPoint(_clip, transform.position);

            if (player != null)
            {
                switch (powerupID)
                {
                    case 0:
                        player.TripleShotActive();
                        break;
                    case 1:
                        player.SpeedBoostActive();
                        break;
                    case 2:
                        player.ShieldsActive();
                        break;
                    case 3:
                        player.AddAmmo(15);
                        break;
                    case 4:
                        player.AddHealth();
                        break;
                    case 5:
                        other.transform.GetComponent<Player>().SecondaryFire();
                        Destroy(this.gameObject);
                        break;
                    case 6:
                        player.NegativeEnabled();
                        break;
                    default:
                        Debug.Log("Default Value");
                        break;
                }
            }

            Destroy(this.gameObject);
        }

        if (other.tag == "Laser")
        {
            Destroy(other.gameObject);
            Destroy(this.gameObject);
        }
    }
}
