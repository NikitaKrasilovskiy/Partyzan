using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ServerSide;
using static Hub;

public class SteamPrice : MonoBehaviour
{
    public static List<string> price = new List<string>();

	IEnumerator Start ()
    {
        // log("==================steam");
        yield return ServerData.Instance.SteamPriceRequest();
	}
	
    void AddProducts(string s)
    {
        if (s.Contains("error"))
        { return; }
        foreach (string line in s.Split('\n'))
        {
            price.Add(line);
            Debug.Log("!!!!!  "+line);
        }
    }

	void Update ()
    { }
}