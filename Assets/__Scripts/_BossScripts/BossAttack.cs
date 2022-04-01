using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAttack : StateMachineBehaviour
{   
    Transform player;
    Rigidbody2D rb;
    private float attackRange = 2f;
    private float moveSpeed = 2.5f;
    private float _canAttack = -1f;
    private float _attackSpeed = 1f;
    private int count = 0;
    private int specialAttack = 0;
    Boss boss;

    //OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //Initializing player, boss and boss's rigid body
       player = GameObject.FindGameObjectWithTag("Player").transform;
       rb = animator.GetComponent<Rigidbody2D>();
       boss = animator.GetComponent<Boss>();
    }

    //OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {   
        //Making boss look at player
        boss.LookAtPlayer();
        //finding players current position
        Vector2 target = new Vector2(player.position.x, rb.position.y);
        //moving boss towards the player
        Vector2 newPos = Vector2.MoveTowards(rb.position, target, moveSpeed * Time.fixedDeltaTime);
        rb.MovePosition(newPos);

        // if the boss is within attack range and player is not dead then melee attack
        if((Vector2.Distance(player.position, rb.position) <= attackRange) && Time.time > _canAttack && !boss.player.deadState){
            _canAttack = Time.time + _attackSpeed;
            //Attack
            animator.SetTrigger("Attack");
            boss.DealDamageToPlayer(10);
        }

        //check if the boss's hp is less than 400 hp, and player is further than melee range then do the range attack
        if(boss.GetComponent<Boss_Health>().currentHealth() < 400  && (Vector2.Distance(player.position, rb.position) >= attackRange) && count <3 && Time.time > _canAttack){
            _canAttack = Time.time + _attackSpeed;
            animator.SetTrigger("Attack1");
            boss.TempFire();
            count +=1;
            //on the second attack, stop the animation (there's a bug where the boss does 1 extra animation)
            if(count == 3){
                animator.ResetTrigger("Attack1");
            }
        }

        //if the boss has less than 200 hp, and is not in melee range, then do a special attack
        if(boss.GetComponent<Boss_Health>().currentHealth() <= 200 && (Vector2.Distance(player.position, rb.position) >= attackRange) && Time.time > _canAttack && specialAttack <2 ){
           _canAttack = Time.time + _attackSpeed;
           //call the special attack animation
           animator.SetTrigger("Attack2");

           //trigger the laser on the second time this if statement is hit -> weird bug where the first time does not trigger animation
            if(specialAttack >0){
                boss.SpecialAttack();
                animator.ResetTrigger("Attack2"); 
            }
            specialAttack +=1;
        }
    }

    //OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {   //reset all animations to avoid weird errors
       animator.ResetTrigger("Attack");
       animator.ResetTrigger("Attack1");
       animator.ResetTrigger("Attack2");
    }

 
}
