using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Maze : MonoBehaviour
{
    int width;
    int height;
    bool[,] grid;
    System.Random rg;

    int startX;
    int startY;

    public Maze(int width, int height, System.Random rg)
    {
        this.width = width;
        this.height = height;
        this.rg = rg;
    }

    void Start()
    { 
    }

    void Update()
    {

    }

    public bool[,] Grid
    {
        get { return grid; }
    }

    public void Generate()
    {
        grid = new bool[width, height];

        startX = 1;
        startY = 1;
        grid[startX, startY] = true;

        MazeDigger(startX, startY);

        string toPrint = "";

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                toPrint += grid[i, j] + " ";
            }
            toPrint += "\n";
            Debug.Log(toPrint);
        }

    }

    void MazeDigger(int x, int y)
    {
        int[] directions = new int[] { 1, 2, 3, 4 };

        Tools.Shuffle(directions, rg);

        for(int i = 0; i < directions.Length; i++)
        {
            if (directions[i] == 1)
            {
                if (y - 2 <= 0)
                    continue;

                if (grid[x, y - 2] == false)
                {
                    grid[x, y - 2] = true;
                    grid[x, y - 1] = true;

                    MazeDigger(x, y - 2);
                }
            }

            if (directions[i] == 2)
            {
                if (x - 2 <= 0)
                    continue;

                if (grid[x - 2, y] == false)
                {
                    grid[x - 2, y] = true;
                    grid[x - 1, y] = true;

                    MazeDigger(x - 2, y);
                }
            }

            if (directions[i] == 3)
            {
                if (x + 2 >= width - 1)
                    continue;

                if (grid[x + 2, y] == false)
                {
                    grid[x + 2, y] = true;
                    grid[x + 1, y] = true;

                    MazeDigger(x + 2, y);
                }
            }

            if (directions[i] == 4)
            {
                if (y + 2 >= height - 1)
                    continue;

                if (grid[x, y + 2] == false)
                {
                    grid[x, y + 2] = true;
                    grid[x, y + 1] = true;

                    MazeDigger(x, y + 2);
                }
            }
        }
    }
}