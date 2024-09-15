using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;

public class FindMatches : MonoBehaviour
{
    private Board Board;
    public List<GameObject> currentMatches = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        Board = FindAnyObjectByType<Board>();
    }

    public void FindAllMatches()
    {
        StartCoroutine(FindAllMatchesCo());
    }

    private IEnumerator FindAllMatchesCo()
    {
        yield return new WaitForSeconds(.2f);
        for(int i = 0; i < Board.width; i++)
        {
            for(int j = 0; j < Board.height; j++)
            {
                GameObject currentDot = Board.AllDots[i, j];
                if(currentDot != null)
                {
                    if(i > 0 && i < Board.width - 1)
                    {
                        GameObject LeftDot = Board.AllDots[i - 1, j];
                        GameObject RightDot = Board.AllDots[i + 1, j];
                        if (LeftDot != null && RightDot != null)
                        {
                            if(LeftDot.tag == currentDot.tag && RightDot.tag == currentDot.tag)
                            {

                                if(currentDot.GetComponent<Dot>().IsRowBomb
                                    || LeftDot.GetComponent<Dot>().IsRowBomb
                                    || RightDot.GetComponent<Dot>().IsRowBomb)
                                {
                                    currentMatches.Union(GetRowPieces(j));
                                }

                                if (currentDot.GetComponent<Dot>().IsColumnBomb)
                                {
                                    currentMatches.Union(GetColumnPieces(i));
                                }

                                if (LeftDot.GetComponent<Dot>().IsColumnBomb)
                                {
                                    currentMatches.Union(GetColumnPieces(i - 1));
                                }

                                if (RightDot.GetComponent<Dot>().IsColumnBomb)
                                {
                                    currentMatches.Union(GetColumnPieces(i + 1));
                                }

                                if (!currentMatches.Contains(LeftDot))
                                {
                                    currentMatches.Add(LeftDot);
                                }
                                LeftDot.GetComponent<Dot>().IsMatched = true;
                                if (!currentMatches.Contains(RightDot))
                                {
                                    currentMatches.Add(RightDot);
                                }
                                RightDot.GetComponent<Dot>().IsMatched = true;
                                if (!currentMatches.Contains(currentDot))
                                {
                                    currentMatches.Add(currentDot);
                                }
                                currentDot.GetComponent<Dot>().IsMatched = true;
                            }
                        }
                    }

                    if (j > 0 && j < Board.height - 1)
                    {
                        GameObject UpDot = Board.AllDots[i , j + 1];
                        GameObject DownDot = Board.AllDots[i, j - 1];
                        if (UpDot != null && DownDot != null)
                        {
                            if (UpDot.tag == currentDot.tag && DownDot.tag == currentDot.tag)
                            {

                                if (currentDot.GetComponent<Dot>().IsColumnBomb
                                    || UpDot.GetComponent<Dot>().IsColumnBomb
                                    || DownDot.GetComponent<Dot>().IsColumnBomb)
                                {
                                    currentMatches.Union(GetColumnPieces(i));
                                }

                                if (currentDot.GetComponent<Dot>().IsRowBomb)
                                {
                                    currentMatches.Union(GetRowPieces(j));
                                }

                                if (UpDot.GetComponent<Dot>().IsRowBomb)
                                {
                                    currentMatches.Union(GetRowPieces(j + 1));
                                }

                                if (DownDot.GetComponent<Dot>().IsRowBomb)
                                {
                                    currentMatches.Union(GetRowPieces(j - 1));
                                }

                                if (!currentMatches.Contains(UpDot))
                                {
                                    currentMatches.Add(UpDot);
                                }

                                UpDot.GetComponent<Dot>().IsMatched = true;
                                if (!currentMatches.Contains(DownDot))
                                {
                                    currentMatches.Add(DownDot);
                                }
                                DownDot.GetComponent<Dot>().IsMatched = true;
                                if (!currentMatches.Contains(currentDot))
                                {
                                    currentMatches.Add(currentDot);
                                }
                                currentDot.GetComponent<Dot>().IsMatched = true;
                            }
                        }
                    }
                }
            }
        }
    }

    List<GameObject> GetColumnPieces(int column)
    {
        List<GameObject> dots = new List<GameObject>();
        for(int i = 0; i< Board.height; i++)
        {
            if (Board.AllDots[column, i] != null)
            {
                dots.Add(Board.AllDots[column, i]);
                Board.AllDots[column, i].GetComponent<Dot>().IsMatched = true;
            }
        }
        return dots;
    }

    List<GameObject> GetRowPieces(int Row)
    {
        List<GameObject> dots = new List<GameObject>();
        for (int i = 0; i < Board.width; i++)
        {
            if (Board.AllDots[i, Row] != null)
            {
                dots.Add(Board.AllDots[i, Row]);
                Board.AllDots[i, Row].GetComponent<Dot>().IsMatched = true;
            }
        }
        return dots;
    }

    public void CheckBombs()
    {
        //did the player moved something
        if(Board.currentdot != null)
        {
            //is it matched
            if (Board.currentdot.IsMatched)
            {
                //make it unmacthed
                Board.currentdot.IsMatched = false;
                /*
                //decide what bomb
                int Typeofbomb = Random.Range(0, 100);
                if (Typeofbomb < 50)
                {
                    //row bomb
                    Board.currentdot.MakeRowbomb();

                }
                else if(Typeofbomb >= 50)
                {
                    //column bomb
                    Board.currentdot.MakeColumnbomb();
                }
                */
                if((Board.currentdot.SwipeAngle > -45 && Board.currentdot.SwipeAngle <=45)
                    ||(Board.currentdot.SwipeAngle < -135 && Board.currentdot.SwipeAngle >= 135))
                {
                    Board.currentdot.MakeRowbomb();
                }
                else
                {
                    Board.currentdot.MakeColumnbomb();
                }
            }
            //is other piece matched
            else if (Board.currentdot.OtherDot != null)
            {
                Dot otherdot = Board.currentdot.OtherDot.GetComponent<Dot>();
                //is other dot match
                if (otherdot.IsMatched)
                {
                    //make it umached
                    otherdot.IsMatched = false;
                    /*
                    //decide type of bomb
                    int Typeofbomb = Random.Range(0, 100);
                    if (Typeofbomb < 50)
                    {
                        //row bomb
                        otherdot.MakeRowbomb();

                    }
                    else if (Typeofbomb >= 50)
                    {
                        //column bomb
                        otherdot.MakeColumnbomb();
                    }
                    */
                    if ((Board.currentdot.SwipeAngle > -45 && Board.currentdot.SwipeAngle <= 45)
                    || (Board.currentdot.SwipeAngle < -135 && Board.currentdot.SwipeAngle >= 135))
                    {
                        otherdot.MakeRowbomb();
                    }
                    else
                    {
                        otherdot.MakeColumnbomb();
                    }
                }
            }
            }
        }
    }

    