using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceManager : MonoBehaviour
{
    public GameObject piecePrefab;
    public UIManager uiManager;

    public bool isBlackTurn;
    public int pieceLimit = 12;

    public List<Piece> whitePieces = null;
    public List<Piece> blackPieces = null;

    public bool canKillOther = false;

    public int prevWhite = 12, prevBlack = 12;

    //1 czarne
    //0 biale
    public bool p1i = true;
    public bool p2i = false;

    private string[] pieceOrder = new string[5]
    {
        "R", "R", "R", "R", "R"
    };

    private Dictionary<string, Type> pieceLib = new Dictionary<string, Type>()
    {
        {"R", typeof(Rock) }
    };

    public void Setup(Board board)
    {
        SwitchPlayer(Color.black);
    }

    public void SetInteractive(List<Piece> allPieces, bool value)
    {
        foreach(Piece p in allPieces)
        {
            p.enabled = value;
        }
    }

    public void SwitchPlayer(Color color)
    {
        isBlackTurn = color == Color.white ? true : false;

        if (isBlackTurn)
        {
            uiManager.SetTurn(false);
            prevBlack = 12 - blackPieces.Count;
        }
        else {
            uiManager.SetTurn(true);
            prevWhite = 12 - whitePieces.Count;
        }


        SetInteractive(whitePieces, !isBlackTurn);
        SetInteractive(blackPieces, isBlackTurn);
    }

    private List<Piece> CreatePieces(Color teamColor, Color32 spriteColor, Board board)
    {
        List<Piece> pieces = new List<Piece>();
        for(int i = 0; i < 12; i++)
        {
            GameObject newPieceObject = Instantiate(piecePrefab, transform);
            newPieceObject.transform.SetParent(transform);

            newPieceObject.transform.localScale = new Vector3(1, 1, 1);
            newPieceObject.transform.localRotation = Quaternion.identity;

            string key = "R";
            Type pieceType = pieceLib[key];

            Piece newPiece = (Piece)newPieceObject.AddComponent(pieceType);
            //newPiece.piecePrefab = newPieceObject;

            pieces.Add(newPiece);

            newPiece.Setup(teamColor, spriteColor, this);
        }
        return pieces;


    }


    private void PlacePieces(int row, List<Piece> pieces, Board board)
    {
        for(int i = 0; i < 5; i++)
        {
            pieces[i].Place(board.cells[i, row]);
        }
    }

    public void PlacePiece(Cell cell, List<Piece> pieces, Color teamColor, Color32 spriteColor, Board board)
    {
        //jesli nie wykorzystalismy wszystkich pionkow
        if (pieces.Count < pieceLimit)
        {
            GameObject newPieceObject = Instantiate(piecePrefab);
            newPieceObject.transform.SetParent(transform);

            newPieceObject.transform.localScale = new Vector3(1, 1, 1);
            newPieceObject.transform.localRotation = Quaternion.identity;

            //to zmienie pozniej
            string key = "R";
            Type pieceType = pieceLib[key];

            Piece newPiece = (Piece)newPieceObject.AddComponent(pieceType);
            //dodajemy do listy pionkow stworzony
            pieces.Add(newPiece);

            //ustawiamy druzyne  i kolor
            newPiece.Setup(teamColor, spriteColor, this);

            //kladziemy na planszy
            newPiece.Place(cell);
            
        }
        
    }

}
