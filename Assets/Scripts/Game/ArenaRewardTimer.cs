using TMPro;
using UnityEngine;
using System;
using CCGKit;

public class ArenaRewardTimer : MonoBehaviour
{
    [NonSerialized] int timer = 3 * 3600 + 34 * 60 + 18;
    [NonSerialized] float t;
    [NonSerialized] string ts;
    TextMeshProUGUI text;

    void Start()
    {
        text = gameObject.GetComponent<TextMeshProUGUI>();
        t = 0;
        updateTimerText();
    }

    void Update()
    {
        Model model = GameManager.Instance.model;
        t -= Time.deltaTime;
        if (t < 0)
        {
            t += 1;
            timer = model.remainArenaReward();
            updateTimerText();
        }
    }

    private void updateTimerText()
    {
        int h = timer / 3600;
        int m = (timer / 60) % 60;
        int s = timer % 60;
        ts = h.ToString("D2") + ":" + m.ToString("D2") + ":" + s.ToString("D2");
        text.SetText(ts);
    }
}