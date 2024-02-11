using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CopyColor : MonoBehaviour
{
    public Image from;

    public Image to;

	void Update ()
    { to.color = from.color; }

}