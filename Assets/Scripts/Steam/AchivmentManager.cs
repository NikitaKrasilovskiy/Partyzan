using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;

public class AchivmentManager : MonoBehaviour
{
	public static AchivmentManager Instance
	{
		get;
		private set;
	}

	 private void Awake()
	{ Instance=this; }
}