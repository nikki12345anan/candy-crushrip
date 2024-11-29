using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Net;

public class ConfirmPanel : MonoBehaviour
{
    [Header("level info")]
    public string Leveltoload;
    public int level;
    private int starsActive;
    private Game_Data gameData;
    private int Highscore;

    [Header("Ui stuffie")]
    public Image[] stars;
    public Text highscoretext;
    public Text startext;

    private void OnEnable()
    {
        gameData = FindObjectOfType<Game_Data>();
        LoadData();
        ActiveateStars();
        settext();
    }
    void LoadData()
    {
        if(gameData != null)
        {
            starsActive = gameData.savedata.stars[level - 1];
            Highscore = gameData.savedata.Highscores[level - 1];

        }
    }
    void ActiveateStars()
    {
        //Not complete binary file waiting
        for (int i = 0; i < starsActive; i++)
        {
            stars[i].enabled = true;
        }
    }

    void settext()
    {
        highscoretext.text = "" + Highscore;
        startext.text = starsActive + "/3";
        Debug.Log("afas");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void cancel()
    {
        this.gameObject.SetActive(false);
        for (int i = 0; i < starsActive; i++)
        {
            stars[i].enabled = false;
        }
    }

    public void play()
    {
        PlayerPrefs.SetInt("Current level", level - 1);
        SceneManager.LoadScene(Leveltoload);
    }
}
