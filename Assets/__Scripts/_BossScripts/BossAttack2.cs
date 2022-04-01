using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAttack2 : StateMachineBehaviour
{
    Transform player;
    Rigidbody2D rb;
    private float attackRange = 2f;
    private float moveSpeed = 2.5f;
    private float _canAttack = -1f;
    private float _attackSpeed = 1f;

    Boss2 boss;

    //OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       player = GameObject.FindGameObjectWithTag("Player").transform;
       rb = animator.GetComponent<Rigidbody2D>();
       boss = animator.GetComponent<Boss2>();
    }

    //OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {   
        boss.LookAtPlayer();
        Vector2 target = new Vector2(player.position.x, rb.position.y);
        Vector2 newPos = Vector2.MoveTowards(rb.position, target, moveSpeed * Time.fixedDeltaTime);
        rb.MovePosition(newPos);
        
        if((Vector2.Distance(player.position, rb.position) <= attackRange) && Time.time > _canAttack && !boss.player.deadState){
            _canAttack = Time.time + _attackSpeed;
            //Attack
            animator.SetTrigger("Attack");
            boss.DealDamageToPlayer(10);
        }
       
    }

    //OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       animator.ResetTrigger("Attack");
    }

 
}
