using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixTutorStep : MonoBehaviour
{
    [SerializeField]
    GameObject g;

	void Start ()
	{
        g.transform.SetParent(transform);
        g.transform.SetAsLastSibling();
	}
}