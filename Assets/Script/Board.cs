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
    public GameObject BreakableTilePreFab;
    public GameObject[] dots;
    public GameObject DestroyEffect;
    public TileType[] BoardLayout;
    private bool[,] BlankSpaces;
    private Backgroundtile[,] Breakabletiles;
    public GameObject[,] AllDots;
    public Dot currentdot;
    private FindMatches findMatches;
    public int BasePieceValue = 20;
    private int StreakValue = 1;
    private scoreManager scoremanager;
    private SoundManager soundmanager;
    public float refillDelay = 0.5f;
    public int[] scoreGoals;

    // Start is called before the first frame update
    void Start()
    {
        soundmanager = FindObjectOfType<SoundManager>();
        scoremanager = FindObjectOfType<scoreManager>();
        Breakabletiles = new Backgroundtile[width, height];
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
    
    public void GenerateBreakableTiles()
    {
        //look at all the tiles in the layout

        for (int i = 0; i < BoardLayout.Length; i++)
        {
            //if jelly tile
            if (BoardLayout[i].tilekind == TileKind.Breakable)
            {
                //create jelly tile
                Vector2 tempPosition = new Vector2(BoardLayout[i].x, BoardLayout[i].y);
                GameObject tile = Instantiate(BreakableTilePreFab,tempPosition, Quaternion.identity);
                Breakabletiles[BoardLayout[i].x, BoardLayout[i].y] = tile.GetComponent<Backgroundtile>();
            }
        }
    }

    private void SetUp()
    {
        GenerateBlankSpace();
        GenerateBreakableTiles();
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (!BlankSpaces[i, j])
                {
                    Vector2 tempPosition = new Vector2(i, j + OffSet);
                    Vector2 tilePosition = new Vector2(i, j);
                    GameObject backgroundTile = Instantiate(TilePrefab, tilePosition, Quaternion.identity) as GameObject;
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
                //is the current dot matched
                if(currentdot != null)
                {
                    if (!currentdot.IsColorBomb)
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
                //is the current dot matched
                if (currentdot != null)
                {
                    if (!currentdot.IsAdjacentBomb)
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

            //does a tile need to break
            if (Breakabletiles[column,row] != null)
            {
                Breakabletiles[column, row].TakeDamage(1);
                if (Breakabletiles[column,row].HitPoints <= 0)
                {
                    Breakabletiles[column, row] = null;
                }
            }
            //ses var mý
            FindObjectOfType<SoundManager>().play("Geri al");

            
            GameObject particle = Instantiate(DestroyEffect, AllDots[column, row].transform.position, Quaternion.identity);
            Destroy(particle, .5f);
            Destroy(AllDots[column, row]);
            scoremanager.IncreaseScore(BasePieceValue * StreakValue);
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
        yield return new WaitForSeconds(refillDelay * 0.5f);
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
        yield return new WaitForSeconds(refillDelay * 0.5f);
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
                    int dotToUse = Random.Range(0, dots.Length);
                    int maxIterations = 0;

                    while (MatchesAt(i, j, dots[dotToUse]) && maxIterations < 100)
                    {
                        maxIterations++;
                        dotToUse = Random.Range(0, dots.Length);
                    }
                    maxIterations = 0;

                    GameObject piece = Instantiate(dots[dotToUse], tempposition, Quaternion.identity);
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
        yield return new WaitForSeconds(refillDelay);

        while (MatchesOnBoard())
        {
            StreakValue ++;
            DestroyMatches();
            yield return new WaitForSeconds(2 * refillDelay);
        }
        findMatches.currentMatches.Clear();
        currentdot = null;

        if (IsDeadLocked())
        {
            ShuffleBoard();
            Debug.Log("deadlocked HAMBURGERRRRRR");
        }
        yield return new WaitForSeconds(refillDelay);
        currentState = GameState.move;

        if(StreakValue >= 2)
        {
            Debug.Log("streak");
        }
        StreakValue = 1;

    }

    private void SwitchPieces(int column, int row, Vector2 direction)
    {
        //take first piece and save it in holder
        GameObject holder = AllDots[column + (int)direction.x, row + (int)direction.y] as GameObject;


        // switching the first dot to be second position
        AllDots[column + (int)direction.x, row + (int)direction.y] = AllDots[column, row];


        // set the first dot to be second dot
        AllDots[column, row] = holder;
    }

    private bool CheckForMatches()
    {
        for(int i = 0; i < width; i++)
        {
            for(int j = 0; j < height; j++)
            {
                if (AllDots[i,j] != null)
                {
                    // make sure that one ant two to the right are in the board
                    if (i < width - 2)
                    {


                        //check if dots to the rigt and two right make a match
                        if (AllDots[i + 1, j] != null && AllDots[i + 2, j] != null)
                        {
                            if (AllDots[i + 1, j].tag == AllDots[i, j].tag && AllDots[i + 2, j].tag == AllDots[i, j].tag)
                            {
                                return true;
                            }
                        }
                    }
                    if (j < height - 2)
                    {
                        if (AllDots[i, j + 1] != null && AllDots[i, j + 2] != null)
                        {
                            if (AllDots[i, j + 1].tag == AllDots[i, j].tag && AllDots[i, j + 2].tag == AllDots[i, j].tag)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
        }
        return false;
    }

    public bool SwitchAndCheck(int column, int row, Vector2 direction)
    {
        SwitchPieces(column, row, direction);
        if (CheckForMatches())
        {
            SwitchPieces(column, row, direction);
            return true;
        }
        SwitchPieces(column, row, direction);
        return false;
    }

    private bool IsDeadLocked()
    {
        for(int i = 0; i < width; i++)
        {
            for(int j = 0; j < height; j++)
            {
                if (AllDots[i, j] != null)
                {


                    if (i < width - 1)
                    {
                        if (SwitchAndCheck(i, j, Vector2.right))
                        {
                            return false;
                        }
                    }
                    if (j < height - 1)
                    {
                        if (SwitchAndCheck(i, j, Vector2.up))
                        {
                            return false;
                        }
                    }
                }
            }
        }
        return true;
    }
    private void ShuffleBoard()
    {
        //create list
        List<GameObject> newBoard = new List<GameObject>();
        //add pieces to list
        for(int i = 0; i < width; i++)
        {
            for(int j = 0; j < height; j++)
            {
                if (AllDots[i,j] != null)
                {
                    newBoard.Add(AllDots[i, j]);
                }
            }
        }
        //her parça için
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                //if spot should not be blank
                if (!BlankSpaces[i, j])
                {
                    //pick random number
                    int PieceToUse = Random.Range(0, newBoard.Count);
                    
                    int maxIterations = 0;
                    while (MatchesAt(i, j, newBoard[PieceToUse]) && maxIterations < 100)
                    {
                        PieceToUse = Random.Range(0, newBoard.Count);
                        maxIterations++;
                        Debug.Log(maxIterations);
                    }
                    maxIterations = 0;

                    //make a container for piece
                    Dot piece = newBoard[PieceToUse].GetComponent<Dot>();

                    //assign column and row
                    piece.Column = i;
                    piece.row = j;
                    //fill in the dots array with this new piece
                    AllDots[i, j] = newBoard[PieceToUse];
                    //remove from list
                    newBoard.Remove(newBoard[PieceToUse]);
                }
            }
        }
        //check and repeat if still locked
        if (IsDeadLocked())
        {
            ShuffleBoard();
        }

    }
}
