using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dot : MonoBehaviour
{
    [Header("Board variabeles")]
    public int Column;
    public int row;
    public int PreviousColumn;
    public int PreviousRow;
    public int TargetX;
    public int TargetY;
    public bool IsMatched = false;


    private FindMatches findMatches;
    private Board Board;
    public GameObject OtherDot;
    private Vector2 FirstTouchPosition;
    private Vector2 LastTouchPosition;
    private Vector2 TempPosition;

    [Header("Swipe stuff")]
    public float SwipeAngle = 0;
    public float SwipeResist = 1f;

    [Header("Powerup swtuff")]
    public bool IsColorBomb;
    public bool IsColumnBomb;
    public bool IsRowBomb;
    public bool IsAdjacentBomb;
    public GameObject RowArrow;
    public GameObject ColumnArrow;
    public GameObject Colorbomb;
    public GameObject AdjacentMark;


    // Start is called before the first frame update
    void Start()
    {
        IsColumnBomb = false;
        IsRowBomb = false;
        IsColorBomb = false;
        IsAdjacentBomb = false;

        Board = FindObjectOfType<Board>();
        findMatches = FindObjectOfType<FindMatches>();
        //TargetX = (int)transform.position.x;
        //TargetY = (int)transform.position.y;
        //Column = TargetX;
        //row = TargetY;
        //PreviousRow = row;
        //PreviousColumn = Column;
    }

    //Test debug only 

    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(1))
        {
            IsAdjacentBomb = true;
            GameObject marker = Instantiate(AdjacentMark, transform.position, Quaternion.identity);
            marker.transform.parent = this.transform;
        }
    }

    // Update is called once per frame
    void Update()
    {

        /*
        if (IsMatched)
        {
            SpriteRenderer mySprite = GetComponent<SpriteRenderer>();
            mySprite.color = new Color(0f, 0f, 0f, 2f);
        }
        */

        TargetX = Column;
        TargetY = row;
        if (Mathf.Abs(TargetX - transform.position.x) > .1)
        {
            //hedefe git
            TempPosition = new Vector2(TargetX, transform.position.y);
            transform.position = Vector2.Lerp(transform.position, TempPosition, .6f);
            if (Board.AllDots[Column,row] != this.gameObject)
            {
                Board.AllDots[Column, row] = this.gameObject;

            }
            findMatches.FindAllMatches();
        }
        else
        {
            TempPosition = new Vector2(TargetX, transform.position.y);
            transform.position = TempPosition;
        }
        if (Mathf.Abs(TargetY - transform.position.y) > .1)
        {
            //hedefe git
            TempPosition = new Vector2(transform.position.x, TargetY);
            transform.position = Vector2.Lerp(transform.position, TempPosition, .6f);
            if (Board.AllDots[Column, row] != this.gameObject)
            {
                Board.AllDots[Column, row] = this.gameObject;

            }
            findMatches.FindAllMatches();

        }
        else
        {
            TempPosition = new Vector2(transform.position.x, TargetY);
            transform.position = TempPosition;
        }
    }
    
    public IEnumerator CheckMoveCO()
    {
        if (IsColorBomb)
        {
            //this one is the volor bomb and the other is the one to destroy
            findMatches.MatchPiecesOfColr(OtherDot.tag);
            IsMatched = true;
        }
        else if (OtherDot.GetComponent<Dot>().IsColorBomb)
        {
            //other one is the color bomb this one is to desroy color
            findMatches.MatchPiecesOfColr(this.gameObject.tag);
            OtherDot.GetComponent<Dot>().IsMatched = true;

        }
        yield return new WaitForSeconds(.5f);
        if(OtherDot != null)
        {
            if(!IsMatched && !OtherDot.GetComponent<Dot>().IsMatched)
            {
                OtherDot.GetComponent<Dot>().row = row;
                OtherDot.GetComponent<Dot>().Column = Column;
                row = PreviousRow;
                Column = PreviousColumn;
                yield return new WaitForSeconds(.5f);
                Board.currentdot = null;
                Board.currentState = GameState.move;
            }
            else
            {
                Board.DestroyMatches();
            }
            //OtherDot = null;
        }
        
    }
    private void OnMouseDown()
    {
        if(Board.currentState == GameState.move)
        {
            FirstTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Debug.Log(FirstTouchPosition);
        }
        
    }

    private void OnMouseUp()
    {
        if(Board.currentState == GameState.move)
        {
            LastTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            CalculateAngle();
        }
        
    }

    void CalculateAngle()
    {
        if(Mathf.Abs(LastTouchPosition.y -FirstTouchPosition.y) > SwipeResist ||Mathf.Abs(LastTouchPosition.x -FirstTouchPosition.x) > SwipeResist)
        {
            SwipeAngle = Mathf.Atan2(LastTouchPosition.y - FirstTouchPosition.y, LastTouchPosition.x - FirstTouchPosition.x) * 180 / Mathf.PI;
            Debug.Log(SwipeAngle);
            Movepieces();
            Board.currentState = GameState.wait;
            Board.currentdot = this;

        }
        else
        {
            Board.currentState = GameState.move;
        }
        
    }
    void Movepieces()
    {
        if(SwipeAngle > -45 && SwipeAngle <= 45 && Column < Board.width-1)
        {
            //sag kaydirma
            OtherDot = Board.AllDots[Column + 1, row];
            PreviousRow = row;
            PreviousColumn = Column;
            OtherDot.GetComponent<Dot>().Column -= 1;
            Column += 1;
        }
        else if(SwipeAngle > 45 && SwipeAngle <= 135 && row < Board.height-1)
        {
            //yukari kaydirma
            OtherDot = Board.AllDots[Column, row + 1];
            PreviousRow = row;
            PreviousColumn = Column;
            OtherDot.GetComponent<Dot>().row -= 1;
            row += 1;
        }
        else if (SwipeAngle > 135 || SwipeAngle <= -135 && Column > 0)
        {
            //sol kaydirma
            OtherDot = Board.AllDots[Column - 1, row];
            PreviousRow = row;
            PreviousColumn = Column;
            OtherDot.GetComponent<Dot>().Column += 1;
            Column -= 1;
        }
        else if (SwipeAngle < -45 && SwipeAngle >= -135 && row > 0)
        {
            //alt kaydirma
            OtherDot = Board.AllDots[Column, row - 1];
            PreviousRow = row;
            PreviousColumn = Column;
            OtherDot.GetComponent<Dot>().row += 1;
            row -= 1;
        }
        StartCoroutine(CheckMoveCO());
    }

    void FindMatches()
    {
        if(Column > 0 && Column < Board.width - 1)
        {
            GameObject LeftDot1 = Board.AllDots[Column - 1, row];
            GameObject RightDot1 = Board.AllDots[Column + 1, row];
            if(LeftDot1 != null && RightDot1 != null)
            {
                if (LeftDot1.tag == this.gameObject.tag && RightDot1.tag == this.gameObject.tag)
                {
                    LeftDot1.GetComponent<Dot>().IsMatched = true;
                    RightDot1.GetComponent<Dot>().IsMatched = true;
                    IsMatched = true;
                }
            }
            
        }

        if (row > 0 && row < Board.height - 1)
        {
            GameObject UpDot1 = Board.AllDots[Column, row + 1];
            GameObject DownDot1 = Board.AllDots[Column, row - 1];
            if(UpDot1 != null && DownDot1 != null)
            {
                if (UpDot1.tag == this.gameObject.tag && DownDot1.tag == this.gameObject.tag)
                {
                    UpDot1.GetComponent<Dot>().IsMatched = true;
                    DownDot1.GetComponent<Dot>().IsMatched = true;
                    IsMatched = true;
                }
            }
            
        }
    }
    public void MakeRowbomb()
    {
        IsRowBomb = true;
        GameObject arrow = Instantiate(RowArrow, transform.position, Quaternion.identity);
        arrow.transform.parent = this.transform;
    }
    public void MakeColumnbomb()
    {
        IsColumnBomb = true;
        GameObject arrow = Instantiate(ColumnArrow, transform.position, Quaternion.identity);
        arrow.transform.parent = this.transform;
    }
}
