using CCGKit;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TasksWindow : MonoBehaviour
{
    public List<TaskBlock> taskBlocks;
    //public GameObject rewardButton;

    public TextMeshProUGUI goldText;
    public PreloaderAmination preloader;

    public GameObject rewardWindow;
    public TranslatorScript rewardMessage;

    public void OpenIfNeed()
    {
        if (HomeScene.needTasksShow)
        {
            HomeScene.needTasksShow = false;
            OpenWindow();
        }
    }

    public void OpenWindow()
    {
        //Debug.Log("Open window");
        gameObject.SetActive(true);
        StartCoroutine(UpdateValues());
        UpdateValues();
    }

    private IEnumerator UpdateValues()
    {
        Model model = GameManager.Instance.model;

        for (int i = 0; i < taskBlocks.Count; i++)
        { taskBlocks[i].gameObject.SetActive(false); }

        //rewardButton.SetActive(false);
        preloader.Activate();
        yield return model.getTasks();

        ShowTasks();
        //rewardButton.SetActive(model.user.canGetTasksReward());
        preloader.Deactivate();
    }

    void ShowTasks()
    {
        Model model = GameManager.Instance.model;

        for (int i = 0; i < taskBlocks.Count; i++)
        {
            taskBlocks[i].gameObject.SetActive(true);

            if (i < model.user.tasks.Count)
            { taskBlocks[i].showTask(model.user.tasks[i]); }
            else
            { taskBlocks[i].hideTask(); }
        }
    }

    public void getReward()
    { StartCoroutine(getRewardProcess()); }

    public void getReward2(int i)
    { StartCoroutine(getRewardProcess2(i)); }

    IEnumerator getRewardProcess()
    {
        //rewardButton.SetActive(false);
        Model model = GameManager.Instance.model;
        yield return model.getTasksReward();

        if (model.tasksReward.gold != 0) goldText.text = model.tasksReward.gold.ToString();

        if (model.tasksReward.goldDelta > 0)
        {
            rewardMessage.UpdateText();
            rewardMessage.SetValue(model.tasksReward.goldDelta);
            rewardWindow.SetActive(true);
            
        }
    }

    IEnumerator getRewardProcess2(int i)
    {
        taskBlocks[i].rewardButton.SetActive(false);
        Model model = GameManager.Instance.model;
        yield return model.getTasksReward2(i);

        if (model.tasksReward.gold != 0) goldText.text = model.tasksReward.gold.ToString();

        if (model.tasksReward.goldDelta > 0)
        {
            rewardMessage.SetValue(model.tasksReward.goldDelta);
            rewardWindow.SetActive(true);
        }

        ShowTasks();
    }
}