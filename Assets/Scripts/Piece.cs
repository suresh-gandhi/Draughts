using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour {

    public bool isWhite;
    public bool isKing;

    public bool IsForceToMove(Piece[,] board, int x, int y)
    {
        // Debug.Log("Inside is forced to move");
        // Debug.Log("isWhite: " + isWhite);
        // Debug.Log("isKing: " + isKing);
        if (isWhite || isKing)
        {
            // Debug.Log("Inside isWhite || isKing");
            // Top Left
            if (x >= 2 && y <= 5)
            {
                // Debug.Log("Top Left");
                Piece p = board[x - 1, y + 1];
                // If there is a piece, and it is not the same color as ours 
                if (p != null && p.isWhite != isWhite)
                {
                    // Check if it is possible to land after the jump
                    if (board[x - 2, y + 2] == null)
                    {
                        return true;
                    }
                }
            }

            // Top Right
            if (x <= 5 && y <= 5)
            {
                // Debug.Log("Top Right");
                Piece p = board[x + 1, y + 1];
                // If there is a piece, and it is not the same color as ours 
                if (p != null && p.isWhite != isWhite)
                {
                    // Check if it is possible to land after the jump
                    if (board[x + 2, y + 2] == null)
                    {
                        return true;
                    }
                }
            }
        }

        if(!isWhite || isKing)
        {                              // MODIFICATION OR DEBUGGING BY ME. He wrote it else if earlier. That was a bug according to me.
            // Debug.Log("Inside else if");
            // Bottom Left
            if (x >= 2 && y >= 2)
            {
                // Debug.Log("Bottom Left");
                Piece p = board[x - 1, y - 1];
                // If there is a piece, and it is not the same color as ours 
                if (p != null && p.isWhite != isWhite)
                {
                    // Check if it is possible to land after the jump
                    if (board[x - 2, y - 2] == null)
                    {
                        return true;
                    }
                }
            }

            // Bottom Right
            if (x <= 5 && y >= 2)
            {
                // Debug.Log("Bottom Right");
                Piece p = board[x + 1, y - 1];
                // If there is a piece, and it is not the same color as ours 
                if (p != null && p.isWhite != isWhite)
                {
                    // Check if it is possible to land after the jump
                    if (board[x + 2, y - 2] == null)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    public bool ValidMove(Piece[,] board, int x1, int y1, int x2, int y2) {
        // If you are moving on top of another piece
        if (board[x2, y2] != null) {
            return false;
        }

        int deltaMoveX = Mathf.Abs(x1 - x2);
        int deltaMoveY = y2 - y1;

        if (isWhite || isKing) {                    // We have used || here, therefore when we are a king we will go inside both the ifs in other words we can move upwards as well as downwards.
            if (deltaMoveX == 1) {
                if(deltaMoveY == 1) {
                    return true;
                }
            }
            else if (deltaMoveX == 2) {
                if (deltaMoveY == 2) {
                    Piece p = board[(x1 + x2) / 2, (y1 + y2) / 2];
                    if (p != null && p.isWhite != isWhite) {
                        return true;
                    }
                }
            }
        }

        if (!isWhite || isKing)
        {
            if (deltaMoveX == 1)
            {
                if (deltaMoveY == -1)
                {
                    return true;
                }
            }
            else if (deltaMoveX == 2)
            {
                if (deltaMoveY == -2)
                {
                    Piece p = board[(x1 + x2) / 2, (y1 + y2) / 2];
                    if (p != null && p.isWhite != isWhite)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }
}
