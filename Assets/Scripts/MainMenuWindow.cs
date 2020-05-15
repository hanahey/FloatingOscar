using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuWindow : MonoBehaviour
{
    private void Awake()
    {
        transform.Find("playButton").GetComponent<Button>().onClick.AddListener(delegate { PlayButtonClicked(); });
        transform.Find("quitButton").GetComponent<Button>().onClick.AddListener(delegate { QuitButtonClicked(); });
    }

    private void PlayButtonClicked()
    {
        Loader.Load(Loader.Scene.GameScene);
        SoundManager.PlaySound(SoundManager.Sound.ButtonClick);
    }

    private void QuitButtonClicked()
    {
        Application.Quit();
        SoundManager.PlaySound(SoundManager.Sound.ButtonClick);
    }
}
