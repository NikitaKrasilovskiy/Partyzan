using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CCGKit;
using UnityEngine.SceneManagement;
using System.IO;

public class ReloadTutorial : MonoBehaviour
{
    public static bool fakeDevice;

	void Update ()
    {
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.T))
        {
            StartCoroutine(Reload());
        }
    }

    IEnumerator Reload()
    {
            fakeDevice = true;
            yield return true;
    }
}