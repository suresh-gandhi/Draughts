using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckersBoard : MonoBehaviour {

    public Piece[,] pieces = new Piece[8, 8];
    public GameObject whitePiecePrefab;
    public GameObject blackPiecePrefab; 

    public Vector3 boardOffset = new Vector3(-4.0f, 0, -4.0f);
    private Vector3 pieceOffset = new Vector3(0.5f, 0, 0.5f);

    public bool isWhite;
    private bool isWhiteTurn;
    private bool hasKilled;

    private Piece selectedPiece;
    private List<Piece> forcedPieces;

    private Vector2 mouseOver;
    private Vector2 startDrag;
    private Vector2 endDrag;

    private void Start() {
        GenerateBoard();
        forcedPieces = new List<Piece>();
        isWhiteTurn = true;
    }

    private void Update() {
        Debug.Log(forcedPieces);
        UpdateMouseOver();                      // Refreshing the mouse over in every frame
        // Debug.Log(mouseOver);            

        // If its my turn
        {
            int x = (int)mouseOver.x;           // This is because in a vector they were transferred back to floats
            int z = (int)mouseOver.y;

            if (selectedPiece != null) {
                UpdatePieceDrag(selectedPiece);     // Refreshing the update drag when we have some piece selected
            }

            if (Input.GetMouseButtonDown(0)) {
                SelectPiece(x, z);
            }

            if (Input.GetMouseButtonUp(0)) {
                TryMove((int)startDrag.x, (int)startDrag.y, x, z);
            }
        }
    }

    // This function basically assigns the mouseOver correctly to the pointed block. mouseOver is a Vector2
    private void UpdateMouseOver() {
        // If its my turn
        if (!Camera.main) {
            Debug.Log("Unable to find main camera");
            return;
        }

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 25.0f, LayerMask.GetMask("Board")))
        {
            mouseOver.x = (int)(hit.point.x - boardOffset.x);
            mouseOver.y = (int)(hit.point.z - boardOffset.z);
        }
        else {
            mouseOver.x = -1;
            mouseOver.y = -1;
        }

    }

    // This function takes a piece and elevates it as we hover a mouse. However it assumes that the piece p is not null which implies a piece should be selected.
    private void UpdatePieceDrag(Piece p) {
        if (!Camera.main)
        {
            Debug.Log("Unable to find main camera");
            return;
        }

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 25.0f, LayerMask.GetMask("Board")))
        {
            p.transform.position = hit.point + Vector3.up;
        }
    }

    // This function selects the piece at the block (x, z) if its in the valid range else returns and does nothing. It also updates the start drag.
    private void SelectPiece(int x, int z) {
        // Out of bounds
        if (x < 0 || x >= 8 || z < 0 || z >= 8) {
            return;
        }

        Piece p = pieces[x, z];
        if (p != null && p.isWhite == isWhite) {

            if (forcedPieces.Count == 0)
            {
                selectedPiece = p;
                startDrag = mouseOver;
            }
            else {
                if (forcedPieces.Find(fp => fp == p) == null)
                {
                    return;
                }
                selectedPiece = p;
                startDrag = mouseOver;
            }

            /*
            selectedPiece = p;
            startDrag = mouseOver;
            Debug.Log(selectedPiece.name);
            */
        }
    }

    private void TryMove(int x1, int y1, int x2, int y2) {

        forcedPieces = ScanForPossibleMoves();

        // Multiplayer Support
        startDrag = new Vector2(x1, y1);
        endDrag = new Vector2(x2, y2);
        selectedPiece = pieces[x1, y1];

        // MovePiece(selectedPiece, x2, y2);

        // Out of bounds
        if (x2 < 0 || x2 >= 8 || y2 < 0 || y2 >= 8) {
            if(selectedPiece != null) {
                MovePiece(selectedPiece, x1, y1);
            }
            startDrag = Vector2.zero;
            selectedPiece = null;
            return;
        }

        if (selectedPiece != null) {
            // If it has not moved
            if (endDrag == startDrag) {
                MovePiece(selectedPiece, x1, y1);
                startDrag = Vector2.zero;
                selectedPiece = null;
                return;
            }

            // Check if it is a valid move  
            if (selectedPiece.ValidMove(pieces, x1, y1, x2, y2))
            {
                // Did we kill something
                // If this is a jump
                if (Mathf.Abs(x1 - x2) == 2)
                {
                    Piece p = pieces[(x1 + x2) / 2, (y1 + y2) / 2];
                    if (p != null)
                    {
                        pieces[(x1 + x2) / 2, (y1 + y2) / 2] = null;
                        Destroy(p.gameObject);
                        hasKilled = true;
                    }
                }

                // Were we supposed to kill anything
                if(forcedPieces.Count != 0 && !hasKilled) {
                    MovePiece(selectedPiece, x1, y1);
                    startDrag = Vector2.zero;
                    selectedPiece = null;
                    return;
                }

                pieces[x2, y2] = selectedPiece;
                pieces[x1, y1] = null;
                MovePiece(selectedPiece, x2, y2);

                EndTurn();
            }
            else {
                MovePiece(selectedPiece, x1, y1);
                startDrag = Vector2.zero;
                selectedPiece = null;
                return;
            }
        }
        
    }

    private void EndTurn() {
        selectedPiece = null;
        startDrag = Vector2.zero;
        isWhiteTurn = !isWhiteTurn;
        hasKilled = false;
        CheckVictory();
    }

    private bool CheckVictory() {
        return false;
    }

    private List<Piece> ScanForPossibleMoves() {
        forcedPieces = new List<Piece>();

        // Check all the pieces
        for (int i = 0; i < 8; i++) {
            for (int j = 0; j < 8; j++) {
                if (pieces[i, j] != null && pieces[i, j].isWhite == isWhiteTurn) {
                    if (pieces[i, j].IsForceToMove(pieces, i, j)) {
                        forcedPieces.Add(pieces[i, j]);
                    }
                }
            }
        }

        return forcedPieces;
    }

    private void GenerateBoard() {
        // Generate White Team
        for (int z = 0; z < 3; z++) {
            bool oddRow = (z % 2 == 0);
            for (int x = 0; x < 8; x+=2) {
                // Generate our Piece
                GeneratePiece((oddRow)?x : x+1 , z);
            }
        }

        // Generate Black Team
        for (int z = 7; z > 4; z--)
        {
            bool oddRow = (z % 2 == 0);
            for (int x = 0; x < 8; x += 2)
            {
                // Generate our Piece
                GeneratePiece((oddRow) ? x : x + 1, z);
            }
        }

    }

    private void GeneratePiece(int x, int z) {
        bool isWhitePiece = (z > 3) ? false : true;
        GameObject go = Instantiate((isWhitePiece) ? whitePiecePrefab: blackPiecePrefab) as GameObject;
        go.transform.SetParent(transform);
        Piece p = go.GetComponent<Piece>();
        pieces[x, z] = p;
        MovePiece(p, x, z);
    }

    // This function takes a piece p and moves it to the block (x, z)
    private void MovePiece(Piece p, int x, int z) {
        p.transform.position = (Vector3.right * x) + (Vector3.forward * z) + boardOffset + pieceOffset;
    }

}
