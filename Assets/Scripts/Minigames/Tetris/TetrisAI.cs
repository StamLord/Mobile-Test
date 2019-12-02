using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TetrisAI : MonoBehaviour
{
    public float heightWeight = 1f; 
    public float rowsWeight = 1f; 
    public float holesWeight = 1f; 
    public float bumpinessWeight = 1f; 

    [SerializeField] TetrisGroup activePiece;
    [SerializeField] Vector3 pieceOrigin;
    [SerializeField] Transform [,] originalGrid;
    [SerializeField] TetrisGrid activeGrid;

    public void NewPiece(TetrisGroup piece, TetrisGrid grid)
    {
        activePiece = piece;
        pieceOrigin = activePiece.transform.position;

        originalGrid = CopyGrid(grid.grid, TetrisGrid.w, TetrisGrid.h);

        activeGrid = grid;

        FindBest();
    }

    private void FindBest()
    {   
        float bestScore = -99999;
        int bestRotation = 0;
        int bestColumn = 0;

        for(int rotation = 0; rotation < 4; rotation++)
        {
            for(int column = 0; column < TetrisGrid.w; column++)
            {
                int x = 5;
                if(column < 5)
                    while(activePiece.MoveLeft() && x > column)
                        x--;
                else if(column > 5)
                    while(activePiece.MoveRight() && x < column)
                        x++;
                
                int rowsDeleted = activePiece.MoveDown(false);
                while(rowsDeleted == -1)
                    rowsDeleted = activePiece.MoveDown(false);

                // Calculate Score
                float lineDel = rowsDeleted * rowsWeight;
                float height = activeGrid.AggregateHeight() * heightWeight;
                float bumps = activeGrid.Bumpiness() * bumpinessWeight; 
                float holes = activeGrid.Holes() * holesWeight;

                float score = lineDel -height -bumps - holes;

                if(score > bestScore)
                {
                    bestScore = score;
                    bestRotation = rotation;
                    bestColumn = column;
                }
                
                Debug.Log("Check rot: " + rotation + " col: "+ column + " height: " + height + " bumps: "+ bumps + " lines deleted: " + lineDel + " holes: " + holes + " Score: "+ score);

                // Return grid to privouse state
                activeGrid.grid = CopyGrid(originalGrid, TetrisGrid.w, TetrisGrid.h);
                // Return piece to origin
                activePiece.transform.position = pieceOrigin;
            }
            activePiece.RotateUp();
        }
        
        // Rotate to origin
        activePiece.transform.rotation = Quaternion.Euler(Vector3.zero);
        ExecuteBest(bestColumn, bestRotation);
    }

    private void ExecuteBest(int column, int rotation)
    {
        Debug.Log("Moving to COL: " + column + " ROT: " + rotation);

        StartCoroutine(Rotate(rotation));

        StartCoroutine(Move(column));

    }

    IEnumerator Rotate(int rotation)
    {
        for(int rot = 0; rot < rotation; rot++)
        {
            activePiece.RotateUp();
            yield return new WaitForSeconds(0.1f);
        }
    }

    IEnumerator Move(int column)
    {
        int x = 5;
        if(column < 5)
            while(activePiece.MoveLeft() && x > column)
            {
                x--;
                yield return new WaitForSeconds(0.1f);
            }
        else if(column > 5)
            while(activePiece.MoveRight() && x < column)
            {
                x++;
                yield return new WaitForSeconds(0.1f);
            }

        StartCoroutine(MoveDown());
    }

    IEnumerator MoveDown()
    {
        while(activePiece.MoveDown(true) == -1)
        {
            yield return new WaitForSeconds(0.1f);
        }
    }

    Transform[,] CopyGrid (Transform[,] grid, int w, int h)
    {
        Transform[,] clone = new Transform[w, h];
        for(int x = 0; x < w; x++)
        {
            for(int y = 0; y < h; y++)
            {
                clone[x,y] = grid[x,y];
            }   
        }

        return clone;
    }
}
