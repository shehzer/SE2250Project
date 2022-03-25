using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HeroHealth : MonoBehaviour
{
    public int heroMaxHealth = 100;
    public int heroCurrentHealth;
    public HealthBarStatus healthBar; 
    public Animator anim;
    public HeroKnight player;
  
    void Start()
    {
        heroCurrentHealth = heroMaxHealth;
        healthBar.setMaxHealth(heroMaxHealth);
        player = GameObject.Find("HeroKnight").GetComponent<HeroKnight>();
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Firepit")
        {
            ObjectTakeDamage(15); // subtract 15 from health every time player touches fire
        } else if (collision.tag == "Enemy" && !player.isBlocking && !collision.gameObject.GetComponent<Enemy>().deadState) { // subtract 10 from health every time an enemy hits a player
            ObjectTakeDamage(10);
        }
    }

    public void ObjectTakeDamage (int amount)
    {
        heroCurrentHealth -= amount; // subtract health
        healthBar.SetHealth(heroCurrentHealth);
        anim.SetTrigger("Hurt");

        if (heroCurrentHealth <= 0)
        {
            player.deadState = true;
            anim.SetTrigger("die");
            Invoke("RestartScene", 3f);
        }
    }

    public void Heal (int amount)
    {
        heroCurrentHealth += amount; // add certain amount of healh 
        if (heroCurrentHealth > heroMaxHealth) // if the current health is greater than max health
        {
            heroCurrentHealth = heroMaxHealth; // caps out health - cannot exceed max health
        }
    }

    //Method that restores the original scene of the game once the hero has been destroyed
    public void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
