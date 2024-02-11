using TMPro;
using UnityEngine;
using System;
using CCGKit;

public class BackpackTimer : MonoBehaviour
{
    [NonSerialized]
    int timer = 3 * 3600 + 34 * 60 + 18;

    [NonSerialized]
    float t;

    [NonSerialized]
    string ts;

    public GameObject newBackpack;

    public GameObject button;
    public GameObject glow;
    public TextMeshProUGUI text;

    void Start()
    {
        t = 1;
        updateTimerText();
    }

    void Update()
    {
        t -= Time.deltaTime;
        if (t < 0)
        {
            t += 1;
            timer = GameManager.Instance.model.remainBackpackTime();
            updateTimerText();
        }
        if (timer <= 0)
        {
            float a = Mathf.Sin(2 * Mathf.PI * t);
            newBackpack.transform.localScale = new Vector3(1 + a * 0.2f, 1 + a * 0.2f, 1);
        }
    }

    private void updateTimerText()
    {
        if (timer > 0)
        {
            newBackpack.SetActive(false);
            button.SetActive(true);
            glow.SetActive(true);
            text.gameObject.SetActive(true);

            int h = timer / 3600;
            int m = (timer / 60) % 60;
            int s = timer % 60;
            ts = h.ToString("D2") + ":" + m.ToString("D2") + ":" + s.ToString("D2");
            text.SetText(ts);
        }
        else
        {
            newBackpack.SetActive(true);
            button.SetActive(false);
            glow.SetActive(false);
            text.gameObject.SetActive(false);
        }
    }
}