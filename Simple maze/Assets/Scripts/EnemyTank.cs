using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTank : MonoBehaviour
{
    public static ArrayList enemies = new ArrayList();

    public GameObject enemyPrefab;

    public int maxNumberOfEnemies = 5;


    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("SpawnEnemy", 1, 7);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SpawnEnemy()
    {
        if (enemies.Count >= maxNumberOfEnemies)
            return;

        while (true)
        {

            System.Random rnd = new System.Random();
            int x = rnd.Next(0, MazeGenerator.instance.mazeWidth);
            int y = rnd.Next(0, MazeGenerator.instance.mazeHeight);

            if (MazeGenerator.instance.castleX == x && MazeGenerator.instance.castleY == y)
                continue;
        
            if (MazeGenerator.instance.GetMazeGridCell(x, y) == true)
            {

                GameObject e = Instantiate(enemyPrefab, new Vector3(x, y, 0), transform.rotation);
                Tank.TankObject t = new Tank.TankObject(x, y, e);
                t.DoBfs();
                enemies.Add(t);
                //Debug.Log("created enemy tank");
                break;
            }
        }
    }


}
