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
    public GameObject GoalPreFab;
    public GameObject GoalIntroParent;
    public GameObject GoalGameParent;
    // Start is called before the first frame update
    void Start()
    {
        SetupIntroGoals();
    }

    void SetupIntroGoals()
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
            panel.thisSprite = LevelGoals[i].GoalSprite;
            panel.thisString = "0/" + LevelGoals[i].NumberNeeded;
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
