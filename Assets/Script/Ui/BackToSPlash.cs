using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackToSPlash : MonoBehaviour
{
    public string Scenetoload;
    private Game_Data gameData;
    private Board board;
    public void WinOK()
    {
        if(gameData != null)
        {
            gameData.savedata.IsActive[board.level + 1] = true;
            gameData.save();
        }
        SceneManager.LoadScene(Scenetoload);
    }

    public void LoseOk()
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        board = FindObjectOfType<Board>();
        gameData = FindObjectOfType<Game_Data>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
