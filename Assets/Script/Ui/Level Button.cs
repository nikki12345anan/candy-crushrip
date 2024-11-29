using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour
{
    [Header("active stuffie")]
    public bool isactive;
    public Sprite activesprite;
    public Sprite lockedsprite;
    private Image ButtonImage;
    private Button Mybutton;
    private int starsActive;


    [Header("Level ui")]
    public Image[] stars;
    public Text Leveltext;
    public int level;
    public GameObject ConfirmPanel;

    private Game_Data gamedata;

    // Start is called before the first frame update
    void Start()
    {
        gamedata = FindObjectOfType<Game_Data>();
        ButtonImage = GetComponent<Image>();
        Mybutton = GetComponent<Button>();
        loadData();
        ActiveateStars();
        Showlevel();
        SpriteDecider();
        
    }

    void loadData()
    {
        //is game data present?
        if(gamedata != null)
        {
            //decide if level active
            if(gamedata.savedata.IsActive[level - 1])
            {
                isactive = true;
            }
            else
            {
                isactive = false;
            }
            //Decide star number
            starsActive = gamedata.savedata.stars[level - 1];
        }
    }

    void ActiveateStars()
    {
         for (int i = 0; i < starsActive; i++)
        {
            stars[i].enabled = true;
        }
    }
    void SpriteDecider()
    {
        if (isactive)
        {
            ButtonImage.sprite = activesprite;
            Mybutton.enabled = true;
            Leveltext.enabled = true;
        }

        else
        {
            ButtonImage.sprite = lockedsprite;
            Mybutton.enabled = false;
            Leveltext.enabled = false;
        }
    }

    void Showlevel()
    {
        Leveltext.text = "" + level;
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
    public void Confirmpanel(int level)
    {
        ConfirmPanel.GetComponent<ConfirmPanel>().level = level;
        ConfirmPanel.SetActive(true);
    }
}
