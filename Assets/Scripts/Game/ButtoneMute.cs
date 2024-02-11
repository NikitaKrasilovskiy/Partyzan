using CCGKit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtoneMute : MonoBehaviour
{
    [SerializeField]
    GameObject muteIcone;

    [SerializeField]
    AudioSource aus;

    private void OnEnable()
    { UpdateView(); }

    void UpdateView()
    { muteIcone.SetActive(AudioListener.volume == 0); }

    public void Mute()
    {
        if(AudioListener.volume == 0)
            AudioListener.volume = 1;
        else
            AudioListener.volume = 0;
      // GameManager.Instance.music.mute = !GameManager.Instance.music.mute;
      //  aus.mute = !aus.mute;
        UpdateView();
    }
}