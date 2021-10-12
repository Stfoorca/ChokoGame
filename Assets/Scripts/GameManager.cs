using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.UIElements;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public Board board;
    //public UIManager uim;
    public int globalDepth;

    public bool hasToPlace = false;
    private int prevTurn;
    public int turn = 1;

    public Piece holdingPiece;
    public Piece destPieceWhenAttack;
    public bool canKill = false;

    public float turnTimer = 0f;

    private Stopwatch watch = new System.Diagnostics.Stopwatch();

    private MenuController menu;
    public bool menuActive = false;
    public GameObject gameEndPanel;

    void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        menu = GameObject.Find("MenuController").GetComponent<MenuController>();

        globalDepth = menu.depth;
        
        board.Setup();
        board.PrintBoard();
        if (!menu.debugMode)
        {
            UIManager.instance.textPanel.SetActive(false);
        }
        else
        {
            UIManager.instance.textPanel.SetActive(true);
        }
    }


    public void GameEnd()
    {
        gameEndPanel.SetActive(true);
        menuActive = true;
    }

    public void PlayAgain()
    {
        SceneManager.LoadScene("test");
        
    }
    public void ReturnToMenu()
    {
        SceneManager.LoadScene("menu");
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && turn == 1)
        {
            //Get the mouse position on the screen and send a raycast into the game world from that position.
            Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);

            //If something was hit, the RaycastHit2D.collider will not be null.
            if (hit.collider != null)
            {
                ButtonClicked(hit.collider.gameObject);
            }
        }

        else if (Input.GetMouseButtonDown(1) && turn == 1)
        {
            ButtonUnclicked();
        }
        else if (Input.GetKeyDown("escape"))
        {
            gameEndPanel.SetActive(!menuActive);
            menuActive = !menuActive;
        }
        UIManager.instance.SetHand(1, board.maxPieces - board.whitePieces);
        UIManager.instance.SetHand(2, board.maxPieces - board.blackPieces);
    }

    public void ButtonUnclicked()
    {
        if (destPieceWhenAttack != null && !canKill)
        {
            destPieceWhenAttack = null;
        }
        else
        {
            board.pieces[holdingPiece.x, holdingPiece.y].GetComponent<SpriteRenderer>().sprite =
                board.player_prefab;
            holdingPiece = null;
        }
    }
    public void PrintMove(MoveController move)
    {

        string player = move.team == 1 ? "Player" : "AI";
        string moveType = move.MoveType.ToString();
        string moveDetails = "";
        if (moveType == "Place")
            moveDetails = "at [" + move.target.x + ", " + move.target.y + "]";
        else if (moveType == "Move")
            moveDetails = "from [" + move.target.x + ", " + move.target.y + "] to [" + move.direction.x + ", " +
                          move.direction.y + "]";
        else if (moveType == "Kill")
            moveDetails = "from [" + move.target.x + ", " + move.target.y + "] to [" + move.direction.x + ", " +
                          move.direction.y + "], killed between [" + move.betweenKill.x + ", " + move.betweenKill.y +
                          "] and killed [" + move.pieceToKill.x + ", " + move.pieceToKill.y + "]";
        UIManager.instance.SetGameStatusText(player + " made move " + moveDetails);
        Debug.Log("Player [" + move.team + "]\n" +
                  "CurrentPos [" + move.target.x + "," + move.target.y + "]\n" +
                  "TargetPos [" + move.direction.x + "," + move.direction.y + "]\n" +
                  "BetweenPos [" + move.betweenKill.x + "," + move.betweenKill.y + "]\n" +
                  "ChosenKillPos [" + move.pieceToKill.x + "," + move.pieceToKill.y);
    }
    
    public void makeBotMove()
    {
        MoveController move;

        Board c_board = Board.OwnDeepCopy(board);
        bool hasToPlace_copy = hasToPlace;

        watch.Start();

        Tuple<MoveController, int> LUL = AIAIAIAIAI.instance.minimax(c_board, globalDepth, -2137214000, 2137214000, 2, hasToPlace_copy);
        watch.Stop();
        turnTimer = (float)watch.ElapsedMilliseconds/1000;
        watch.Reset();
        UIManager.instance.SetGameStatusText("AI MYSLAL " + turnTimer.ToString() + " sekund");
        move = LUL.Item1;
        string moveType = "";
        if (move.MoveType == MoveController.Move.Place)
        {
            moveType = "Place";
          
        }
        else if (move.MoveType == MoveController.Move.Move)
        {
            moveType = "Move";
        }
        else if (move.MoveType == MoveController.Move.Kill)
        {
            moveType = "Kill";
        }
        if (Board.instance.MakeMove(move))
        {
            if (moveType == "Place")
            {
                hasToPlace = !hasToPlace;
            }
            PrintMove(move);
            //Board.instance.UpdateBoardLogic();
            ChangeTurn();
        }

    }
    public void ChangeTurn()
    {
        if (GameManager.instance.menuActive)
            return;

        if (GameManager.instance.turn == 1)
        {
            GameManager.instance.turn = 2;
            UIManager.instance.SetTurn(2);

            makeBotMove();
            //StartCoroutine(GameManager.instance.makeBotMove());
        }
        else
        {
            GameManager.instance.turn = 1;
            UIManager.instance.SetTurn(1);
        }
        UIManager.instance.SetHasToPlaceText(hasToPlace);
    }

    public void ButtonClicked(GameObject button)
    {
        Piece buttonPiece = button.GetComponent<Piece>();
        int x = buttonPiece.x;
        int y = buttonPiece.y;
        MoveController move;

        if (canKill)
        {
            Vector2Int curTemp = new Vector2Int(holdingPiece.x, holdingPiece.y);
            Vector2Int destTemp = new Vector2Int(destPieceWhenAttack.x, destPieceWhenAttack.y);
            Vector2Int killTemp = new Vector2Int(x, y);

            int tempX = 0, tempY = 0;
            if (curTemp.x == destTemp.x)
            {
                tempX = curTemp.x;
                tempY = Mathf.Abs(curTemp.y + destTemp.y) / 2;
            }
            else if (curTemp.y == destTemp.y)
            {
                tempX = Mathf.Abs(curTemp.x + destTemp.x) / 2;
                tempY = curTemp.y;
            }

            Vector2Int betweenTemp = new Vector2Int(tempX, tempY);
            if (betweenTemp == killTemp)
                return;

            move = new MoveController(MoveController.Move.Kill, turn, curTemp, destTemp, killTemp, betweenTemp);
            if (Board.instance.MakeMove(move))
            {
                PrintMove(move);
                //Board.instance.UpdateBoardLogic();
                ChangeTurn();
            }

            canKill = false;
            holdingPiece = null;
            destPieceWhenAttack = null;
        }

        else if (holdingPiece != null)
        {
            Vector2Int temp = new Vector2Int(holdingPiece.x, holdingPiece.y);

            if (IsMoveLegal(holdingPiece, buttonPiece))
            {
                move = new MoveController(MoveController.Move.Move, turn, temp, new Vector2Int(x, y));

                if (Board.instance.MakeMove(move))
                {
                    PrintMove(move);
                   //Board.instance.UpdateBoardLogic();
                    ChangeTurn();
                }

                holdingPiece = null;
            }
            else if (IsAttackLegal(holdingPiece, buttonPiece))
            {
                destPieceWhenAttack = buttonPiece;
                canKill = true;
            }
        }
        else
        {
            if (buttonPiece.team == 0 && (board.maxPieces - board.whitePieces) > 0)
            {
                move = new MoveController(MoveController.Move.Place, turn, new Vector2Int(x, y));
                
                hasToPlace = !hasToPlace;
                if (Board.instance.MakeMove(move))
                {
                    PrintMove(move);
                    //Board.instance.UpdateBoardLogic();
                    ChangeTurn();
                }


            }
            else if (buttonPiece.team != 0)
            {
                if (hasToPlace && (board.maxPieces - board.whitePieces) > 0)
                {
                    Debug.Log("Player [" + turn + "] has to place piece");
                    return;
                }
                else
                {
                    if ((turn == 1 && buttonPiece.team == 1) || (turn == 2 && buttonPiece.team == 2))
                    {
                        holdingPiece = buttonPiece;
                        board.pieces[holdingPiece.x, holdingPiece.y].GetComponent<SpriteRenderer>().sprite =
                            board.player_highlighted_prefab;
                    }
                }
            }
        }
    }

    public bool IsMoveLegal(Piece cur, Piece dest)
    {
        if (dest.team != 0)
            return false;

        if (Mathf.Abs(cur.x - dest.x) != 1 && Mathf.Abs(cur.y - dest.y) != 1)
            return false;

        if (cur.x != dest.x && cur.y != dest.y)
            return false;

        return true;
    }

    public bool IsAttackLegal(Piece cur, Piece dest)
    {
        if (dest.team != 0)
            return false;

        if (Mathf.Abs(cur.x - dest.x) != 2 && Mathf.Abs(cur.y - dest.y) != 2)
            return false;

        if (cur.x != dest.x && cur.y != dest.y)
            return false;

        int tempX = (cur.x + dest.x) / 2;
        int tempY = (cur.y + dest.y) / 2;
        int enemyTeam = cur.team == 1 ? 2 : 1;

        if (board.pieces[tempX, cur.y].team != enemyTeam && board.pieces[cur.x, tempY].team != enemyTeam)
            return false;

        return true;
    }
}