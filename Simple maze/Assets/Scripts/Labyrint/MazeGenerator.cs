using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    public int mazeWidth = 0;
    public int mazeHeight = 0;

    public Sprite floorSprite;
    public Sprite roofSprite;
    public Sprite wallSprite;
    public Sprite wallCornerSprite;
    public GameObject castle;

    public MazeSprite mazeSpritePrefab;
    public Sprite enemySprite;

    public GameObject barrier;
    public int numberOfBarriers;
    public ArrayList barriers = new ArrayList();

    System.Random mazeRG;

    Maze maze;

    public static MazeGenerator instance;

    public int castleX;
    public int castleY;

    public int[,] mazeHeur;

    void Awake()
    {
        mazeHeight = PlayerPrefs.GetInt("numberOfRows");
        if (mazeHeight < 5)
            mazeHeight = 5;
        
        mazeWidth = PlayerPrefs.GetInt("numberOfColumns");
        if (mazeWidth < 5)
            mazeWidth = 5;

        numberOfBarriers = PlayerPrefs.GetInt("numberOfObstacles");
        if (numberOfBarriers > mazeHeight * mazeWidth / 2)
            numberOfBarriers = mazeWidth * mazeHeight / 2;

        instance = this;
    }

    void Start()
    {
        mazeRG = new System.Random();

        if (mazeWidth % 2 == 0)
            mazeWidth++;

        if (mazeHeight % 2 == 0)
        {
            mazeHeight++;
        }

        maze = new Maze(mazeWidth, mazeHeight, mazeRG);
        maze.Generate();

        mazeHeur = new int[mazeWidth, mazeHeight];

        DrawMaze();

        while (true)
        {
            System.Random rnd = new System.Random();
            int x = rnd.Next(0, mazeWidth);
            int y = rnd.Next(0, mazeHeight);

            if (maze.Grid[x, y] == true)
            {
                DrawCastle(x, y);
                castleX = x;
                castleY = y;
                break;
            }
        }

        for (int i = 0; i < numberOfBarriers; i++)
        {
            while (true)
            {
                System.Random rnd = new System.Random();
                int x = rnd.Next(0, mazeWidth);
                int y = rnd.Next(0, mazeHeight);
                if (x == castleX && y == castleY)
                    continue;
                Vector2Int v = new Vector2Int(x, y);
                if (maze.Grid[x, y] == true && !barriers.Contains(v))
                {
                    DrawBarrier(x, y);
                    barriers.Add(v);
                    break;
                }
            }
        }
        MakeAstarHeur();
    }

    void DrawMaze()
    {
        for (int x = 0; x < mazeWidth; x++)
        {
            for (int y = 0; y < mazeHeight; y++)
            {
                Vector3 position = new Vector3(x, y);

                if (maze.Grid[x, y] == true)
                {
                    CreateMazeSprite(position, floorSprite, transform, 0, mazeRG.Next(0, 3) * 90);
                }
                else
                {
                    CreateMazeSprite(position, roofSprite, transform, 0, 0);

                    DrawWalls(x, y);
                }
            }
        }
    }

    public void CreateMazeSprite(Vector3 position, Sprite sprite, Transform parent, int sortingOrder, float rotation)
    {
        MazeSprite mazeSprite = Instantiate(mazeSpritePrefab, position, Quaternion.identity) as MazeSprite;
        mazeSprite.SetSprite(sprite, sortingOrder);
        mazeSprite.transform.SetParent(parent);
        mazeSprite.transform.Rotate(0, 0, rotation);
    }

    void DrawCastle(int x, int y)
    {
        Vector3 position = new Vector3(x, y);
        Instantiate(castle, position, Quaternion.identity);
    }

    void DrawBarrier(int x, int y)
    {
        Vector3 position = new Vector3(x, y);
        Instantiate(barrier, position, Quaternion.identity);
    }

    void DrawWalls(int x, int y)
    {
        bool top = GetMazeGridCell(x, y + 1);
        bool bottom = GetMazeGridCell(x, y - 1);
        bool right = GetMazeGridCell(x + 1, y);
        bool left = GetMazeGridCell(x - 1, y);

        Vector3 position = new Vector3(x, y);

        if (top)
        {
            CreateMazeSprite(position, wallSprite, transform, 1, 0);
        }

        if (left)
        {
            CreateMazeSprite(position, wallSprite, transform, 1, 90);
        }

        if (bottom)
        {
            CreateMazeSprite(position, wallSprite, transform, 1, 180);
        }

        if (right)
        {
            CreateMazeSprite(position, wallSprite, transform, 1, 270);
        }

        if (!left && !top && x > 0 && y < mazeHeight - 1)
        {
            CreateMazeSprite(position, wallCornerSprite, transform, 2, 0);
        }

        if (!left && !bottom && x > 0 && y > 0)
        {
            CreateMazeSprite(position, wallCornerSprite, transform, 2, 90);
        }

        if (!right && !bottom && x < mazeWidth - 1 && y > 0)
        {
            CreateMazeSprite(position, wallCornerSprite, transform, 2, 180);
        }

        if (!right && !top && x < mazeWidth - 1 && y < mazeHeight - 1)
        {
            CreateMazeSprite(position, wallCornerSprite, transform, 2, 270);
        }
    }

    public bool GetMazeGridCell(int x, int y)
    {
        if (x >= mazeWidth || x < 0 || y >= mazeHeight || y < 0)
        {
            return false;
        }

        return maze.Grid[x, y];
    }

    void MakeAstarHeur()
    {
        for (int i = 0; i < mazeWidth; i++)
        {
            for (int j = 0; j < mazeHeight; j++)
            {
                mazeHeur[i, j] = -1;
            }
        }
        mazeHeur[castleX, castleY] = 0;

        Vector2Int pos = new Vector2Int(castleX, castleY);

        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        queue.Enqueue(pos);

        int[] directionx = {1, -1, 0, 0};
        int[] directiony = {0, 0, 1, -1};

        while(queue.Count > 0)
        {
            Vector2Int curr = queue.Dequeue();

            for(int i = 0; i < 4; i++)
            {
                //ako nije zid proveri da li je vrednost heuristike -1, ako jeste dodaj u kju,
                //jer to mesto nismo posetili. Ako vrednost nije -1, to znaci da smo vec posetili,
                //i proveri da li je prethodna heuristika veca od nova koja bi nastala. To znaci da smo
                //dosli do istog mesta u kracem broju koraka.
                if (GetMazeGridCell(curr.x + directionx[i], curr.y + directiony[i]) && (mazeHeur[curr.x + directionx[i], curr.y + directiony[i]] == -1 || 
                    mazeHeur[curr.x + directionx[i], curr.y + directiony[i]] > mazeHeur[curr.x, curr.y] + 1))
                {
                    mazeHeur[curr.x + directionx[i], curr.y + directiony[i]] = mazeHeur[curr.x, curr.y] + 1;
                    queue.Enqueue(new Vector2Int(curr.x + directionx[i], curr.y + directiony[i]));
                }
            }

        }
        string prnt = "";

        for (int i = 0; i < mazeWidth; i++)
        {
            for(int j = 0; j < mazeHeight; j++)
            {
                prnt += mazeHeur[i, j] + ", ";
            }
            prnt += "\n";
        }
        Debug.Log(prnt);
    }

    public Vector2Int NextPos(int x, int y)
    {
        int[] directionx = { 1, -1, 0, 0 };
        int[] directiony = { 0, 0, 1, -1 };
        Vector2Int toReturn = new Vector2Int(0, 0);
        int min = 1000000;
        for(int i = 0; i < 4; i++)
        {
            if (!GetMazeGridCell(x + directionx[i], y + directiony[i]))
                continue;
            if (mazeHeur[x + directionx[i], y + directiony[i]] < min)
            {
                toReturn = new Vector2Int(x + directionx[i], y + directiony[i]);
                min = mazeHeur[x + directionx[i], y + directiony[i]];
            }
        }

        return toReturn;
    }

}
