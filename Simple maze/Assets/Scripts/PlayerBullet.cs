using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    public float speed = 20f;
    public Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        rb.velocity = transform.up * speed;
    }

    private void Update()
    {
        if (!MazeGenerator.instance.GetMazeGridCell((int)(rb.position.x+0.5f), (int)(rb.position.y+0.5f))) {
            Destroy(gameObject);
            Debug.Log("destroying: " + rb.position.x + " " + rb.position.y);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Tank t = collision.GetComponent<Tank>();
        if(t != null)
        {
            t.tankForThis.TakeDamage(20);
            Destroy(gameObject);
        }

        Barrier b = collision.GetComponent<Barrier>();
        if (b != null)
        {
            Debug.Log("barrier hit with bullet");
            Destroy(b.gameObject);
            MazeGenerator.instance.barriers.Remove(new Vector2Int((int)b.gameObject.transform.position.x, (int)b.gameObject.transform.position.y));
            Destroy(gameObject);
        }
    }
}
