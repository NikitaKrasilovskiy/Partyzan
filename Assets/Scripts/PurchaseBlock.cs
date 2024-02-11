using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PurchaseBlock : MonoBehaviour
{
	[SerializeField]
	GameObject obj;

	public void SetActiveBlock()
	{
		obj.SetActive(true);
		StartCoroutine(Deactivate());
	}

	IEnumerator Deactivate()
	{
		yield return new WaitForSeconds(5);
		obj.SetActive(false);
	}
}