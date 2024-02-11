using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CCGKit;

public class Loginer : MonoBehaviour
{
	IEnumerator Start ()
    {
        if(PlayerPrefs.HasKey("login") && PlayerPrefs.HasKey("password"))
            yield return GameManager.Instance.model.login(PlayerPrefs.GetString("login"), PlayerPrefs.GetString("password"));
    }
}