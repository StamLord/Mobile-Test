using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public TetrisGrid grid;
    public TetrisAI ai;
    [SerializeField] private GameObject[] groups;
    private bool running = true;

    void Start()
    {
        SpawnNext();
    }

    public void SpawnNext()
    {
        if(running == false)
            return;

        int i = Random.Range(0, groups.Length);
        TetrisGroup piece = Instantiate(groups[i], transform.position, Quaternion.identity).GetComponent<TetrisGroup>();
        
        bool gameOver = piece.SetGrid(grid, this, (ai == null));
        if(gameOver)
        {
            GameOver();
            return;
        }
        
        if(ai)
            ai.NewPiece(piece, grid);
    }

    public void GameOver()
    {
        running = false;
    }
}
