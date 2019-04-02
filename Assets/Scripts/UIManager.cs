using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Text turn;
    public Text turnLabel;

    public Text whitePieces;
    public Text whitePiecesDead;

    public Text blackPieces;
    public Text blackPiecesDead;

    public Text wonText;
    


    // Start is called before the first frame update
    void Start()
    {
        whitePieces.text = "12";
        whitePiecesDead.text = "0";

        blackPieces.text = "12";
        blackPiecesDead.text = "0";

        turn.text = "white";
        turnLabel.text = "Ruch gracza:";

        wonText.text = "";
    }

    public void SetWhitePieces(int i)
    {
        whitePieces.text = i.ToString();
    }

    public void SetWhitePiecesDead(int i)
    {
        int a = int.Parse(whitePiecesDead.text) + i;
        Debug.Log("Before: " + whitePiecesDead.text + " After: " + a.ToString());

        whitePiecesDead.text = a.ToString();
    }

    public void SetBlackPieces(int i)
    {
        blackPieces.text = i.ToString();
    }

    public void SetBlackPiecesDead(int i)
    {
        int a = int.Parse(blackPiecesDead.text) + i;
        Debug.Log("Before: " + blackPiecesDead.text + " After: " + a.ToString());
        blackPiecesDead.text = a.ToString();
    }

    public void SetTurn(bool playerTurn)
    {
        if (!playerTurn)
            turn.text = "black";
        else turn.text = "white";
    }
}
