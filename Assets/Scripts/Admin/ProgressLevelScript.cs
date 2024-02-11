using CCGKit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressLevelScript : MonoBehaviour
{
    public Text number;
    public InputField points;
    public Dropdown rewardType;
    public InputField rewardCount;

    public void setLevels(Level level)
    {
        number.text = level.number.ToString();
        points.text = level.exp.ToString();

        if (level.rewardType == "") rewardType.value = 0;

        if (level.rewardType == "buster") rewardType.value = 1;

        if (level.rewardType == "backpack") rewardType.value = 2;

        if (level.rewardType == "gold") rewardType.value = 3;

        if (level.rewardType == "premium") rewardType.value = 4;

        rewardCount.text = level.rewardValue.ToString();
    }

    public Level getData()
    {
        Level level = new Level();

        level.number = int.Parse(number.text);
        level.exp = int.Parse(points.text);

        string[] s = {"","buster","backpack","gold","premium"};

        level.rewardType = s[rewardType.value];
        level.rewardValue = int.Parse(rewardCount.text);
        return level;
    }
}