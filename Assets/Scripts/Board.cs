using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Board : MonoBehaviour
{
    public static int boardSize = 5;

    public GameObject cellPrefab;

    [HideInInspector]
    public Cell[,] cells = new Cell[boardSize, boardSize];

    public enum CellState
    {
        None,
        Friendly,
        Enemy,
        Free,
        OutOfBounds
    }

    public void CreateBoard()
    {
        for (int y = 0; y < boardSize; y++)
        {
            for (int x = 0; x < boardSize; x++)
            {
                GameObject cell = Instantiate(cellPrefab, transform);

                RectTransform rectTransform = cell.GetComponent<RectTransform>();
                rectTransform.anchoredPosition = new Vector2((x * 100) + 50, (y * 100) + 50);

                cells[x, y] = cell.GetComponent<Cell>();
                cells[x, y].Setup(new Vector2Int(x, y), this);
            }
        }

        //kolorowanie pola gry
        for (int x = 0; x < boardSize; x++)
        {
            for (int y = 0; y < boardSize; y++)
            {
                if (y % 2 == 0 && x % 2 == 0 || (x + y) % 2 == 0) 
                    cells[x, y].GetComponent<Image>().color = new Color32(230, 220, 187, 255);
            }
        }
    }

    //walidacja do tworzenia mozliwych sciezek ruchu pionka
    public CellState ValidateCell(int targetX, int targetY, Piece checkingPiece)
    {
        if (targetX < 0 || targetX > boardSize)
            return CellState.OutOfBounds;

        if (targetY < 0 || targetY > boardSize)
            return CellState.OutOfBounds;

        Cell targetCell = cells[targetX, targetY];

        if(targetCell.currentPiece != null)
        {
            if (checkingPiece.color == targetCell.currentPiece.color)
                return CellState.Friendly;
            else return CellState.Enemy;
        }

        return CellState.Free;
    }
}
