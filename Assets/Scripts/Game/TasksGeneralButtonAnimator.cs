using CCGKit;
using System;
using UnityEngine;

public class TasksGeneralButtonAnimator : MonoBehaviour
{
    [NonSerialized]
    float t;

    void Start ()
    { t = 0; }

    void Update ()
    {
        if (GameManager.Instance.model.haveGeneralReward)
        {
            t += Time.deltaTime;
            float a = Mathf.Sin(2 * Mathf.PI * t);
            transform.localScale = new Vector3((1 + a * 0.2f) * 1.55f, (1 + a * 0.2f) * 1.55f, 1);
        }
    }
}