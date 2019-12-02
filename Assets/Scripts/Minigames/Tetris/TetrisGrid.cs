using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetrisGrid : MonoBehaviour
{
    public int horizontalOffset = 0;
    public static int w = 10;
    public static int h = 22;
    public Transform[,] grid = new Transform[w, h];

    public Vector2 RoundVec2(Vector2 vector)
    {
        return new Vector2(Mathf.Round(vector.x - horizontalOffset) , Mathf.Round(vector.y));
    }

    public bool InsideBorder(Vector2 position)
    {
        return ((int)position.x >= 0 &&
                (int)position.x < w &&
                (int)position.y >= 0);
    }

    public void DeleteRow(int y)
    {
        for(int x=0; x< w; ++x)
        {
            Destroy(grid[x,y].gameObject);
            grid[x,y] = null;
        }
    }

    public void DecreaseRow(int y)
    {
        for(int x = 0; x < w; ++x)
        {
            if(grid[x,y] != null)
            {
                grid[x,y-1] = grid[x,y];
                grid[x,y] = null;

                grid[x,y-1].position += new Vector3(0, -1, 0);
            }
        }
    }

    public void DecreaseRowsAbove(int y)
    {
        for(int i = y; i < h; ++i)
            DecreaseRow(i);
    }

    public bool IsRowFull(int y) 
    {
        for (int x = 0; x < w; ++x)
            if (grid[x, y] == null)
                return false;
        return true;
    }

    public int DeleteFullRows(bool realMove) 
    {
        int rowsDeleted = 0;

        for (int y = 0; y < h; ++y) 
        {
            if (IsRowFull(y)) 
            {
                if(realMove)
                {
                    DeleteRow(y);
                    DecreaseRowsAbove(y+1);
                    --y;
                }
                rowsDeleted++;
            }
        }

        return rowsDeleted;
    }

    private int ColumnHeight(int x)
    {
        int y = h-1;
        for(;y > 0 && grid[x,y] == null; y--);
        
        return y + 1;
    }

    public int AggregateHeight()
    {
        int totalHeight = 0;

        for(int x = 0; x < w; x++)
            totalHeight += ColumnHeight(x);

        return totalHeight;
    }

    public int Bumpiness()
    {
        int bumpiness = 0;

        for(int x = 0; x < w - 1; x++)
            bumpiness += Mathf.Abs(ColumnHeight(x) - ColumnHeight(x+1));

        return bumpiness;
    }

    public int Holes()
    {
        int count = 0;
        for(int x = 0; x < w; x++)
        {
            bool block = false;
            for(int y = h-1;y > 0; y--)
            {
                if (grid[x,y] != null)
                    block = true;
                else if (block)
                    count++;
            }
        }
        
        return count;
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawLine(new Vector3(horizontalOffset, 0, 0), new Vector3(horizontalOffset + w, 0, 0));
        Gizmos.DrawLine(new Vector3(horizontalOffset, 0, 0), new Vector3(horizontalOffset, h, 0));
        Gizmos.DrawLine(new Vector3(horizontalOffset, h, 0), new Vector3(horizontalOffset + w, h, 0));
        Gizmos.DrawLine(new Vector3(horizontalOffset+w, 0, 0), new Vector3(horizontalOffset+w, h, 0));
    }
}
