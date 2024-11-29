using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class scoreManager : MonoBehaviour
{
    public Board board;
    public Text ScoreText;
    public int Score;
    public Image ScoreBar;
    private Game_Data gameData;

    // Start is called before the first frame update
    void Start()
    {
        board = FindObjectOfType<Board>();
        gameData = FindObjectOfType<Game_Data>();
        Updatebar();
    }

    // Update is called once per frame
    void Update()
    {
        ScoreText.text = "" + Score;
    }

    public void IncreaseScore(int amountToIncrease)
    {
        Score += amountToIncrease;
        if (gameData != null)
        {
            int gihscore = gameData.savedata.Highscores[board.level];
            if (Score > gihscore)
            {
                gameData.savedata.Highscores[board.level] = Score;
            }
            gameData.save();
            Updatebar();
        }
    }

    private void Updatebar()
    {
        if (board != null && ScoreBar != null)
        {
            int length = board.scoreGoals.Length;
            ScoreBar.fillAmount = (float)Score / (float)board.scoreGoals[length - 1];
        }
    }
}
