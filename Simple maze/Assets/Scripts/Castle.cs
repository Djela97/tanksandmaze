using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Castle : MonoBehaviour
{
    public float CurrHealth { get; set; }
    public float MaxHealth { get; set; }

    //Slider se veze za GameObject, medjutim kad se prevuce dole u prefab Slider nestane.
    public Slider healthBar;

    // Start is called before the first frame update
    void Start()
    {
        CurrHealth = 100;
        MaxHealth = 100;
        healthBar = GameObject.Find("BaseHealth").GetComponent<Slider>();
        healthBar.value = CalculateHealth();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(int dmg)
    {
        CurrHealth -= dmg;
        healthBar.value = CalculateHealth();
        if (CurrHealth <= 0)
        {
            GameObject.Find("Text").GetComponent<Text>().text = "YOU LOST!!!";
            GameObject.Find("Text").GetComponent<Text>().color = Color.red;
            Invoke("YouDied", 3f);
        }
    }

    public void YouDied()
    {
        
        SceneManager.LoadScene(0);
        DestroyObjects();
    }

    float CalculateHealth()
    {
        return CurrHealth / MaxHealth;
    }

    private void DestroyObjects()
    {
        EnemyTank.enemies = new ArrayList();
        Tank.enemiesDestroyed = 0;
    }

}
