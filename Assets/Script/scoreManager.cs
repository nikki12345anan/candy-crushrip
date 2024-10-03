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

    // Start is called before the first frame update
    void Start()
    {
        board = FindObjectOfType<Board>();
    }

    // Update is called once per frame
    void Update()
    {
        ScoreText.text = "" + Score;
    }

    public void IncreaseScore(int amountToIncrease)
    {
        Score += amountToIncrease;
        if(board != null && ScoreBar != null)
        {
            int length = board.scoreGoals.Length;
            ScoreBar.fillAmount = (float)Score / (float)board.scoreGoals[length - 1];
        }
    }
}
