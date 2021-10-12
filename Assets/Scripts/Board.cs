using System;
using System.Collections;
using System.Collections.Generic;
using System.Transactions;
using UnityEngine;


[System.Serializable]
public class Board : MonoBehaviour
{
    public static Board instance;
    public Sprite player_prefab, enemy_prefab, empty_prefab, player_highlighted_prefab;

    public GameObject piecePrefab;
    public static int boardSize = 5;
    public Piece[,] pieces = new Piece[boardSize, boardSize];
    public List<Piece> whitePiecesLocations;
    public List<Piece> blackPiecesLocations;
    public int maxPieces = 12, whitePieces = 0, blackPieces = 0, p1PiecesDead = 0, p2PiecesDead = 0;


    public static Board OwnDeepCopy(Board myCopy)
    {
        Board ret = new Board();
        ret.pieces = myCopy.pieces.Copy();
        ret.blackPieces = myCopy.blackPieces.Copy();
        ret.whitePieces = myCopy.whitePieces.Copy();
        ret.p1PiecesDead = myCopy.p1PiecesDead.Copy();
        ret.p2PiecesDead = myCopy.p2PiecesDead.Copy();
        ret.piecePrefab = myCopy.piecePrefab.Copy();
        ret.blackPiecesLocations = myCopy.blackPiecesLocations.Copy();
        ret.whitePiecesLocations = myCopy.whitePiecesLocations.Copy();
        return ret;
    }

    void Start()
    {
        instance = this;

    }

    public void Setup()
    {
        for (int y = 0; y < boardSize; y++)
        {
            for (int x = 0; x < boardSize; x++)
            {
                GameObject obj = Instantiate(piecePrefab, new Vector3(-4f + x * 2.1f, 5 + -y * 2.1f), Quaternion.identity, this.transform);
                obj.name = x.ToString() + y.ToString();
                obj.GetComponent<Piece>().x = x;
                obj.GetComponent<Piece>().y = y;
                pieces[x, y] = obj.GetComponent<Piece>();
                pieces[x, y].GetComponent<SpriteRenderer>().sprite = empty_prefab;
            }
        }
    }

    public void PrintBoard()
    {
        string board = "";
        for (int y = 0; y < boardSize; y++)
        {
            string row = "";
            for (int x = 0; x < boardSize; x++)
            {
                row += (pieces[x, y].team).ToString() + " ";
            }
            board += row + "\n";
        }
        Debug.Log(board);
    }

    public bool MakeMove(MoveController moveController)
    {
        //WTF xD
        if (moveController.MoveType == MoveController.Move.Place)
        {
            Piece tempPiece = pieces[moveController.target.x, moveController.target.y];
            if (tempPiece.team == 0)
            {
                tempPiece.team = moveController.team;
                if (moveController.team == 1)
                {
                    //tempPiece.GetComponent<SpriteRenderer>().color = Color.magenta;
                    whitePieces++;
                    UIManager.instance.SetHand(1, maxPieces-whitePieces);
                }
                else if (moveController.team == 2)
                {
                    blackPieces++;
                   // tempPiece.GetComponent<SpriteRenderer>().color = Color.black;
                    UIManager.instance.SetHand(2, maxPieces - blackPieces);
                }


                //PrintBoard();

                UpdateBoardLogic();
                return true;
            }
        }
        else if (moveController.MoveType == MoveController.Move.Move)
        {
            Piece current = pieces[moveController.target.x, moveController.target.y];
            Piece destination = pieces[moveController.direction.x, moveController.direction.y];
            //current.GetComponent<SpriteRenderer>().color = Color.white;
                
            Color color = new Color();

            if (current.team == 1)
            {
                color = Color.magenta;
            }
            else if (current.team == 2)
            {
                color = Color.black;
            }

            //destination.GetComponent<SpriteRenderer>().color = color;
            destination.team = current.team;
            current.team = 0;
            // PrintBoard();
            UpdateBoardLogic();
            return true;
        }
        else if (moveController.MoveType == MoveController.Move.Kill)
        {

            Piece current = pieces[moveController.target.x, moveController.target.y];
            Piece destination = pieces[moveController.direction.x, moveController.direction.y];
            Piece toKill = pieces[moveController.pieceToKill.x, moveController.pieceToKill.y];
            Piece betweenKill = pieces[moveController.betweenKill.x, moveController.betweenKill.y];

            Color color = new Color();

            if (current.team == 1)
            {
                color = Color.magenta;
            }
            else if (current.team == 2)
            {
                color = Color.black;
            }

            //destination.GetComponent<SpriteRenderer>().color = color;
            destination.team = current.team;     
           
            //current.GetComponent<SpriteRenderer>().color = Color.white;
            
            int destroyed = 0;
            if (toKill.team != current.team && toKill.team != 0 && betweenKill.team != current.team &&
                betweenKill.team != 0)
            {
                destroyed = 2;
            }
            else
            {
                destroyed = 1;
            }
            //toKill.GetComponent<SpriteRenderer>().color = Color.white;
            toKill.team = 0;

           // betweenKill.GetComponent<SpriteRenderer>().color = Color.white;
            betweenKill.team = 0;
            if (current.team == 1)
            {
                p2PiecesDead += destroyed;
            }
            else if (current.team == 2)
            {
                p1PiecesDead += destroyed;
            }
            int end = CheckEndGameCondition();
            if (end == 0)
            {
                UIManager.instance.SetWinner(0);
                GameManager.instance.GameEnd();
                return true;
            }
            else if (end == 1)
            {
                UIManager.instance.SetWinner(1);
                GameManager.instance.GameEnd();
                return true;
            }
            else if (end == 2)
            {
                UIManager.instance.SetWinner(2);
                GameManager.instance.GameEnd();
                return true;
            }

            current.team = 0;
            // PrintBoard();
            UpdateBoardLogic();
            return true;
        }
        return false;
    }

    public void UpdateBoardLogic()
    {
        whitePiecesLocations.Clear();
        blackPiecesLocations.Clear();
        for (int x = 0; x < 5; x++)
        {
            for (int y = 0; y < 5; y++)
            {
                if (pieces[x, y].team == 0)
                {
                    pieces[x, y].GetComponent<SpriteRenderer>().sprite = empty_prefab;
                }
                else if (pieces[x, y].team == 1)
                {
                    pieces[x, y].GetComponent<SpriteRenderer>().sprite = player_prefab;
                    whitePiecesLocations.Add(pieces[x, y]);
                }
                if (pieces[x, y].team == 2)
                {
                    pieces[x, y].GetComponent<SpriteRenderer>().sprite = enemy_prefab;
                    blackPiecesLocations.Add(pieces[x, y]);
                }
            }
        }
    }
    
    public int CheckEndGameCondition()
    {
        int whiteAlive = 0, blackAlive = 0;
        for (int x = 0; x < 5; x++)
        {
            for (int y = 0; y < 5; y++)
            {
                if (pieces[x, y].team == 1)
                {
                    whiteAlive++;
                }
                else if (pieces[x, y].team == 2)
                {
                    blackAlive++;
                }
            }
        }

        if (whiteAlive + (12-whitePieces) <= 0 && blackAlive + (12-blackPieces) > 3)
            return 2;
        else if (whiteAlive + maxPieces - whitePieces > 3 && blackAlive + maxPieces - blackPieces <= 0)
            return 1;
        else if (whiteAlive + maxPieces - whitePieces <= 3 && blackAlive + maxPieces - blackPieces <= 3)
            return 0;
        return -1;
    }
}