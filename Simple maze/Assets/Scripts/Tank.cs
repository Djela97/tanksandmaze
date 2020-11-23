using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tank : MonoBehaviour
{

    public Rigidbody2D rb;
    public Transform rotationTransform;


    public TankObject tankForThis = null;
    public GameObject bulletPrefab;

    public Transform projectile;

    //koliko polja vide oko sebe tenkovi
    static int lineOfSight = 2;

    public static int enemiesDestroyed = 0;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if(tankForThis == null)
        {
            foreach (TankObject t in EnemyTank.enemies)
            {
                if (t.GetGameObject().Equals(gameObject))
                {
                    tankForThis = t;
                    break;
                }
            }
        }

        if (tankForThis != null)
        {
            if (tankForThis.Shoot())//ako nije pucao, onda radi movement
            {
                Shoot();
            }
            else
            {
                tankForThis.UpdateMovement();
            }
            
        }
    }

    public class TankObject
    {
        float targetX;
        float targetY;

        int currentX;
        int currentY;

        float currentAngle;
        float lastAngle;

        float walkSpeed = 2;
        float rotationSpeed = 15;
        Vector2 direction = Vector2.zero;

        GameObject go;

        Node castleNode { get; set; } = null;
        ArrayList pathToCastle = new ArrayList();

        int health = 100;

        float previousShot = 0.0f;

        public TankObject(int currentX, int currentY, GameObject gameObject)
        {
            this.currentX = currentX;
            this.currentY = currentY;
            go = gameObject;
        }

        public GameObject GetGameObject()
        {
            return go;
        }

        public void TakeDamage(int dmg)
        {
            health = health - dmg;
            if(health <= 0)
            {
                TankObject toDestroy = null;
                foreach (TankObject t in EnemyTank.enemies)
                {
                    if (t.GetGameObject().Equals(go))
                    {
                        toDestroy = t;
                        break;
                    }
                }
                Destroy(go);
                EnemyTank.enemies.Remove(toDestroy);
                Debug.Log(enemiesDestroyed);
                enemiesDestroyed++;
            }
        }
        public void UpdateMovement()
        {
            bool onlyRotate = false;
            Vector2Int targetVector = MazeGenerator.instance.NextPos(currentX, currentY);//promena koriscenog algoritma je u ovoj linije, kao vrednost targetVectora se stavi vrednost
            //Vector2Int targetVector = NextPosBfs(currentX, currentY); //ovo radi bfs, linija iznad radi heuristiku

            if (targetVector == null)
                return;
            targetX = targetVector.x;
            targetY = targetVector.y;

            if ((currentX == MazeGenerator.instance.castleX && Mathf.Abs(currentY - MazeGenerator.instance.castleY) == 1) ||
                (currentY == MazeGenerator.instance.castleY && Mathf.Abs(currentX - MazeGenerator.instance.castleX) == 1)) //ako smo tik do baze, necemo da ulazimo u bazu
            {
                onlyRotate = true;
            }

            bool targetReached = (Mathf.Abs(go.transform.position.x - targetX) < 0.1f) && (Mathf.Abs(go.transform.position.y - targetY) < 0.1f);
            //sledece dve linije daju lepo skretanje crvenim tenkovima, da ne idu dijagonalno
            currentX = targetX > go.transform.position.x ? Mathf.FloorToInt(go.transform.position.x) : Mathf.CeilToInt(go.transform.position.x);
            currentY = targetY > go.transform.position.y ? Mathf.FloorToInt(go.transform.position.y) : Mathf.CeilToInt(go.transform.position.y);



            float angle = 0;

            if (targetX - currentX > 0)
            {
                angle = 270;

                if (MazeGenerator.instance.GetMazeGridCell(currentX + 1, currentY) && targetReached)
                {
                    targetX = currentX + 1;
                    targetY = currentY;
                }
            }
            else if (targetX - currentX < 0)
            {
                angle = 90;

                if (MazeGenerator.instance.GetMazeGridCell(currentX - 1, currentY) && targetReached)
                {
                    targetX = currentX - 1;
                    targetY = currentY;
                }
            }
            else if (targetY - currentY > 0)
            {
                angle = 0;

                if (MazeGenerator.instance.GetMazeGridCell(currentX, currentY + 1) && targetReached)
                {
                    targetX = currentX;
                    targetY = currentY + 1;
                }
            }
            else if (targetY - currentY < 0)
            {
                angle = 180;

                if (MazeGenerator.instance.GetMazeGridCell(currentX, currentY - 1) && targetReached)
                {
                    targetX = currentX;
                    targetY = currentY - 1;
                }
            }
            else
            {
                angle = lastAngle;
            }

            currentAngle = Mathf.LerpAngle(currentAngle, angle, rotationSpeed * Time.deltaTime);
            /*if (!onlyRotate)
            {
                go.transform.position = Vector3.MoveTowards(go.transform.position, new Vector3(targetX, targetY), walkSpeed * Time.deltaTime);
            }*/

            if (MazeGenerator.instance.barriers.Contains(new Vector2Int((int)targetX, (int)targetY)))
            {
                targetX = currentX;
                targetY = currentY;
            } else if (!onlyRotate)
            {
                go.transform.position = Vector3.MoveTowards(go.transform.position, new Vector3(targetX, targetY), walkSpeed * Time.deltaTime);
            }

            //rotationTransform.eulerAngles = new Vector3(0, 0, currentAngle);
            go.transform.eulerAngles = new Vector3(0, 0, currentAngle);


            lastAngle = angle;
        }

        //vidno polje neka je 2 polja
        public bool Shoot()
        {
            int playerX = MazeRunner.instance.currentX;
            int playerY = MazeRunner.instance.currentY;
            int castleX = MazeGenerator.instance.castleX;
            int castleY = MazeGenerator.instance.castleY;

            int[] directionx = { 1, -1, 0, 0 };
            int[] directiony = { 0, 0, 1, -1 };

            for(int i = 0; i < 4; i++)
            {
                for (int j = 1; j < lineOfSight+1; j++)
                {
                    bool value = MazeGenerator.instance.GetMazeGridCell(currentX + directionx[i] * j, currentY + directiony[i] * j);
                    if (!value)
                        break;
                    //ako smo tik do zamak
                    if ((playerX == currentX + directionx[i] * j && playerY == currentY + directiony[i] * j) ||
                        (castleX == currentX + directionx[i] * j && castleY == currentY + directiony[i] * j))
                    {

                        if((int)go.transform.eulerAngles.z != 0 && Mathf.Abs((int)go.transform.eulerAngles.z) != 90 && Mathf.Abs((int)go.transform.eulerAngles.z) != 180 && Mathf.Abs((int)go.transform.eulerAngles.z) != 270)
                        {
                            return false;
                        }
                        

                        float vreme = Time.time;
                        if(Mathf.Abs(vreme - previousShot) > 2.0f) //reload od 2 sekunde za pucanje
                        {
                            previousShot = vreme;
                            return true;
                        }
                    }
                    //ako smo pored barijere
                    if(MazeGenerator.instance.barriers.Contains(new Vector2Int(currentX + directionx[i], currentY + directiony[i])))
                    {
                        float vreme = Time.time;
                        if (Mathf.Abs(vreme - previousShot) > 2.0f) //reload od 2 sekunde za pucanje
                        {
                            previousShot = vreme;
                            return true;
                        }
                    }

                    //ako naidjemo na false, break, jer je to zid, nema poente racunati preko
                }

            }
            return false;
        }

        public void DoBfs()
        {
            Vector2Int pos = new Vector2Int(currentX, currentY);

            Queue<Node> queue = new Queue<Node>();
            Node n = new Node(null, pos);
            queue.Enqueue(n);

            int[] directionx = { 1, -1, 0, 0 };
            int[] directiony = { 0, 0, 1, -1 };

            Vector2Int castleCoord = new Vector2Int(MazeGenerator.instance.castleX, MazeGenerator.instance.castleY);
            ArrayList visitedNodes = new ArrayList();
            visitedNodes.Add(n.coordinates);
            while (true)
            {
                n = queue.Dequeue();

                if (n.coordinates == castleCoord)
                {
                    castleNode = new Node(n, castleCoord);
                    break;
                }
                for (int i = 0; i < 4; i++)
                {
                    if (MazeGenerator.instance.GetMazeGridCell(n.coordinates.x + directionx[i], n.coordinates.y + directiony[i]))
                    {
                        Node newNode = new Node(n, new Vector2Int(n.coordinates.x + directionx[i], n.coordinates.y + directiony[i]));
                        if (!visitedNodes.Contains(newNode.coordinates))
                        {
                            visitedNodes.Add(newNode.coordinates);
                            queue.Enqueue(newNode);
                        }
                        

                    }
                }
            }
            //na kraju while, imamo cvor gde je castle, i preko backtrakinga preko roditelja dolazimo do pocetnog cvora gde je tank
            Node currNode = castleNode;
            while(currNode.parent != null)
            {
                pathToCastle.Add(currNode);
                currNode = currNode.parent;
            }
            pathToCastle.Reverse();
        }

        public Vector2Int NextPosBfs(int currentX, int currentY)
        {
            if (pathToCastle.Count == 0)
                return new Vector2Int(currentX, currentY);

            Vector2Int nextStep = ((Node)pathToCastle[0]).coordinates;
            if(currentX == nextStep.x && currentY == nextStep.y)
                pathToCastle.RemoveAt(0);
            return nextStep;
        }

    }

    public class Node
    {
        public Node parent { get; set; }
        public Vector2Int coordinates { get; set; }

        public Node(Node parent, Vector2Int coordinates)
        {
            this.parent = parent;
            this.coordinates = coordinates;
        }


    }
    void DestroyTank()
    {
        TankObject toDestroy = null;
        foreach(TankObject t in EnemyTank.enemies)
        {
            if (t.GetGameObject().Equals(gameObject))
            {
                toDestroy = t;
                break;
            }
        }
        Destroy(gameObject);
        EnemyTank.enemies.Remove(toDestroy);
    }

    public void Shoot()
    {
        Vector3 angle = projectile.eulerAngles;
        Vector3 newPosition = projectile.position;
        //Debug.Log("angle:" + angle);
        angle.z = angle.z + 0.5f;
        angle.z = (int)angle.z;
        if (angle.z == 270)
        {
            newPosition.x = projectile.position.x + 1;
        }
        else if (angle.z == 90)
        {
            newPosition.x = projectile.position.x - 1;
        }
        else if (angle.z == 0)
        {
            newPosition.y = projectile.position.y + 1;
        }
        else if (angle.z == 180)
        {
            newPosition.y = projectile.position.y - 1;
        }
        Instantiate(bulletPrefab, newPosition, projectile.rotation);

        //Debug.Log("started from: " + newPosition.x + " " + newPosition.y);
    }


}
