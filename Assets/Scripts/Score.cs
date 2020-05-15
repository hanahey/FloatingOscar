using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Score
{
    public static void Start()
    {
        Oscar.GetInstance().OnHit += Oscar_OnHit;
    }

    public static void Oscar_OnHit(object sender, EventArgs e)
    {
        TrySetNewHighScore(Level.GetInstance().GetBranchesPassed());
    }


    public static int GetHighScore()
    {
        return PlayerPrefs.GetInt("highscore");
    }

    public static bool TrySetNewHighScore (int score)
    {
        int currentHighscore = GetHighScore();
        if (score > currentHighscore)
        {   //Save new highscore
            PlayerPrefs.SetInt("highscore", score);
            PlayerPrefs.Save();
            return true;
        }
        else
        {
            return false;
        }
    }

    public static void ResetHighscore()
    {
        PlayerPrefs.SetInt("highscore", 0);
        PlayerPrefs.Save();
    }
}
