using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Piece : EventTrigger
{
    [HideInInspector]
    public Color color = Color.clear;

    protected Cell originalCell = null;
    protected Cell currentCell = null;

    protected RectTransform rectTransform = null;
    protected PieceManager pieceManager;

    protected Vector3Int movement = Vector3Int.one;
    protected List<Cell> highlightedCells = new List<Cell>();

    protected Cell targetCell = null;


    

    public virtual void Setup (Color newTeamColor, Color32 newSpriteColor, PieceManager newPieceManager)
    {
        pieceManager = newPieceManager;

        color = newTeamColor;
        GetComponent<Image>().color = newSpriteColor;
        rectTransform = GetComponent<RectTransform>();
    }

    public void Place(Cell cell)
    {
        currentCell = cell;
        originalCell = cell;
        currentCell.currentPiece = this;

        transform.position = cell.transform.position;
        gameObject.SetActive(true);
        
    }

    public void Reset()
    {
        
    }

    public virtual void Kill()
    {
        currentCell.currentPiece = null;
        //jesli umarl czarny klocek
        if (color == Color.black) {
            //zmieniamy tekst w UI
            UIManager temp = (UIManager)GameObject.FindObjectOfType(typeof(UIManager));
            temp.SetBlackPiecesDead(1);
        }
        else
        {
            UIManager temp = (UIManager)GameObject.FindObjectOfType(typeof(UIManager));
            temp.SetWhitePiecesDead(1);
        }
        gameObject.SetActive(false);
    }

    public void CreateCellPath(int xDir, int yDir, int mov)
    {
        int currentX = currentCell.boardPos.x;
        int currentY = currentCell.boardPos.y;

        for (int i = 1; i <= mov; i++)
        {
            //sprawdzamy czy nie wychodzi poza pole gry
            if (currentX + xDir >= 0 && currentX + xDir < 5 && currentY + yDir >= 0 && currentY + yDir < 5)
            {
                currentX += xDir;
                currentY += yDir;

                Board.CellState cellState = Board.CellState.None;
                cellState = currentCell.board.ValidateCell(currentX, currentY, this);
                //jesli obok pionka to przeciwnik
                if (cellState == Board.CellState.Enemy)
                {
                    //sprawdzamy czy pole dalej jest puste
                    if (currentX + xDir < 5 && currentY + yDir < 5 && currentCell.board.ValidateCell(currentX + xDir, currentY + yDir, this)==Board.CellState.Free) 
                        //jesli tak to podswietlamy ze mozna wykonac ruch zbijania
                        highlightedCells.Add(currentCell.board.cells[currentX+xDir, currentY+yDir]);
                    break;
                }

                if (cellState != Board.CellState.Free)
                    break;

                highlightedCells.Add(currentCell.board.cells[currentX, currentY]);
            }
        }
    }

    protected virtual void CheckPath()
    {
        CreateCellPath(1, 0, movement.x);
        CreateCellPath(-1, 0, movement.x);

        CreateCellPath(0, 1, movement.y);
        CreateCellPath(0, -1, movement.y);
    }

    protected void ShowCells()
    {
        foreach(Cell cell in highlightedCells)
        {
            cell.outlineImage.enabled = true;
        }
    }

    protected void ClearCells()
    {
        foreach (Cell cell in highlightedCells)
        {
            cell.outlineImage.enabled = false;
        }
        highlightedCells.Clear();
    }

    protected virtual void Move()
    {
        int currentX = currentCell.boardPos.x;
        int currentY = currentCell.boardPos.y;
        int targetX = targetCell.boardPos.x;
        int targetY = targetCell.boardPos.y;
        //sprawdzanie czy wykonany ruch byl do zbicia
        if (currentX + 2 == targetX)
        {
            //jesli tak to usuwamy ten co zbilismy
            currentCell.board.cells[currentX + 1, currentY].RemovePiece();
            //ustawiamy flage zbijania dodaktowego
            pieceManager.canKillOther = true;
        }
        else if (currentX - 2 == targetX)
        {
            currentCell.board.cells[currentX - 1, currentY].RemovePiece();
            pieceManager.canKillOther = true;
        }
        else if (currentY + 2 == targetY)
        {
            currentCell.board.cells[currentX, currentY + 1].RemovePiece();
            pieceManager.canKillOther = true;
        }
        else if (currentY - 2 == targetY)
        {
            currentCell.board.cells[currentX, currentY - 1].RemovePiece();
            pieceManager.canKillOther = true;
        }

        currentCell.currentPiece = null;

        currentCell = targetCell;

        currentCell.currentPiece = this;

        transform.position = currentCell.transform.position;
        targetCell = null;
        //po wykonaniu "ruchu" zmieniamy inicjatywe na 2 gracza
        if (color == Color.black)
        {
            pieceManager.p2i = false;
            pieceManager.p1i = true;
        }
        else
        {
            pieceManager.p1i = false;
            pieceManager.p2i = true;
        }
        UIManager temp = (UIManager)GameObject.FindObjectOfType(typeof(UIManager));

        //sprawdzamy czy moze zbic dodatkowy pionek
        if (!pieceManager.canKillOther)
        {
            //jesli nie to zmieniamy normalnie runde
            if (color == Color.black)
                temp.SetTurn(false);
            else temp.SetTurn(true);
            pieceManager.SwitchPlayer(color);
        }   
        else if (pieceManager.canKillOther)
        {
            //jesli tak to dezaktywujemy poruszanie klockami aby na pewno zbil
            if (color == Color.black)
            {
                pieceManager.SetInteractive(pieceManager.blackPieces, false);
            }
            else
            {
                pieceManager.SetInteractive(pieceManager.whitePieces, false);
            }
        }
            
    }

    public override void OnBeginDrag(PointerEventData eventData)
    {
        base.OnBeginDrag(eventData);

        CheckPath();
        ShowCells();
    }

    public override void OnDrag(PointerEventData eventData)
    {
        base.OnDrag(eventData);

        transform.position += (Vector3)eventData.delta;

        foreach(Cell cell in highlightedCells)
        {
            if(RectTransformUtility.RectangleContainsScreenPoint(cell.rectTransform, Input.mousePosition))
            {
                targetCell = cell;
                break;
            }

            targetCell = null;
        }
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        base.OnEndDrag(eventData);

        ClearCells();

        if (!targetCell)
        {
            transform.position = currentCell.gameObject.transform.position;
            return;
        }

        Move();
    }
}
