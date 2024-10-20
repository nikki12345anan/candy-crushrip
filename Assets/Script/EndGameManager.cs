using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Gametype
{
    moves,
    time
        
}

[System.Serializable]
public class EndgameRequirements
{
    public Gametype gameType;
    public int counterValue;
}
public class EndGameManager : MonoBehaviour
{
    public GameObject Movelabel;
    public GameObject Timelabel;
    public GameObject YouWinPanel;
    public GameObject YouLosePanel;
    public Text counter;
    public EndgameRequirements requirements;
    public int CurrentCounterValue;
    public Board board;
    public float TimerSeconds;

    // Start is called before the first frame update
    void Start()
    {
        board = FindObjectOfType<Board>();
        setupGame();
    }

    void setupGame()
    {
        CurrentCounterValue = requirements.counterValue;
        if(requirements.gameType == Gametype.moves)
        {
            Movelabel.SetActive(true);
            Timelabel.SetActive(false);
        }
        else
        {
            TimerSeconds = 1;
            Movelabel.SetActive(false);
            Timelabel.SetActive(true);
        }
        counter.text = "" + CurrentCounterValue;
    }

    public void DecreaseCounterValue()
    {
        if (board.currentState != GameState.pause)
        {
            CurrentCounterValue--;
            counter.text = "" + CurrentCounterValue;
            if (CurrentCounterValue <= 0)
            {
                LoseGame();
            }
        }
    }

    public void WinGame()
    {
        YouWinPanel.SetActive(true);
        board.currentState = GameState.win;
        CurrentCounterValue = 0;
        counter.text = "" + CurrentCounterValue;
        PanelfadeinController fade = FindObjectOfType<PanelfadeinController>();
        fade.GameOver();
    }

    public void LoseGame()
    {
        YouLosePanel.SetActive(true);
        board.currentState = GameState.lose;
        Debug.Log("lose");
        CurrentCounterValue = 0;
        counter.text = "" + CurrentCounterValue;
        PanelfadeinController fade = FindObjectOfType<PanelfadeinController>();
        fade.GameOver();
    }
    // Update is called once per frame
    void Update()
    {
        if(requirements.gameType == Gametype.time && CurrentCounterValue > 0)
        {
            TimerSeconds -= Time.deltaTime;
            if(TimerSeconds <= 0)
            {
                DecreaseCounterValue();
                TimerSeconds = 1;
            }
        }
    }
}
