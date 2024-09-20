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

public enum TileKind
{
    Breakable,
    Blank,
    Normal
}
[System.Serializable]
public class TileType
{
    public int x;
    public int y;
    public TileKind tilekind;
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
    public TileType[] BoardLayout;
    private bool[,] BlankSpaces;
    public GameObject[,] AllDots;
    public Dot currentdot;
    private FindMatches findMatches;

    // Start is called before the first frame update
    void Start()
    {
        findMatches = FindObjectOfType<FindMatches>();
        BlankSpaces = new bool[width, height];
        AllDots = new GameObject[width, height];
        SetUp();
    }

    public void GenerateBlankSpace()
    {
        for (int i =0; i < BoardLayout.Length; i++)
        {
            if (BoardLayout[i].tilekind == TileKind.Blank)
            {
                BlankSpaces[BoardLayout[i].x, BoardLayout[i].y] = true;
            }
        }
    }

    private void SetUp()
    {
        GenerateBlankSpace();
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (!BlankSpaces[i, j])
                {
                    Vector2 tempPosition = new Vector2(i, j + OffSet);
                    GameObject backgroundTile = Instantiate(TilePrefab, tempPosition, Quaternion.identity) as GameObject;
                    backgroundTile.transform.parent = this.transform;
                    backgroundTile.name = "( " + i + ", " + j + " )";

                    int dotToUse = Random.Range(0, dots.Length);

                    int maxIterations = 0;

                    while (MatchesAt(i, j, dots[dotToUse]) && maxIterations < 100)
                    {
                        dotToUse = Random.Range(0, dots.Length);
                        maxIterations++;
                        Debug.Log(maxIterations);
                    }

                    maxIterations = 0;

                    GameObject dot = Instantiate(dots[dotToUse], tempPosition, Quaternion.identity);
                    dot.GetComponent<Dot>().row = j;
                    dot.GetComponent<Dot>().Column = i;
                    dot.transform.parent = this.transform;
                    dot.name = "( " + i + ", " + j + " )";
                    AllDots[i, j] = dot;
                    DestroyMatches();
                }
            }

        }
    }

    private bool MatchesAt(int column, int row, GameObject piece)
    {
        if (column > 1 && row > 1)
        {
            if (AllDots[column - 1, row] != null && AllDots[column - 2, row] != null)
            {
                if (AllDots[column - 1, row].tag == piece.tag && AllDots[column - 2, row].tag == piece.tag)
                {
                    return true;
                }
            }
            if (AllDots[column, row - 1] != null && AllDots[column, row - 2] != null)
            {
                if (AllDots[column, row - 1].tag == piece.tag && AllDots[column, row - 2].tag == piece.tag)
                {
                    return true;
                }
            }

        }
        else if (column <= 1 || row <= 1)
        {
            if (row > 1)
            {
                if (AllDots[column, row - 1] != null && AllDots[column, row - 2] != null)
                {
                    if (AllDots[column, row - 1].tag == piece.tag && AllDots[column, row - 2].tag == piece.tag)
                    {
                        return true;
                    }
                }
            }
            if (column > 1)
            {
                if (AllDots[column - 1, row] != null && AllDots[column - 2, row] != null)
                {
                    if (AllDots[column - 1, row].tag == piece.tag && AllDots[column - 2, row].tag == piece.tag)
                    {
                        return true;
                    }
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
        StartCoroutine(DecreasoRowCo2());
    }

    private IEnumerator DecreasoRowCo2()
    {
        for(int i = 0; i < width; i++)
        {
            for(int j = 0; j < height;  j ++)
            {
                //if the current spot is not blnak and is empty
                if (!BlankSpaces[i,j] && AllDots[i,j] == null)
                {
                    //loop from the space above to the top of column
                    for(int k = j+1; k < height; k++)
                    {
                        //if that dot is found
                        if (AllDots[i,k] != null)
                        {
                            //move that dot to empty space
                            AllDots[i, k].GetComponent<Dot>().row = j;
                            //sett to be null
                            AllDots[i, k] = null;
                            //break out of loof
                            break;
                        }
                    }
                }
            }
        }
        yield return new WaitForSeconds(.4f);
        StartCoroutine(FillboardCo());
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
                if (AllDots[i,j] == null && !BlankSpaces[i,j])
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
