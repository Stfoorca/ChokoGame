using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cell : MonoBehaviour
{

    public Image outlineImage;

    [HideInInspector]
    public Vector2Int boardPos = Vector2Int.zero;
    [HideInInspector]
    public Board board = null;
    [HideInInspector]
    public RectTransform rectTransform = null;

    [HideInInspector]
    public Piece currentPiece = null;

    public void Setup(Vector2Int newBoardPos, Board newBoard)
    {
        boardPos = newBoardPos;
        board = newBoard;

        rectTransform = GetComponent<RectTransform>();
    }

    public void RemovePiece()
    {
        if (currentPiece != null)
        {
            currentPiece.Kill();
        }
    }

}
