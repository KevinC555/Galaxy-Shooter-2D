using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMissile : MonoBehaviour
{
    [SerializeField]
    private float _missileSpeed = 3.0f;
    private float _minDistance;
    private Vector3 _currentPosition;
    private GameObject _enemyTarget;
    [SerializeField]
    private GameObject[] _availableEnemies;

    // Start is called before the first frame update
    void Start()
    {
        GetNearestEnemy();
    }

    // Update is called once per frame
    void Update()
    {
        HomeInOnTarget();
    }

    private GameObject GetNearestEnemy()
    {
        _availableEnemies = GameObject.FindGameObjectsWithTag("Enemy");
        _currentPosition = this.transform.position;
        _minDistance = Mathf.Infinity;

        foreach(GameObject enemy in _availableEnemies)
        {
            float distance = Vector3.Distance(enemy.transform.position, _currentPosition);

            if (distance < _minDistance)
            {
                _enemyTarget = enemy;
                _minDistance = distance;
            }
        }

        return _enemyTarget;
    }

    private void HomeInOnTarget()
    {
        if (_enemyTarget != null)
        {
            if (Vector3.Distance(transform.position, _enemyTarget.transform.position) != 0)
            {
                transform.position = Vector2.MoveTowards(transform.position, _enemyTarget.transform.position, _missileSpeed * Time.deltaTime);

                Vector2 direction = (transform.position - _enemyTarget.transform.position).normalized;
                var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                var offset = 90f;

                transform.rotation = Quaternion.Euler(Vector3.forward * (angle + offset));
            }
        }
        else
        {
            _enemyTarget = GetNearestEnemy();
        }
    }
}
