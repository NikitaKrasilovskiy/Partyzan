using CCGKit;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TaskBlock : MonoBehaviour
{
    public List<GameObject> images;
    public TranslatorScript description;
    public TextMeshProUGUI count;
    public List<GameObject> diffs;
    public GameObject complete;
    public TextMeshProUGUI timer;
    public GameObject rewardButton;
    private int taskId;
    public int number;

    void Start()
    { Update(); }

    private int time;

    void Update()
    {
        time = GameManager.Instance.model.remainTaskTime(number);
        string ts = "";

        if (time > 0)
        {
            int h = time / 3600;
            int m = (time / 60) % 60;
            int s = time % 60;
            ts = h.ToString("D2") + ":" + m.ToString("D2") + ":" + s.ToString("D2");
        }

        if (ts != "")
        {
            description.gameObject.SetActive(false);
            count.gameObject.SetActive(false);
        }
        else
        { description.gameObject.SetActive(true); }

        timer.SetText(ts);
    }

    public void showTask(Task task)
    {
        Update();
        gameObject.SetActive(true);

        for (int i = 0; i < images.Count; i++) images[i].SetActive((i + 1) == task.type);

        if (!description.gameObject.activeInHierarchy)
        { description.gameObject.SetActive(true); }
        description.SetText(task.description);

        if (task.need <= 1)
        { count.gameObject.SetActive(false); }
        else
        {
            if (task.complete())
            { count.gameObject.SetActive(false); }
            else
            {
                count.gameObject.SetActive(true);
                count.text = "(" + (task.need - task.value).ToString() + ")";
            }
        }

        // for (int i = 0; i < diffs.Count; i++) diffs[i].SetActive(i == task.diff);

        if (time > 0)
        { complete.SetActive(true); }
        else
        { complete.SetActive(false); }

        rewardButton.SetActive( task.complete() && ( time == 0 ) );
        taskId = task.type;
        timer.text = "";
    }

    public void hideTask()
    { gameObject.SetActive(false); }

    public int getTaskId()
    { return taskId; }
}