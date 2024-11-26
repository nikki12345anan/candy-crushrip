using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ConfirmPanel : MonoBehaviour
{
    public string Leveltoload;
    public Image[] stars;
    public int level;

    // Start is called before the first frame update
    void Start()
    {
        ActiveateStars();
    }

    void ActiveateStars()
    {
        //Not complete binary file waiting
        for (int i = 0; i < stars.Length; i++)
        {
            stars[i].enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void cancel()
    {
        this.gameObject.SetActive(false);
    }

    public void play()
    {
        PlayerPrefs.SetInt("Current level", level - 1);
        SceneManager.LoadScene(Leveltoload);
    }
}
