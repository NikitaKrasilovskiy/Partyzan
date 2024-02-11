using CCGKit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelsEditScene : MonoBehaviour
{
    public ProgressLevelScript levelDescGO;
    public GameObject parent;

    Model model;

    void Start ()
    {
        model = GameManager.Instance.model;
        StartCoroutine(updateState());
	}

    List<GameObject> lvls;

    IEnumerator updateState()
    {
        if (model.rules.levels == null) yield return model.loadRules();
        float y = 0;
        lvls = new List<GameObject>();
        for (int i=0;i< model.rules.levels.Count;i++)
        {
            GameObject go = Instantiate(levelDescGO.gameObject);
            go.GetComponent<ProgressLevelScript>().setLevels(model.rules.levels[i]);
            go.transform.SetParent(parent.transform);
            go.transform.localPosition = new Vector3(levelDescGO.transform.localPosition.x, levelDescGO.transform.localPosition.y+y, levelDescGO.transform.localPosition.z);
            go.transform.localScale = Vector3.one;
            y -= 55;
            lvls.Add(go);
        }
        levelDescGO.gameObject.SetActive(false);
    }

    public void Save()
    {
        for (int i = 0; i < lvls.Count; i++)
        { model.rules.levels[i] = lvls[i].GetComponent<ProgressLevelScript>().getData(); }
        StartCoroutine(model.saveLevels());
    }
}