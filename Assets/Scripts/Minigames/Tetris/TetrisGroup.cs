using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetrisGroup : MonoBehaviour
{
    float lastFall = 0;

    Spawner spawner;
    TetrisGrid grid;
    bool player;

    public bool SetGrid(TetrisGrid grid, Spawner spawner, bool player)
    {
        this.grid = grid;
        this.spawner = spawner;
        this.player = player;

        return IsValidGridPos() == false;
    }

    void Update()
    {
        if (player && Input.GetKeyDown(KeyCode.LeftArrow)) 
            MoveLeft();

        else if (player && Input.GetKeyDown(KeyCode.RightArrow)) 
            MoveRight();

        else if (player && Input.GetKeyDown(KeyCode.UpArrow))
            RotateUp();

        else if (player && Input.GetKeyDown(KeyCode.DownArrow))
            RotateDown();

        if (player && Input.GetKey(KeyCode.Space) && Time.time - lastFall >= .1f || Time.time - lastFall >= .5f) 
        {
            MoveDown(true);
        }
    }

    public bool MoveLeft()
    {
        transform.position += new Vector3(-1, 0, 0);
        
            if (IsValidGridPos())
            {
                UpdateGrid();
                return true;
            }
            else
            {
                transform.position += new Vector3(1, 0, 0);
                return false;
            }
    }

    public bool MoveRight()
    {   
        transform.position += new Vector3(1, 0, 0);
        
            if (IsValidGridPos())
            {    
                UpdateGrid();
                return true;
            }
            else
            {
                transform.position += new Vector3(-1, 0, 0);
                return false;
            }
    }

    public int MoveDown(bool realMove)
    {
        int rowsDeleted = -1;
        transform.position += new Vector3(0, -1, 0);

        if (IsValidGridPos()) 
            UpdateGrid();
        else 
        {
            transform.position += new Vector3(0, 1, 0);
            rowsDeleted = grid.DeleteFullRows(realMove);

            // Debug.Log("Rows deleted: " + rowsDeleted);
            // Debug.Log("Height: " + grid.AggregateHeight());
            // Debug.Log("Holes: " + grid.Holes());

            // AI checks will not trigger this
            if(realMove) 
            {
                spawner.SpawnNext();
                enabled = false;
            }

        }
        
        lastFall = Time.time;
        return rowsDeleted;
    }

    public void RotateUp()
    {
        transform.Rotate(0, 0, -90);

        if (IsValidGridPos())
            UpdateGrid();
        else
        {
            // Try move right
            transform.position += new Vector3(1, 0, 0);
    
            if (IsValidGridPos())
                UpdateGrid();
            else
            {
                // Revert back
                transform.position += new Vector3(-1, 0, 0);

                // Try move left
                transform.position += new Vector3(-1, 0, 0);
        
                if (IsValidGridPos())
                    UpdateGrid();
                else
                {
                    // Revert back
                    transform.position += new Vector3(1, 0, 0);
                    // Rotate back
                    transform.Rotate(0, 0, 90);
                }
            }
        }
    }

    public void RotateDown()
    {
        transform.Rotate(0, 0, 90);
   
        if (IsValidGridPos())
            UpdateGrid();
        else
        {
            // Try move right
            transform.position += new Vector3(1, 0, 0);
    
            if (IsValidGridPos())
                UpdateGrid();
            else
            {
                // Revert back
                transform.position += new Vector3(-1, 0, 0);

                // Try move left
                transform.position += new Vector3(-1, 0, 0);
        
                if (IsValidGridPos())
                    UpdateGrid();
                else
                {
                    // Revert back
                    transform.position += new Vector3(1, 0, 0);
                    // Rotate back
                    transform.Rotate(0, 0, -90);
                }
            }
        }
    }

    bool IsValidGridPos()
    {
        foreach (Transform child in transform)
        {
            Vector2 vector = grid.RoundVec2(child.position);

            if(grid.InsideBorder(vector) == false)
                return false;

            if(grid.grid[(int)vector.x, (int)vector.y] != null &&
                grid.grid[(int)vector.x, (int)vector.y].parent != transform)
                return false;
        }
        return true;
    }

    void UpdateGrid()
    {
        for(int y = 0; y < TetrisGrid.h; ++y)
        {
            for(int x = 0; x < TetrisGrid.w; ++x)
            {
                if(grid.grid[x,y] != null)
                    if(grid.grid[x,y].parent == transform)
                        grid.grid[x,y] = null;
            }
        }

        foreach(Transform child in transform)
        {
            Vector2 vector = grid.RoundVec2(child.position);
            grid.grid[(int)vector.x,(int)vector.y] = child;
        }
    }
}
