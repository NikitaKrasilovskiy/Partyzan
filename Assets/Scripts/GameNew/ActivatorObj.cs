using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivatorObj : MonoBehaviour
{
    [SerializeField]
    GameObject obj;

	void Start ()
    {
        if(!PlayerPrefs.HasKey("lang"))
        obj.SetActive(true);
	}
}