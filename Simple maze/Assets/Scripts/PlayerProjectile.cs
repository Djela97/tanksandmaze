using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProjectile : MonoBehaviour
{
    public Transform projectile;
    public GameObject bulletPrefab;
    public MazeRunner mazeRunner;

    // Update is called once per frame
    void Update()
    {
#if UNITY_STANDALONE || UNITY_WEBPLAYER
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Shoot();
        }
#elif UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE
        if (IsDoubleTap())
        {
            Shoot();
        }
#endif
    }

    void Shoot()
    {
        float angle = mazeRunner.getAngle();
        Vector3 newPosition = projectile.position;
        Debug.Log(angle);
        if (angle == 270)
        {
            newPosition.x = projectile.position.x+1;
        }
        else if (angle == 90)
        {
            newPosition.x = projectile.position.x - 1;
        }
        else if (angle == 0)
        {
            newPosition.y= projectile.position.y + 1;
        }
        else if (angle == 180)
        {
            newPosition.y = projectile.position.y - 1;
        }
        Instantiate(bulletPrefab, newPosition, projectile.rotation);

        Debug.Log("started from: " + newPosition.x + " " + newPosition.y);
    }

    public static bool IsDoubleTap()
    {
        bool result = false;
        float MaxTimeWait = 1;
        float VariancePosition = 1;

        if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            float DeltaTime = Input.GetTouch(0).deltaTime;
            float DeltaPositionLenght = Input.GetTouch(0).deltaPosition.magnitude;

            if (DeltaTime > 0 && DeltaTime < MaxTimeWait && DeltaPositionLenght < VariancePosition)
                result = true;
        }
        return result;
    }
}
