using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewMessageScript : MonoBehaviour
{
    void Start()
    { CloseWindow(); }

    public void OpenWindow()
    {
        gameObject.SetActive(true);
        updateData();
    }

    public void CloseWindow()
    { gameObject.SetActive(false); }

    void updateData()
    { }
}