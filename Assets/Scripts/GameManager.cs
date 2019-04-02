using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public Board board;
    public PieceManager pieceManager;
    public UIManager uiManager;


    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;
        board.CreateBoard();

        pieceManager.Setup(board);
    }

    // Update is called once per frame
    void Update()
    {
        //Spradzamy czy czarny wygral
        if(int.Parse(uiManager.whitePiecesDead.text) == 12 && pieceManager.whitePieces.Count !=0)
        {
            uiManager.wonText.text = "CZARNI WYGRALI!";
            uiManager.turn.text = "";
            uiManager.turnLabel.text = "";
            Debug.Log("Koniec, czarny wygral :(");
            this.enabled = false;
        }
        //a tutaj czy bialy
        else if(int.Parse(uiManager.blackPiecesDead.text) == 12 && pieceManager.blackPieces.Count !=0)
        {
            uiManager.wonText.text = "BIALI WYGRALI!";
            uiManager.turn.text = "";
            uiManager.turnLabel.text = "";
            Debug.Log("Koniec, biali gora!! :)");
            this.enabled = false;
        }
        else if(pieceManager.blackPieces.Count - int.Parse(uiManager.blackPiecesDead.text) + int.Parse(uiManager.blackPieces.text) <= 3
            && pieceManager.whitePieces.Count - int.Parse(uiManager.whitePiecesDead.text) + int.Parse(uiManager.whitePieces.text) <= 3)
        {
            uiManager.wonText.text = "REMIS!";
            uiManager.turn.text = "";
            uiManager.turnLabel.text = "";
            Debug.Log("remis");
            this.enabled = false;
        }

        
        //sprawdzamy czy bialy gracz ma inicjatywe
        if (pieceManager.p1i && int.Parse(uiManager.blackPieces.text) > 0)
        {
            //jesli tak to czarny musi polozyc
            pieceManager.SetInteractive(pieceManager.blackPieces, false);
        }
        //tutaj odwrotnie
        else if (pieceManager.p2i && int.Parse(uiManager.whitePieces.text) > 0)
        {
            pieceManager.SetInteractive(pieceManager.whitePieces, false);
        }

        //sprawdzamy czy lewy klik
        if (Input.GetMouseButtonDown(0))
        {

            //sprawdzamy czy gracz w poprzednim ruchu zbil pionka
            if (!pieceManager.canKillOther)
            {
                bool temp = false;
                if ((int.Parse(uiManager.blackPieces.text) > 0 && pieceManager.isBlackTurn))
                {
                    if (int.Parse(uiManager.whitePieces.text) == 0 && pieceManager.prevWhite == 1)
                    {
                        //jesli nie to tworzy w kliknietym miejscu pionka
                        temp = true;
                    }
                    if (temp)
                    {
                        pieceManager.p2i = true;
                        pieceManager.p1i = false;
                    }
                    CreatePiece();
                    if (temp)
                    {
                        pieceManager.SwitchPlayer(Color.white);
                        
                    }

                }
                else if (int.Parse(uiManager.whitePieces.text) > 0 && !pieceManager.isBlackTurn)
                {
                    if (int.Parse(uiManager.blackPieces.text) == 0 && pieceManager.prevBlack == 1)
                    {
                        //jesli nie to tworzy w kliknietym miejscu pionka
                        temp = true;
                    }
                    if (temp)
                    {
                        pieceManager.p1i = true;
                        pieceManager.p2i = false;
                    }
                    CreatePiece();
                    if (temp)
                    {
                        pieceManager.SwitchPlayer(Color.black);
                        
                    }
                }
                else { }

            }
            else
                //jesli tak to moze sobie wybrac pionek do zbicia
                KillOtherPiece();
            }
        
    }

    private void CreatePiece()
    {
        foreach (Cell cell in board.cells)
            if (RectTransformUtility.RectangleContainsScreenPoint(cell.rectTransform, Input.mousePosition))
                //jesli pole jest puste
                if (cell.currentPiece == null)
                {
                    //jesli tura czarnego
                    if (pieceManager.isBlackTurn)
                    {
                        //kladziemy klocek
                        pieceManager.PlacePiece(cell, pieceManager.blackPieces, Color.black, new Color32(0, 0, 0, 255), board);
                        //zmieniamy tekst w UI dot. tury
                        
                        //zmieniamy ture w kodzie
                        pieceManager.SwitchPlayer(Color.black);
                        //zmniejszamy licznik czarnych w UI
                        uiManager.SetBlackPieces(12 - pieceManager.blackPieces.Count);
                    }
                    else
                    {
                        pieceManager.PlacePiece(cell, pieceManager.whitePieces, Color.white, new Color32(255, 255, 255, 255), board);
                        
                        pieceManager.SwitchPlayer(Color.white);
                        uiManager.SetWhitePieces(12 - pieceManager.whitePieces.Count);
                    }
                    
                }
    }

    private void KillOtherPiece()
    {
        foreach (Cell cell in board.cells)
            if (RectTransformUtility.RectangleContainsScreenPoint(cell.rectTransform, Input.mousePosition)) {
                //jesli tura czarnego po zbiciu pionka to moze zabic dowolny inny przeciwnika
                if (pieceManager.isBlackTurn)
                {
                    //sprawdzamy czy kliknal bialy
                    if (cell.currentPiece.color == Color.white)
                    {
                        //zabijamy ten pionek
                        cell.currentPiece.Kill();
                        //wylaczamy flage zbijania
                        pieceManager.canKillOther = false;
                        //zmieniamy tekst w UI dot. tury
                        //zmieniamy ture w kodzie
                        pieceManager.SwitchPlayer(Color.black);
                    }
                }
                else
                {
                    if (cell.currentPiece.color == Color.black)
                    {
                        cell.currentPiece.Kill();
                        pieceManager.canKillOther = false;
                        pieceManager.SwitchPlayer(Color.white);
                    }
                }
            }
                
    }
}
