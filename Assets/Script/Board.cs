using System.Collections;
using System.Collections.Generic;
using System.Security.Principal;
using UnityEngine;

public enum GameState
{
    //hamle bekleme suresi gerekirse if al kisitla
    wait,
    move
}

public class Board : MonoBehaviour
{

    public GameState currentState = GameState.move;
    public int width;
    public int height;
    public int OffSet;
    public GameObject TilePrefab;
    public GameObject[] dots;
    public GameObject DestroyEffect;
    private Backgroundtile[,] allTiles;
    public GameObject[,] AllDots;
    public Dot currentdot;
    private FindMatches findMatches;

    // Start is called before the first frame update
    void Start()
    {
        findMatches = FindObjectOfType<FindMatches>();
        allTiles = new Backgroundtile[width, height];
        AllDots = new GameObject[width, height];
        SetUp();
    }

    private void SetUp()
    {
        for (int i = 0; i < width; i ++)
        {
            for (int j = 0; j < height; j ++)
            {
                Vector2 TempPosition = new Vector2(i, j + OffSet);
                GameObject backgroundTile = Instantiate(TilePrefab, TempPosition, Quaternion.identity) as GameObject;
                backgroundTile.transform.parent = this.transform;
                backgroundTile.name = "(" + i + "," + j + ")";
                int DotToUse = Random.Range(0, dots.Length);
                int MaxIterations = 0;
                while(MatchesAt(i, j,dots[DotToUse]) && MaxIterations < 100)
                {
                    DotToUse = Random.Range(0, dots.Length);
                    MaxIterations++;
                }
                MaxIterations = 0;

                GameObject dot = Instantiate(dots[DotToUse], TempPosition, Quaternion.identity);
                dot.GetComponent<Dot>().row = j;
                dot.GetComponent<Dot>().Column = i;



                dot.transform.parent = this.transform;
                dot.name = "(" + i + "," + j + ")";
                AllDots[i, j] = dot;
            }
        }
    }

    private bool MatchesAt(int column, int row, GameObject piece)
    {
        if(column > 1 && row > 1)
        {
            if (AllDots[column - 1, row].tag == piece.tag && AllDots[column - 2, row].tag == piece.tag)
            {
                return true;
            }
            if (AllDots[column, row - 1].tag == piece.tag && AllDots[column, row - 2].tag == piece.tag)
            {
                return true;
            }
        }
        else if(column <=1 || row <= 1)
        {
            if(row > 1)
            {
                if (AllDots[column, row - 1].tag == piece.tag && AllDots[column, row - 2].tag == piece.tag)
                {
                    return true;
                }
            }
            if (column > 1)
            {
                if (AllDots[column - 1, row].tag == piece.tag && AllDots[column- 2, row].tag == piece.tag)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private bool ColumnOrRow()
    {
        int numberHorizantel = 0;
        int NumberVertical = 0;
        Dot Firstpiece = findMatches.currentMatches[0].GetComponent<Dot>();
        if(Firstpiece != null)
        {
            foreach (GameObject currentpiece in findMatches.currentMatches)
            {
                Dot dot = currentpiece.GetComponent<Dot>();
                if(dot.row == Firstpiece.row)
                {
                    numberHorizantel++;
                }
                if(dot.Column == Firstpiece.Column)
                {
                    NumberVertical++;
                }
            }
        }
        return (NumberVertical == 5 || numberHorizantel == 5);
    }

    private void CheckToMakeBombs()
    {
        if(findMatches.currentMatches.Count == 4 || findMatches.currentMatches.Count == 7)
        {
            findMatches.CheckBombs();
        }
        if(findMatches.currentMatches.Count == 5 || findMatches.currentMatches.Count == 8)
        {
            if (ColumnOrRow())
            {
                //make a color bomb
                Debug.Log("colorbomb");
                //is the current dot matched
                if(currentdot != null)
                {
                    if (currentdot.IsColorBomb)
                    {
                        currentdot.IsMatched = false;
                        currentdot.MakeColorBomb();
                    }
                    else
                    {
                        if(currentdot.OtherDot != null)
                        {
                            Dot otherdot = currentdot.OtherDot.GetComponent<Dot>();
                            if (otherdot.IsMatched)
                            {
                                if (!otherdot.IsColorBomb)
                                {
                                    otherdot.IsMatched = false;
                                    otherdot.MakeColorBomb();
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                //make adjacent bomb
                Debug.Log("adjacentbomb");
                //is the current dot matched
                if (currentdot != null)
                {
                    if (currentdot.IsAdjacentBomb)
                    {
                        currentdot.IsMatched = false;
                        currentdot.MakeAdjacentBomb();
                    }
                    else
                    {
                        if (currentdot.OtherDot != null)
                        {
                            Dot otherdot = currentdot.OtherDot.GetComponent<Dot>();
                            if (otherdot.IsMatched)
                            {
                                if (!otherdot.IsAdjacentBomb)
                                {
                                    otherdot.IsMatched = false;
                                    otherdot.MakeAdjacentBomb();
                                }
                            }
                        }
                    }
                }

            }


        }
    }

    private void DestroyMatchesAt(int column, int row)
    {
        if (AllDots[column, row].GetComponent<Dot>().IsMatched)
        {
            //how many matches elements are in the list
            if(findMatches.currentMatches.Count >= 4)
            {
                CheckToMakeBombs();
            }

            GameObject particle = Instantiate(DestroyEffect, AllDots[column, row].transform.position, Quaternion.identity);
            Destroy(particle, .5f);
            Destroy(AllDots[column, row]);
            AllDots[column, row] = null;
        }
    }
    public void DestroyMatches()
    {
        for(int i = 0; i < width; i++)
        {
            for(int j = 0; j < height; j++)
            {
                if (AllDots[i,j] != null)
                {
                    DestroyMatchesAt(i, j);
                }
            }
        }
        findMatches.currentMatches.Clear();
        StartCoroutine(DecreaseRowCo());
    }

    private IEnumerator DecreaseRowCo()
    {
        int nullcount = 0;
        for (int i = 0; i < width; i++)
        {
            for(int j = 0; j < height; j++)
            {
                if (AllDots[i,j] == null)
                {
                    nullcount++;
                }
                else if(nullcount > 0)
                {
                    AllDots[i, j].GetComponent<Dot>().row -= nullcount;
                    AllDots[i, j] = null;
                }
            }
            nullcount = 0;
        }
        yield return new WaitForSeconds(.4f);
        StartCoroutine(FillboardCo());
    }

    private void RefillBoard()
    {
        for(int i = 0; i < width; i++)
        {
            for(int j = 0; j < height; j++)
            {
                if (AllDots[i,j] == null)
                {
                    Vector2 tempposition = new Vector2(i, j + OffSet);
                    int dotTouse = Random.Range(0, dots.Length);
                    GameObject piece = Instantiate(dots[dotTouse], tempposition, Quaternion.identity);
                    AllDots[i, j] = piece;
                    piece.GetComponent<Dot>().row = j;
                    piece.GetComponent<Dot>().Column = i;

                }
            }
        }
    }

    private bool MatchesOnBoard()
    {
        for(int i = 0; i < width; i++)
        {
            for(int j = 0; j < height; j++)
            {
                if (AllDots[i,j]!= null)
                {
                    if (AllDots[i, j].GetComponent<Dot>().IsMatched)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    private IEnumerator FillboardCo()
    {
        RefillBoard();
        yield return new WaitForSeconds(.5f);

        while (MatchesOnBoard())
        {
            yield return new WaitForSeconds(.5f);
            DestroyMatches();
        }
        findMatches.currentMatches.Clear();
        currentdot = null;
        yield return new WaitForSeconds(.5f);
        currentState = GameState.move;
    }
}
