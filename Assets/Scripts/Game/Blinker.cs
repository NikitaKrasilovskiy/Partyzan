using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blinker : MonoBehaviour
{
    Vector3 baseScale;

    float t = 0;

	void Start ()
    { baseScale = gameObject.transform.localScale; }

	void Update ()
    {
        t += Time.deltaTime;

        gameObject.transform.localScale = baseScale * (1 + 0.1f * Mathf.Sin(t * 5));
    }
}