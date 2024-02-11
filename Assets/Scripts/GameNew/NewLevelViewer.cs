using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CCGKit;

public class NewLevelViewer : MonoBehaviour
{
    [SerializeField]
    GameObject newLevelWindow;

    void Start ()
    {
        if (NewLevelWindow.mastShow)
        {
            Debug.Log(GameManager.Instance.model.upLevelReward);
            GameManager.Instance.model.upLevelReward.Reverse();

            foreach (Level l in GameManager.Instance.model.upLevelReward)
            {
                Debug.Log(l);
                GameObject go = Instantiate(newLevelWindow);
                go.SetActive(true);
                go.GetComponent<NewLevelWindow>().Show(l);
                go.transform.SetParent(newLevelWindow.transform.parent);
                go.transform.localScale = newLevelWindow.transform.localScale;
                Vector3 v = go.transform.position;
                v.z = 0;
                go.transform.position = v;
            }

            GameManager.Instance.model.upLevelReward.Clear();
            NewLevelWindow.mastShow = false;
        }
    }
}