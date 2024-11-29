using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class BlankGoal
{
    public int NumberNeeded;
    public int NumberCollected;
    public Sprite GoalSprite;
    public string MatchValue;
}

public class goalManager : MonoBehaviour
{
    public BlankGoal[] LevelGoals;
    public List<GoalPanel> CurrentGoals = new List<GoalPanel>();
    public GameObject GoalPreFab;
    public GameObject GoalIntroParent;
    public GameObject GoalGameParent;
    private Board board;
    private EndGameManager endGame;
    // Start is called before the first frame update
    void Start()
    {
        board = FindObjectOfType<Board>();
        endGame = FindObjectOfType<EndGameManager>();
        GetGoals();
        SetupGoals();

    }

    void GetGoals()
    {
        if (board != null)
            if (board.world != null)
            {
                if (board.level < board.world.levels.Length)
                {
                    if (board.world.levels[board.level] != null)
                    {
                        LevelGoals = board.world.levels[board.level].levelGoals;
                        for (int i = 0; i < LevelGoals.Length; i++)
                        {
                            LevelGoals[i].NumberCollected = 0;
                        }
                    }
                }
            }
    }
    void SetupGoals()
    {
        for(int i = 0; i < LevelGoals.Length; i++)
        {
            //goalintroparentte yeni bi panel olustur
            GameObject goal = Instantiate(GoalPreFab, GoalIntroParent.transform.position, Quaternion.identity);
            goal.transform.SetParent(GoalIntroParent.transform);
            //make imake and set text
            GoalPanel panel = goal.GetComponent<GoalPanel>();
            panel.thisSprite = LevelGoals[i].GoalSprite;
            panel.thisString = "0/" + LevelGoals[i].NumberNeeded;

            //game goal oluþtur konumda
            GameObject gamegoal = Instantiate(GoalPreFab, GoalGameParent.transform.position, Quaternion.identity);
            gamegoal.transform.SetParent(GoalGameParent.transform);
            panel = gamegoal.GetComponent<GoalPanel>();
            CurrentGoals.Add(panel);
            panel.thisSprite = LevelGoals[i].GoalSprite;
            panel.thisString = "0/" + LevelGoals[i].NumberNeeded;
        }
    }
    
    public void UpdateGoals()
    {
        int GoalsCompleted = 0;
        for(int i = 0; i < LevelGoals.Length; i++)
        {
            CurrentGoals[i].thisText.text = "" + LevelGoals[i].NumberCollected + "/" + LevelGoals[i].NumberNeeded;
            if (LevelGoals[i].NumberCollected >= LevelGoals[i].NumberNeeded)
            {
                GoalsCompleted++;
                CurrentGoals[i].thisText.text = "" + LevelGoals[i].NumberNeeded + "/" + LevelGoals[i].NumberNeeded;
            }
        }
        if(GoalsCompleted >= LevelGoals.Length)
        {
            if(endGame != null)
            {
                endGame.WinGame();
                Debug.Log("youwwinn");

            }
        }
    }
    public void CompareGoal(string GoalToCompare)
    {
        for(int i = 0; i < LevelGoals.Length; i++)
        {
            if(GoalToCompare == LevelGoals[i].MatchValue)
            {
                LevelGoals[i].NumberCollected++;
            }
        }
    }

}
