using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckersBoard : MonoBehaviour {

    public Piece[,] pieces = new Piece[8, 8];
    public GameObject whitePiecePrefab;
    public GameObject blackPiecePrefab; 

    public Vector3 boardOffset = new Vector3(-4.0f, 0, -4.0f);
    private Vector3 pieceOffset = new Vector3(0.5f, 0, 0.5f);

    private Piece selectedPiece;

    private Vector2 mouseOver;
    private Vector2 startDrag;
    private Vector2 endDrag;

    private void Start() {
        GenerateBoard();
    }

    private void Update() {
        UpdateMouseOver();
        // Debug.Log(mouseOver);            

        // If its my turn
        {
            int x = (int)mouseOver.x;           // This is because in a vector they were transferred back to floats
            int z = (int)mouseOver.y;

            if (Input.GetMouseButtonDown(0)) {
                SelectPiece(x, z);
            }

            if (Input.GetMouseButtonUp(0)) {

            }
        }
    }

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

    private void SelectPiece(int x, int z) {
        // Out of bounds
        if (x < 0 || x >= pieces.Length || z < 0 || z >= pieces.Length) {
            return;
        }

        Piece p = pieces[x, z];
        if (p != null) {
            selectedPiece = p;
            startDrag = mouseOver;
            Debug.Log(selectedPiece.name);
        }
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

    private void MovePiece(Piece p, int x, int z) {
        p.transform.position = (Vector3.right * x) + (Vector3.forward * z) + boardOffset + pieceOffset;
    }

}
