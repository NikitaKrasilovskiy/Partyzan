using CCGKit;
using System.Collections;
using UnityEngine;

public class BackpackEditorScene : MonoBehaviour
{
    public BackpackTypeWorker backpackTypeWorker1;
    public BackpackTypeWorker backpackTypeWorker2;
    public BackpackTypeWorker backpackTypeWorker3;

    Model model;

    void Start ()
    {
        model = GameManager.Instance.model;
        StartCoroutine(loadBackpackTypes());
    }

    public void Save()
    {
        model.rules.backpackContent[0] = backpackTypeWorker1.getContent();
        model.rules.backpackContent[1] = backpackTypeWorker2.getContent();
        model.rules.backpackContent[2] = backpackTypeWorker3.getContent();
        StartCoroutine(model.saveBackpackContent());
    }

    IEnumerator loadBackpackTypes()
    {
        if (model.rules.backpackContent == null) yield return model.loadRules();

        backpackTypeWorker1.setContent(model.rules.backpackContent[0]);
        backpackTypeWorker2.setContent(model.rules.backpackContent[1]);
        backpackTypeWorker3.setContent(model.rules.backpackContent[2]);
    }
}