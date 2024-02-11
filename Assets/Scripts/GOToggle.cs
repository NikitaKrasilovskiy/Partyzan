using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GOToggle : MonoBehaviour
{
    [SerializeField]
    GameObject g;

    public bool b;

	void Start ()
    { g.SetActive(b); }
}