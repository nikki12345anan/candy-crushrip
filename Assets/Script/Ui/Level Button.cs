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


    public Image[] stars;
    public Text Leveltext;
    public int level;
    public GameObject ConfirmPanel;

    // Start is called before the first frame update
    void Start()
    {
        ButtonImage = GetComponent<Image>();
        Mybutton = GetComponent<Button>();
        ActiveateStars();
        Showlevel();
        SpriteDecider();
        
    }

    void ActiveateStars()
    {
        //Not complete binary file waiting
         for (int i = 0; i< stars.Length; i++)
        {
            stars[i].enabled = false;
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
