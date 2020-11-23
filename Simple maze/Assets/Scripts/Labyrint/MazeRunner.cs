using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MazeRunner : MonoBehaviour
{

    public float walkSpeed;
    public float rotationSpeed;

    public Camera topdownCamera;
    public Camera movingCamera;

    public Transform rotationTransform;
    Vector2 direction = Vector2.zero;

    int targetX = 1;
    int targetY = 1;

    public int currentX = 1;
    public int currentY = 1;
    float currentAngle;
    float lastAngle;

    public float CurrHealth { get; set; }
    public float MaxHealth { get; set; }

    public Slider healthBar;

    public Vector2 touchOrigin = -Vector2.one;

    //static instanca zato sto ne znam kako da uhvatim objekat igraca kad ga pogodi bullet
    //Registrujem kad ga metak pogodi ali ne mogu objekat da uzmem da skinem helte igracu
    //Verovatno je trebao tenk igraca da bude prefab od starta.
    public static MazeRunner instance;
    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        MaxHealth = 100;
        CurrHealth = 100;
        topdownCamera.enabled = false;
        topdownCamera.transform.position = new Vector3(currentX, currentY, -10);
        movingCamera.enabled = true;
        healthBar.value = CalculateHealth();
        Tank.enemiesDestroyed = 0;
    }

    void Update()
    {
        UpdateMovement();
       
        if (Input.GetKeyDown(KeyCode.V))
        {
            topdownCamera.enabled = !topdownCamera.enabled;
            movingCamera.enabled = !movingCamera.enabled;
            topdownCamera.transform.position = new Vector3(currentX, currentY, -10);
        }

        if (Input.GetKeyDown(KeyCode.KeypadMinus))
        {
            if (topdownCamera.enabled)
            {
                topdownCamera.orthographicSize += 1;
            } else if (movingCamera.enabled)
            {
                movingCamera.orthographicSize += 1;
            }
        }

        if (Input.GetKeyDown(KeyCode.KeypadPlus))
        {
            if (topdownCamera.enabled)
            {
                topdownCamera.orthographicSize -= 1;
            }
            else if (movingCamera.enabled)
            {
                movingCamera.orthographicSize -= 1;
            }
        }

        if(Input.GetKeyDown(KeyCode.KeypadPeriod) && topdownCamera.enabled)
        {
            topdownCamera.transform.position = new Vector3(currentX, currentY, -10);
        }

        if(Tank.enemiesDestroyed == 5)
        {
            GameObject.Find("Text").GetComponent<Text>().text = "YOU WIN!!!";
            GameObject.Find("Text").GetComponent<Text>().color = Color.green;
            Invoke("Win", 3f);
        }

    }

    private void UpdateMovement()
    {
        bool targetReached = transform.position.x == targetX && transform.position.y == targetY;
        currentX = targetX > transform.position.x ? Mathf.FloorToInt(transform.position.x) : Mathf.CeilToInt(transform.position.x);
        currentY = targetY > transform.position.y ? Mathf.FloorToInt(transform.position.y) : Mathf.CeilToInt(transform.position.y);

        #if UNITY_STANDALONE || UNITY_WEBPLAYER

        direction.x = Input.GetAxisRaw("Horizontal");
        direction.y = Input.GetAxisRaw("Vertical");

        #elif UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE

        //Check if Input has registered more than zero touches
        if (Input.touchCount > 0)
        {
            //Store the first touch detected.
            Touch myTouch = Input.touches[0];

            //Check if the phase of that touch equals Began
            if (myTouch.phase == TouchPhase.Began)
            {
                //If so, set touchOrigin to the position of that touch
                touchOrigin = myTouch.position;
            }

            //If the touch phase is not Began, and instead is equal to Ended and the x of touchOrigin is greater or equal to zero:
            else if (myTouch.phase == TouchPhase.Ended && touchOrigin.x >= 0)
            {
                //Set touchEnd to equal the position of this touch
                Vector2 touchEnd = myTouch.position;

                //Calculate the difference between the beginning and end of the touch on the x axis.
                float x = touchEnd.x - touchOrigin.x;

                //Calculate the difference between the beginning and end of the touch on the y axis.
                float y = touchEnd.y - touchOrigin.y;

                //Set touchOrigin.x to -1 so that our else if statement will evaluate false and not repeat immediately.
                touchOrigin.x = -1;

                //Check if the difference along the x axis is greater than the difference along the y axis.
                if (Mathf.Abs(x) > Mathf.Abs(y))
                    //If x is greater than zero, set horizontal to 1, otherwise set it to -1
                    direction.x = x > 0 ? 1 : -1;
                else
                    //If y is greater than zero, set horizontal to 1, otherwise set it to -1
                    direction.y = y > 0 ? 1 : -1;
            }
        }

        #endif //End of mobile platform dependendent compilation section started above with #elif


        float angle = 0;

        if (direction.x > 0)
        {
            angle = 270;

            if (MazeGenerator.instance.GetMazeGridCell(currentX + 1, currentY) && targetReached)
            {
                targetX = currentX + 1;
                targetY = currentY;
            }
        }
        else if (direction.x < 0)
        {
            angle = 90;

            if (MazeGenerator.instance.GetMazeGridCell(currentX - 1, currentY) && targetReached)
            {
                targetX = currentX - 1;
                targetY = currentY;
            }
        }
        else if (direction.y > 0)
        {
            angle = 0;

            if (MazeGenerator.instance.GetMazeGridCell(currentX, currentY + 1) && targetReached)
            {
                targetX = currentX;
                targetY = currentY + 1;
            }
        }
        else if (direction.y < 0)
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
        if (MazeGenerator.instance.barriers.Contains(new Vector2Int(targetX, targetY)))
        {
            targetX = currentX;
            targetY = currentY;
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(targetX, targetY), walkSpeed * Time.deltaTime);
        }

        rotationTransform.eulerAngles = new Vector3(0, 0, currentAngle);

        lastAngle = angle;
    }
    
    public float getAngle()
    {
        return lastAngle;
    }

    public void TakeDamage(int dmg)
    {
        CurrHealth -= dmg;
        healthBar.value = CalculateHealth();
        if (CurrHealth <= 0)
        {
            GameObject.Find("Text").GetComponent<Text>().text = "YOU DIED!!!";
            GameObject.Find("Text").GetComponent<Text>().color = Color.red;
            Invoke("YouDied", 3f);
        }
    }

    float CalculateHealth()
    {
        return CurrHealth / MaxHealth;
    }

    public void YouDied()
    {
        SceneManager.LoadScene(0);
        DestroyObjects();
    }

    public void Win()
    {
        SceneManager.LoadScene(0);
        DestroyObjects();
    }

    private void DestroyObjects()
    {
        EnemyTank.enemies = new ArrayList();
        Tank.enemiesDestroyed = 0;
    }

}