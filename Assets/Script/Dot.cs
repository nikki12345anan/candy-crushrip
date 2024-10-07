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

    private HintManager hintmanager;
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

        hintmanager = FindObjectOfType<HintManager>();
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
            FindObjectOfType<SoundManager>().play("Geri al");

            IsColorBomb = true;
            GameObject color = Instantiate(Colorbomb, transform.position, Quaternion.identity);
            color.transform.parent = this.transform;
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
            findMatches.MatchPiecesOfColor(OtherDot.tag);
            IsMatched = true;
        }
        else if (OtherDot.GetComponent<Dot>().IsColorBomb)
        {
            //other one is the color bomb this one is to desroy color
            findMatches.MatchPiecesOfColor(this.gameObject.tag);
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
        //ip ucunu yok et
        if(hintmanager != null)
        {
            hintmanager.DestroyHint();
        }

        if(Board.currentState == GameState.move)
        {
            FirstTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
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
            Movepieces();
            Board.currentdot = this;
            Board.currentState = GameState.wait;

        }
        else
        {
            Board.currentState = GameState.move;
        }
        
    }

    void MovePiecesActual(Vector2 direction)
    {
        OtherDot = Board.AllDots[Column + (int)direction.x, row + (int)direction.y];
        PreviousRow = row;
        PreviousColumn = Column;
        if(OtherDot != null){
            OtherDot.GetComponent<Dot>().Column += -1 * (int)direction.x;
            OtherDot.GetComponent<Dot>().row += -1 * (int)direction.y;
            row += (int)direction.y;
            Column += (int)direction.x;
            StartCoroutine(CheckMoveCO());
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
            MovePiecesActual(Vector2.right);
        }
        else if(SwipeAngle > 45 && SwipeAngle <= 135 && row < Board.height-1)
        {
            //yukari kaydirma
            

            MovePiecesActual(Vector2.up);

        }
        else if (SwipeAngle > 135 || SwipeAngle <= -135 && Column > 0)
        {
            //sol kaydirma
            MovePiecesActual(Vector2.left);

        }
        else if (SwipeAngle < -45 && SwipeAngle >= -135 && row > 0)
        {
            //alt kaydirma
            MovePiecesActual(Vector2.down);

        }
        Board.currentState = GameState.move;
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

    public void MakeColorBomb()
    {
        IsColorBomb = true;
        GameObject color = Instantiate(Colorbomb, transform.position, Quaternion.identity);
        color.transform.parent = this.transform;
        this.gameObject.tag = "color";
    }
    public void MakeAdjacentBomb()
    {
        IsAdjacentBomb = true;
        GameObject marker = Instantiate(AdjacentMark, transform.position, Quaternion.identity);
        marker.transform.parent = this.transform;
    }
}
