using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public Transform projectile;
    public GameObject bulletPrefab;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            Shoot();
        }
    }

    public void Shoot()
    {
        Vector3 angle = projectile.eulerAngles;
        Vector3 newPosition = projectile.position;
        Debug.Log("angle:" + angle);
        angle.z = angle.z + 0.5f;
        angle.z = (int)angle.z;
        if (angle.z == 270)
        {
            newPosition.x = projectile.position.x+1;
        }
        else if (angle.z == 90)
        {
            newPosition.x = projectile.position.x - 1;
        }
        else if (angle.z == 0)
        {
            newPosition.y= projectile.position.y + 1;
        }
        else if (angle.z == 180)
        {
            newPosition.y = projectile.position.y - 1;
        }
        Instantiate(bulletPrefab, newPosition, projectile.rotation);

        Debug.Log("started from: " + newPosition.x + " " + newPosition.y);
    }
}
