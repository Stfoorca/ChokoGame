using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIAIAIAIAI : MonoBehaviour
{
    public static AIAIAIAIAI instance;

    void Start()
    {

        instance = this;
    }

    int AI_PLAYER = 2;
    int REAL_PLAYER = 1;
       

    int is_terminal(Board board)
    {
        int whiteAlive = 0, blackAlive = 0;
        for (int x = 0; x < 5; x++)
        {
            for (int y = 0; y < 5; y++)
            {
                if (board.pieces[x, y].team == 1)
                {
                    whiteAlive++;
                }
                else if (board.pieces[x, y].team == 2)
                {
                    blackAlive++;
                }
            }
        }

        if (whiteAlive + board.whitePieces <= 0 && blackAlive + board.blackPieces > 3)
            return 2;
        else if (whiteAlive + board.maxPieces - board.whitePieces > 3 &&
                 blackAlive + board.maxPieces - board.blackPieces <= 0)
            return 1;
        else if (whiteAlive + board.maxPieces - board.whitePieces <= 3 &&
                 blackAlive + board.maxPieces - board.blackPieces <= 3)
            return 0;
        return -1;
    }

    // excuse me what the fuck  
    ///TODO: zmniejszyc bydlaka
    ///
    ///
    bool isValidMove(int x, int y, Piece[,] tmp)
    {
        if (x < 0 || x > 4 || y < 0 || y > 4) return false;
        if (tmp[x, y].team == 0) return true;
        return false;
    }

    bool isValidKill(int x, int y, Piece[,] tmp, int enemy, int newX, int newY)
    {
        if (x < 0 || x > 4 || y < 0 || y > 4) return false;
        if (newX < 0 || newX > 4 || newY < 0 || newY > 4) return false;
        if (tmp[x, y].team == enemy && tmp[newX, newY].team == 0)
        {
            return true;

        }


        return false;
    }
    List<MoveController> get_valid_locations(Piece[,] board, int player, bool hasToPlace, List<Piece> enemyLocations)
    {
        List<MoveController> toRet = new List<MoveController>();
        bool areThereAnyPiecesLeft;
        int Friendly_Team = player;
        int Enemy_Team;
        if (player == 1) Enemy_Team = 2;
        else Enemy_Team = 1;

        if(player == 1)  areThereAnyPiecesLeft = (Board.instance.maxPieces - Board.instance.whitePieces) > 0;
        else areThereAnyPiecesLeft = (Board.instance.maxPieces - Board.instance.blackPieces) > 0;


        for (int i = 0; i < 5; ++i)
        {
            for (int j = 0; j < 5; ++j)
            {
                // Możliwe postawienia
                if (board[i, j].team == 0 && areThereAnyPiecesLeft)
                {
                    toRet.Add(new MoveController(MoveController.Move.Place, Friendly_Team, new Vector2Int(i, j)));
                }

                else if ((!hasToPlace && board[i, j].team == Friendly_Team) || (board[i, j].team == Friendly_Team && !areThereAnyPiecesLeft))
                {
                    // 0 -> L; 1 -> U 2 -> R 3 -> D
                    // Możliwe przesunięcia
                    if (isValidMove(i - 1, j, board))
                    {
                        toRet.Add(new MoveController(MoveController.Move.Move, Friendly_Team, new Vector2Int(i, j),
                            new Vector2Int(i - 1, j)));
                    }

                    if (isValidMove(i, j - 1, board))
                    {
                        toRet.Add(new MoveController(MoveController.Move.Move, Friendly_Team, new Vector2Int(i, j),
                            new Vector2Int(i, j - 1)));
                    }

                    if (isValidMove(i + 1, j, board))
                    {
                        toRet.Add(new MoveController(MoveController.Move.Move, Friendly_Team, new Vector2Int(i, j),
                            new Vector2Int(i + 1, j)));
                    }

                    if (isValidMove(i, j + 1, board))
                    {
                        toRet.Add(new MoveController(MoveController.Move.Move, Friendly_Team, new Vector2Int(i, j),
                            new Vector2Int(i, j + 1)));
                    }

                    // Możliwe kill
                    if (isValidKill(i - 1, j, board, Enemy_Team, i-2, j))
                    {
                        int d = 0, p = 0;
                        if (enemyLocations.Count > 1)
                        {
                            if (enemyLocations[0].x != i - 1 || enemyLocations[0].y != j)
                            {
                                d = enemyLocations[0].x;
                                p = enemyLocations[0].y;
                            }
                            else 
                            {
                                d = enemyLocations[1].x;
                                p = enemyLocations[1].y;
                            }
                        }

                        toRet.Add(new MoveController(MoveController.Move.Kill, Friendly_Team,
                            new Vector2Int(i, j), new Vector2Int(i - 2, j), new Vector2Int(d, p),
                            new Vector2Int(i - 1, j)));
                    }

                    if (isValidKill(i, j - 1, board, Enemy_Team, i,j-2))
                    {
                        int d = 0, p = 0;
                        if (enemyLocations.Count > 1)
                        {
                            if (enemyLocations[0].x != i || enemyLocations[0].y != j - 1)
                            {
                                d = enemyLocations[0].x;
                                p = enemyLocations[0].y;
                            }
                            else 
                            {
                                d = enemyLocations[1].x;
                                p = enemyLocations[1].y;
                            }
                        }

                        toRet.Add(new MoveController(MoveController.Move.Kill, Friendly_Team,
                            new Vector2Int(i, j), new Vector2Int(i, j - 2), new Vector2Int(d, p),
                            new Vector2Int(i, j - 1)));
                    }

                    if (isValidKill(i + 1, j, board, Enemy_Team, i+2, j))
                    {
                        int d = 0, p = 0;
                        if (enemyLocations.Count > 1)
                        {
                            if (enemyLocations[0].x != i+1 || enemyLocations[0].y != j)
                            {
                                d = enemyLocations[0].x;
                                p = enemyLocations[0].y;
                            }
                            else
                            {
                                d = enemyLocations[1].x;
                                p = enemyLocations[1].y;
                            }
                        }

                        toRet.Add(new MoveController(MoveController.Move.Kill, Friendly_Team,
                            new Vector2Int(i, j), new Vector2Int(i + 2, j), new Vector2Int(d, p),
                            new Vector2Int(i + 1, j)));
                    }

                    if (isValidKill(i, j + 1, board, Enemy_Team, i, j+2))
                    {
                        int d = 0, p = 0;
                        if (enemyLocations.Count > 1)
                        {
                            if (enemyLocations[0].x != i || enemyLocations[0].y != j + 1)
                            {
                                d = enemyLocations[0].x;
                                p = enemyLocations[0].y;
                            }
                            else 
                            {
                                d = enemyLocations[1].x;
                                p = enemyLocations[1].y;
                            }
                        }

                        toRet.Add(new MoveController(MoveController.Move.Kill, Friendly_Team,
                            new Vector2Int(i, j), new Vector2Int(i, j + 2), new Vector2Int(d, p),
                            new Vector2Int(i, j + 1)));
                    }
                }
            }
        }


        //move = new MoveController(MoveController.Move.Kill, turn, curTemp, destTemp, killTemp, betweenTemp);
        return toRet;
    }

    //int isValid(int x, int y, Piece[,] tmp, int enemy, int dir)
    //{
    //    if (x < 0 || x > 4 || y < 0 || y > 4) return 0;
    //    if (tmp[x, y].team == 0) return 1;
    //    if (tmp[x, y].team == enemy)
    //    {
    //        if (dir == 0 && x > 0 && tmp[x - 1, y].team == 0) return 2;
    //        if (dir == 1 && y > 0 && tmp[x, y - 1].team == 0) return 2;
    //        if (dir == 2 && x < 4 && tmp[x + 1, y].team == 0) return 2;
    //        if (dir == 3 && y < 4 && tmp[x, y + 1].team == 0) return 2;
    //    }

    //    return 0;
    //}

    

   

    int score_position(Board board)
    {
        int score = 0;
        score += (board.maxPieces - board.blackPieces) * 20;
        score -= (board.maxPieces - board.whitePieces) * 20;
        score += board.p1PiecesDead * 500;
        score -= board.p2PiecesDead * 150;
        for (int i = 0; i < 5; ++i)
        {
            for (int j = 0; j < 5; ++j)
            {
                if (board.pieces[i, j].team == REAL_PLAYER)
                {
                    if (i % 4 == 0 || j % 4 == 0) score -= 10;
                }

                if (board.pieces[i, j].team == AI_PLAYER)
                {
                    if (i % 4 == 0 || j % 4 == 0) score += 10;
                }

                if (isValidKill(i - 1, j, board.pieces, REAL_PLAYER, i - 2, j))
                {
                    score += 150;
                }

                else if (isValidKill(i, j - 1, board.pieces, REAL_PLAYER, i, j-2))
                {
                    score += 150;
                }

                else if(isValidKill(i + 1, j, board.pieces, REAL_PLAYER, i+2, j))
                {
                    score += 150;
                }

                else if(isValidKill(i, j + 1, board.pieces, REAL_PLAYER, i, j+2))
                {
                    score += 150;
                }

                 if (isValidKill(i - 1, j, board.pieces, AI_PLAYER, i-2, j))
                {
                    score -= 100;
                }

                else if (isValidKill(i, j - 1, board.pieces, AI_PLAYER, i, j-2))
                {
                    score -= 100;
                }

                else if (isValidKill(i + 1, j, board.pieces, AI_PLAYER, i+2, j))
                {
                    score -= 100;
                }

                else if (isValidKill(i, j + 1, board.pieces, AI_PLAYER, i, j+2))
                {
                    score -= 100;
                }
            }
        }


        return score;
    }

    public Tuple<MoveController, int> minimax(Board board, int depth, int alpha, int beta, int Player, bool hasToPlace)
    {
        // maximising true == AI
        List<MoveController> valid = new List<MoveController>();
        if (Player == REAL_PLAYER) { valid = get_valid_locations(board.pieces, Player, hasToPlace, board.blackPiecesLocations); }
        if (Player == AI_PLAYER) { valid = get_valid_locations(board.pieces, Player, hasToPlace, board.whitePiecesLocations); }

        if (depth == 0 || is_terminal(board) > -1)
        {
            int gameState = is_terminal(board);
            if (gameState > -1)
            {
                if (gameState == Player) return new Tuple<MoveController, int>(null, 21372137);
                else if (gameState == 0) return new Tuple<MoveController, int>(null, 0);
                else return new Tuple<MoveController, int>(null, -21372137);
            }

            else return new Tuple<MoveController, int>(null, score_position(board)); // check this shit
        }
         if (Player == AI_PLAYER)
        {
            int value = -2137214000;
            MoveController bestMove = valid[0];
            foreach (MoveController mov in valid)
            {
                Board b_copy = Board.OwnDeepCopy(board);
                int new_score = -213721400;
                b_copy.MakeMove(mov);
                if (hasToPlace)
                {
                    new_score = minimax(b_copy, depth - 1, alpha, beta, REAL_PLAYER, false).Item2;
                }
                else if (mov.MoveType == MoveController.Move.Place)
                {
                    new_score = minimax(b_copy, depth - 1, alpha, beta, REAL_PLAYER, true).Item2;
                }
                else
                {
                    new_score = minimax(b_copy, depth - 1, alpha, beta, REAL_PLAYER, false).Item2;
                }

                if (new_score > value)
                {
                    value = new_score;
                    bestMove = mov;
                }

                alpha = alpha > value ? alpha : value;
                if (alpha >= beta) break;
            }

            return new Tuple<MoveController, int>(bestMove, value);
        }
        else
        {
            int value = 2137214000;
            MoveController bestMove = valid[0];
            foreach (MoveController mov in valid)
            {
                Board b_copy = Board.OwnDeepCopy(board);
                int new_score = 213721400;
                b_copy.MakeMove(mov);
                if (hasToPlace)
                {
                    new_score = minimax(b_copy, depth - 1, alpha, beta, REAL_PLAYER, false).Item2;
                }
                else if (mov.MoveType == MoveController.Move.Place)
                {
                    new_score = minimax(b_copy, depth - 1, alpha, beta, REAL_PLAYER, true).Item2;
                }
                else
                {
                    new_score = minimax(b_copy, depth - 1, alpha, beta, REAL_PLAYER, false).Item2;
                }

                if (new_score < value)
                {
                    value = new_score;
                    bestMove = mov;
                }

                beta = beta < value ? beta : value;
                if (alpha >= beta) break;
            }

            return new Tuple<MoveController, int>(bestMove, value);
        }
    }
}