using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HintManager : MonoBehaviour
{

    private Board board;
    public float HintDelay;
    private float HintDelaySeconds;
    public GameObject HintParticle;
    public GameObject CurrentHint;

    // Start is called before the first frame update
    void Start()
    {
        board = FindObjectOfType<Board>();
        HintDelaySeconds = HintDelay;
    }

    // Update is called once per frame
    void Update()
    {
        HintDelaySeconds -= Time.deltaTime;
        if(HintDelaySeconds <= 0 && CurrentHint == null)
        {
            MarkHint();
            HintDelaySeconds = HintDelay;
        }
    }

    //find possible matches 
    List<GameObject> FindAllMatches()
    {
        List<GameObject> PossibleMoves = new List<GameObject>();
        for(int i = 0; i<board.width; i++)
        {
            for(int j = 0; j< board.height; j++)
            {
                if (board.AllDots[i, j] != null)
                {
                    if (i< board.width - 1)
                    {
                        if (board.SwitchAndCheck(i, j, Vector2.right))
                        {
                            PossibleMoves.Add(board.AllDots[i, j]);
                        }
                    }
                    if (j < board.height - 1)
                    {
                        if (board.SwitchAndCheck(i, j, Vector2.up))
                        {
                            PossibleMoves.Add(board.AllDots[i, j]);
                        }
                    }
                }
            }
        }
        return PossibleMoves;
    }
    //pick one of them
    GameObject PickOneRandomly()
    {
        List<GameObject> PossibleMoves = new List<GameObject>();
        PossibleMoves = FindAllMatches();
        if(PossibleMoves.Count > 0)
        {
            int pieceToUse = Random.Range(0, PossibleMoves.Count);
            return PossibleMoves[pieceToUse];
                
        }
        return null;
    }
    //create hint behind the dot
    private void MarkHint()
    {
        GameObject Move = PickOneRandomly();
        if(Move != null)
        {
            CurrentHint = Instantiate(HintParticle, Move.transform.position, Quaternion.identity);
        }
    }
    //destroy hint
    public void DestroyHint()
    {
        Destroy(CurrentHint);
        CurrentHint = null;
        HintDelaySeconds = HintDelay;
    }



}
