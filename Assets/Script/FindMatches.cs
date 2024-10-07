using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;

public class FindMatches : MonoBehaviour
{
    private Board Board;
    public List<GameObject> currentMatches = new List<GameObject>();

    void Start()
    {
        Board = FindAnyObjectByType<Board>();
    }

    public void FindAllMatches()
    {
        StartCoroutine(FindAllMatchesCo());
    }

    private List<GameObject> IsAdjacentBomb(Dot dot1, Dot dot2, Dot dot3)
    {

        List<GameObject> currentdots = new List<GameObject>();
        if (dot1.IsAdjacentBomb)
        {
            currentMatches.Union(GetAdjacentPieces(dot1.Column ,dot1.row));
        }

        if (dot2.IsAdjacentBomb)
        {
            currentMatches.Union(GetAdjacentPieces(dot2.Column,dot2.row));
        }

        if (dot3.IsAdjacentBomb)
        {
            currentMatches.Union(GetAdjacentPieces(dot3.Column,dot3.row));
        }
        return currentdots;
    }

    private List<GameObject> IsRowBomb(Dot dot1, Dot dot2, Dot dot3)
    {
        List<GameObject> currentdots = new List<GameObject>();
        if (dot1.IsRowBomb)
        {
            currentMatches.Union(GetRowPieces(dot1.row));
        }

        if (dot2.IsRowBomb)
        {
            currentMatches.Union(GetRowPieces(dot2.row));
        }

        if (dot3.IsRowBomb)
        {
            currentMatches.Union(GetRowPieces(dot3.row));
        }
        return currentdots;
    }

    private List<GameObject> IsColumnBomb(Dot dot1, Dot dot2, Dot dot3)
    {
        List<GameObject> currentdots = new List<GameObject>();
        if (dot1.IsColumnBomb)
        {
            currentMatches.Union(GetColumnPieces(dot1.Column));
        }

        if (dot2.IsColumnBomb)
        {
            currentMatches.Union(GetColumnPieces(dot2.Column));
        }

        if (dot3.IsColumnBomb)
        {
            currentMatches.Union(GetColumnPieces(dot3.Column));
        }
        return currentdots;
    }

    private void AddToListAndMatch(GameObject dot)
    {
        if (!currentMatches.Contains(dot))
        {
            currentMatches.Add(dot);
        }
        dot.GetComponent<Dot>().IsMatched = true;
    }

    private void GetNearbyPieces(GameObject dot1, GameObject dot2, GameObject dot3)
    {
        AddToListAndMatch(dot1);
        AddToListAndMatch(dot2);
        AddToListAndMatch(dot3);

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
                    Dot currentDotDot = currentDot.GetComponent<Dot>();

                    if (i > 0 && i < Board.width - 1)
                    {
                        GameObject LeftDot = Board.AllDots[i - 1, j];
                        GameObject RightDot = Board.AllDots[i + 1, j];

                        if (LeftDot != null && RightDot != null)
                        {
                            Dot RightDotDot = RightDot.GetComponent<Dot>();
                            Dot LeftDotDot = LeftDot.GetComponent<Dot>();



                            if (LeftDot != null && RightDot != null)
                            {
                                if (LeftDot.tag == currentDot.tag && RightDot.tag == currentDot.tag)
                                {
                                    currentMatches.Union(IsRowBomb(LeftDotDot, currentDotDot, RightDotDot));

                                    currentMatches.Union(IsColumnBomb(LeftDotDot, currentDotDot, RightDotDot));

                                    currentMatches.Union(IsAdjacentBomb(LeftDotDot, currentDotDot, RightDotDot));

                                    GetNearbyPieces(LeftDot, currentDot, RightDot);
                                }
                            }
                        }
                    
                    }

                    if (j > 0 && j < Board.height - 1)
                    {
                        GameObject UpDot = Board.AllDots[i , j + 1];
                        GameObject DownDot = Board.AllDots[i, j - 1];
                        if (UpDot != null && DownDot != null)
                        {
                            Dot DownDotDot = DownDot.GetComponent<Dot>();
                            Dot UpDotDot = UpDot.GetComponent<Dot>();



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

                                    currentMatches.Union(IsColumnBomb(UpDotDot, currentDotDot, DownDotDot));

                                    currentMatches.Union(IsRowBomb(UpDotDot, currentDotDot, DownDotDot));

                                    currentMatches.Union(IsAdjacentBomb(UpDotDot, currentDotDot, DownDotDot));


                                    GetNearbyPieces(UpDot, currentDot, DownDot);
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    public void MatchPiecesOfColor(string color)
    {
        for (int i = 0; i < Board.width; i++)
        {
            for (int j = 0; j < Board.height; j++)
            {                    
                //check if that piece exist
                if (Board.AllDots[i,j] != null)
                {
                    if (Board.AllDots[i, j].tag == color)
                    {
                        //set that dot to be matched
                        Board.AllDots[i, j].GetComponent<Dot>().IsMatched = true;
                    }
                }
            }
        }
    }

    List<GameObject> GetAdjacentPieces(int column, int row)
    {
        List<GameObject> dots = new List<GameObject>();
        for(int i = column - 1; i <=column + 1; i++)
        {
            for(int j = row - 1; j <= row + 1; j++)
            {
                //check if the piece is inside the board... hamburger aaaaaaaaaaaaaa
                if(i >= 0 && i < Board.width && j >= 0 && j< Board.height)
                {
                    if (Board.AllDots[i,j] != null)
                    {
                        dots.Add(Board.AllDots[i, j]);
                        Board.AllDots[i, j].GetComponent<Dot>().IsMatched = true;
                    }
                }
            }
        }
        return dots;
    }

    List<GameObject> GetColumnPieces(int column)
    {
        List<GameObject> dots = new List<GameObject>();
        for(int i = 0; i< Board.height; i++)
        {
            if (Board.AllDots[column, i] != null)
            {
                Dot dot = Board.AllDots[column, i].GetComponent<Dot>();
                if (dot.IsRowBomb)
                {
                    dots.Union(GetRowPieces(i)).ToList();
                }
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
                Dot dot = Board.AllDots[i,Row].GetComponent<Dot>();
                if (dot.IsColumnBomb)
                {
                    dots.Union(GetColumnPieces(i)).ToList();
                }
                dots.Add(Board.AllDots[i, Row]);
                dot.IsMatched = true;
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

    