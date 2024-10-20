using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelfadeinController : MonoBehaviour
{

    public Animator PanelAnim;
    public Animator GameInfoAnim;

    public void OK()
    {
        if(PanelAnim != null && GameInfoAnim != null)
        {
            PanelAnim.SetBool("out", true);
            GameInfoAnim.SetBool("Out", true);
            StartCoroutine(GamestartCo());
        }
        
    }

    public void GameOver()
    {
        PanelAnim.SetBool("out", false);
        PanelAnim.SetBool("Game Over", true);
    }
    IEnumerator GamestartCo()
    {
        yield return new WaitForSeconds(1f);
        Board board = FindObjectOfType<Board>();
        board.currentState = GameState.move;
    }
}
