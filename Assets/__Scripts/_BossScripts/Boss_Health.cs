using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_Health : MonoBehaviour
{
    // Start is called before the first frame update

    public int health = 500;
    public GameObject boss;
    public Animator anim;
    public bool isDead = false;

    public bool isInvulnerable = false;

    public void TakeDamage(int damage){
        if(isInvulnerable)
            return;
        
        health -= damage;

        if(health <= 0){
            isDead = true;
        }
    }
}
