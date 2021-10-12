using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    public static UIManager instance;

    public Text whiteHand, blackHand, turnText, turnLabel, gameStatusText, hasToPlaceText;
    public GameObject textPanel;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        gameStatusText.text = "";
        turnText.text = "PATYKI";
        whiteHand.text = "12";
        blackHand.text = "12";
        hasToPlaceText.text = "Informacja:\nmusisz kłaść";
        
    }


    public void Print(string msg)
    {
        //debugText.text = msg;
    }

    public void SetHand(int team, int msg)
    {
        if (team == 1)
            whiteHand.text = msg.ToString();
        else blackHand.text = msg.ToString();
    }

    public void SetTurn(int team)
    {
        if (team == 1)
        {
            turnText.text = "PATYKI";
        }
        else turnText.text = "KAMIENIE";
    }

    public void SetWinner(int team)
    {

        Debug.Log("Koniec gry: <<" + team + ">>");

        if (team == 0)
        {
            turnLabel.text = "";
            turnText.text = "REMIS";
        }
        else if (team == 1)
        {
            turnLabel.text = "TRIUMF";
            turnText.text = "PATYKÓW";
        }
        else if (team == 2)
        {
            turnLabel.text = "TRIUMF";
            turnText.text = "KAMIENI";
        }
    }

    public void SetGameStatusText(string text)
    {
        gameStatusText.text += text + "\n";
    }

    public void SetHasToPlaceText(bool v)
    {
        if (v)
            hasToPlaceText.text = "Informacja:\nmusisz kłaść";
        else hasToPlaceText.text = "Informacja:\nmożesz przesuwać";
    }

}
