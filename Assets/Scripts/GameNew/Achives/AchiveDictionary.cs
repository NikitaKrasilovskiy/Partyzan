using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchiveDictionary : MonoBehaviour
{
    [SerializeField]
    Sprite[] achiveDictionary;

    string[] discriptStrings;

    private void Awake()
    { Load(); }

    private void Load()
    {
        discriptStrings = (Resources.Load("tasks") as TextAsset).text.Split(',');

        Debug.Log("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA "+discriptStrings.Length);
    }

    public Sprite GetSpriteByID(int id)
    { return achiveDictionary[id]; }

    public string GetDiscription(int id)
    {
        if (discriptStrings == null)
        { Load(); }

        int n = discriptStrings.Length;

        return discriptStrings[id];
    }
}