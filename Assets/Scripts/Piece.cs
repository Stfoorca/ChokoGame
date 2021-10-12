using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    public Sprite image;
    public int team = 0;
    public int x = -1, y = -1;
    public Piece(int team, int x, int y)
    {
        this.team = team;
        this.x = x;
        this.y = y;
    }

}
