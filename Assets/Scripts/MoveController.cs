using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class MoveController : MonoBehaviour
{
    public enum Move
    {
        Place,
        Move,
        Kill
    };

    public Move MoveType;

    public int team;

    public Vector2Int target;

    public Vector2Int direction;

    public Vector2Int pieceToKill;

    public Vector2Int betweenKill;

    public MoveController()
    {

    }

    public MoveController(Move move, int team, Vector2Int target)
    {
        this.MoveType = move;
        this.team = team;
        this.target = target;
    }
    public MoveController(Move move, int team, Vector2Int target, Vector2Int direction)
    {
        this.MoveType = move;
        this.team = team;
        this.target = target;
        this.direction = direction;
    }
    public MoveController(Move move, int team, Vector2Int target, Vector2Int direction, Vector2Int pieceToKill, Vector2Int betweenKill)
    {
        this.MoveType = move;
        this.team = team;
        this.target = target;
        this.direction = direction;
        this.pieceToKill = pieceToKill;
        this.betweenKill = betweenKill;
    }
}
