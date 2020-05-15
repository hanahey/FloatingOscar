using System;
using UnityEngine;
using UnityEngine.UI;

public class GameOverWindow : MonoBehaviour
{
    private Text scoreText;
    private Text highscoreText;
    private void Awake()
    {
        scoreText = transform.Find("scoreText").GetComponent<Text>();
        highscoreText = transform.Find("highscoreText").GetComponent<Text>();//add sounds to buttons

        transform.Find("retryButton").GetComponent<Button>().onClick.AddListener(delegate { RetryButtonClicked(); });
        transform.Find("mainMenuButton").GetComponent<Button>().onClick.AddListener(delegate { MainMenuButtonClicked(); });
    }

    private void Start()
    {
        Oscar.GetInstance().OnHit += Oscar_OnHit;
        Hide();
    }

    private void Update()
    {
        //retry using spaceber
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Loader.Load(Loader.Scene.GameScene);
        }
    }

    private void RetryButtonClicked()
    {
        Loader.Load(Loader.Scene.GameScene);
        SoundManager.PlaySound(SoundManager.Sound.ButtonClick);
    }

    private void MainMenuButtonClicked()
    {
        Loader.Load(Loader.Scene.MainMenu);
        SoundManager.PlaySound(SoundManager.Sound.ButtonClick);
    }
    
    private void Oscar_OnHit(object sender, EventArgs e)
    {
        scoreText.text = Level.GetInstance().GetBranchesPassed().ToString();
        if (Level.GetInstance().GetBranchesPassed() > Score.GetHighScore())
        {
            highscoreText.text = "NEW HIGHSCORE";
        }
        else
        {
            highscoreText.text = "HIGHSCORE: " + Score.GetHighScore();
        }
        Show();
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
