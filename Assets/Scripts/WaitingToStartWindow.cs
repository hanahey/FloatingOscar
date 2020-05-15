using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitingToStartWindow : MonoBehaviour
{
    private void Start()
    {
        Oscar.GetInstance().OnStartPlaying += WaitingToStartWindow_OnStartPlaying;
    }

    private void WaitingToStartWindow_OnStartPlaying(object sender, EventArgs e)
    {
        Hide();
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }
}
