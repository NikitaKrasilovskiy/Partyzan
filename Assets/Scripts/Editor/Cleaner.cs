using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class Cleaner : MonoBehaviour
{
    [MenuItem("CheatData/Clear UserData")]

    public static void Clean()
    {
        Directory.Delete(Application.persistentDataPath,true);
        PlayerPrefs.DeleteAll();
    }

    [MenuItem("CheatData/Clear PlayerPrefs")]
    public static void CleanPrefs()
    { PlayerPrefs.DeleteAll(); }
}