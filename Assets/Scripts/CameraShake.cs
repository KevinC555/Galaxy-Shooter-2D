using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    private Vector3 _originalPosition;

    // Start is called before the first frame update
    void Start()
    {
        _originalPosition = transform.position;
    }

    public IEnumerator ShakeCamera (float duration, float strength)
    {
        float _elapsed = 0.0f;

        while (_elapsed < duration)
        {
            float xPos = Random.Range(-1f, 1f) * strength;
            float yPos = Random.Range(-1f, 1f) * strength;

            transform.position = new Vector3(xPos, yPos, _originalPosition.z);

            _elapsed += Time.deltaTime;

            yield return null;
        }

        transform.position = _originalPosition;
    }
}
