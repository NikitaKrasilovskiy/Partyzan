using CCGKit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TaskGeneralScript : MonoBehaviour
{
    public GameObject Back1;
    public GameObject Back2;
    public GameObject Back3;

    public GameObject Complete;
    public GameObject NotComplete;

    public TranslatorScript description;
    public TranslatorScript progress;

    public TranslatorScript coinsRewardComplete;
    public TranslatorScript coinsRewardNotComplete;

    public GameObject GetRewardButton;
    public GameObject CoinsCount;

    private TaskGeneralProcess taskGeneralProcess;
    public PreloaderAmination preloader;

    [SerializeField]
    Image icon;

    [SerializeField]
    AchiveDictionary achiveDictionary;

    public void Show (TaskGeneralProcess taskGeneralProcess)
    {
        Back1.SetActive(taskGeneralProcess.level == 1);
        Back2.SetActive(taskGeneralProcess.level == 2);
        Back3.SetActive(taskGeneralProcess.level == 3);

#if UNITY_EDITOR
        int i = taskGeneralProcess.state;

        if (Input.GetKey(KeyCode.F5))
            i = 1;
        switch (i) {
            case TaskGeneralProcess.TASK_GENERAL_STATE_NOT_COMPLETE:
            NotComplete.SetActive(true);
            Complete.SetActive(false);
            GetRewardButton.SetActive(false);
            CoinsCount.SetActive(true);
            break;
            case TaskGeneralProcess.TASK_GENERAL_STATE_HAVE_REWARD:
            NotComplete.SetActive(false);
            Complete.SetActive(true);
            GetRewardButton.SetActive(true);
            CoinsCount.SetActive(false);
            break;
            case TaskGeneralProcess.TASK_GENERAL_STATE_COMPLETE:
            NotComplete.SetActive(false);
            Complete.SetActive(true);
            GetRewardButton.SetActive(false);
            CoinsCount.SetActive(true);
            break;
        }
#else

switch (taskGeneralProcess.state)
        {
            case TaskGeneralProcess.TASK_GENERAL_STATE_NOT_COMPLETE:
            NotComplete.SetActive(true);
            Complete.SetActive(false);
            GetRewardButton.SetActive(false);
            CoinsCount.SetActive(true);
            break;
            case TaskGeneralProcess.TASK_GENERAL_STATE_HAVE_REWARD:
            NotComplete.SetActive(false);
            Complete.SetActive(true);
            GetRewardButton.SetActive(true);
            CoinsCount.SetActive(false);
            break;
            case TaskGeneralProcess.TASK_GENERAL_STATE_COMPLETE:
            NotComplete.SetActive(false);
            Complete.SetActive(true);
            GetRewardButton.SetActive(false);
            CoinsCount.SetActive(true);
            break;
        }
#endif
        icon.sprite = achiveDictionary.GetSpriteByID(taskGeneralProcess.id);
        description.SetText(achiveDictionary.GetDiscription(taskGeneralProcess.id));//taskGeneralProcess.description);
        description.SetValue(taskGeneralProcess.goal);
        progress.SetValue(Mathf.Min(taskGeneralProcess.value, taskGeneralProcess.goal));
        progress.SetValue2(taskGeneralProcess.goal);
        coinsRewardComplete.SetValue(taskGeneralProcess.coins);
        coinsRewardNotComplete.SetValue(taskGeneralProcess.coins);
        this.taskGeneralProcess = taskGeneralProcess;
    }

    public void ClickGetReward()
    { StartCoroutine(getRewardProcess()); }

    private IEnumerator getRewardProcess()
    {
        Model model = GameManager.Instance.model;
        preloader.Activate();
        yield return model.getTaskGeneralReward(taskGeneralProcess.id);
        Show(taskGeneralProcess);
        preloader.Deactivate();
    }
}