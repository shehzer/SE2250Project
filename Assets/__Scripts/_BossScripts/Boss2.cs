using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss2 : MonoBehaviour
{

    // speed is 1.4 units. I.e. it takes 1.4/10 frames to move 1 unit 

    [SerializeField] float m_speed = 1.96f;
    [SerializeField] bool m_noBlood = false;

    public int enemyCurrentHealth = 100;

    private Animator m_animator;
    private SpriteRenderer m_SpriteRenderer;
    private Rigidbody2D m_body2d;
    private Sensor_HeroKnight m_groundSensor;
    private Sensor_HeroKnight m_wallSensorR1;
    private Sensor_HeroKnight m_wallSensorR2;
    private Sensor_HeroKnight m_wallSensorL1;
    private Sensor_HeroKnight m_wallSensorL2;
    public HeroKnight player;
    public GameObject playerObject;
    public GameObject boss;
    
    private int m_facingDirection = 1;
    private float m_delayToIdle = 0.0f;
    private float _canAttack = -1f;
    private float _attackSpeed = 1f;
    private float attackRange = 2.6f;
    private float inputX = -1;
    public bool deadState = false;
    public bool isBoss = false;
    public bool isFlipped = false;

    // Use this for initialization
    void Start()
    {
        m_animator = GetComponent<Animator>();
        m_body2d = GetComponent<Rigidbody2D>();
        m_groundSensor = transform.Find("GroundSensor").GetComponent<Sensor_HeroKnight>();
        m_wallSensorR1 = transform.Find("WallSensor_R1").GetComponent<Sensor_HeroKnight>();
        m_wallSensorR2 = transform.Find("WallSensor_R2").GetComponent<Sensor_HeroKnight>();
        m_wallSensorL1 = transform.Find("WallSensor_L1").GetComponent<Sensor_HeroKnight>();
        m_wallSensorL2 = transform.Find("WallSensor_L2").GetComponent<Sensor_HeroKnight>();

        m_SpriteRenderer = GetComponent<SpriteRenderer>();
        playerObject = GameObject.Find("HeroKnight");
        boss = this.gameObject;
        if (boss == null){
            Debug.LogError("No Boss component found.");  
            }
        player = playerObject.GetComponent<HeroKnight>();
        
        if (this.gameObject.name == "Boss(Clone)") 
            isBoss = true;

        GetComponent<SpriteRenderer>().flipX = true;
        m_facingDirection = 1;
    }

    // Update is called once per frame
    void Update()
    {
        
        /*
        Deal damage to the boss
        */
        
        // attack
        if((Vector2.Distance(playerObject.transform.position, m_body2d.position) <= attackRange)&&player.isAttacking && Time.time > _canAttack && !deadState)
        {
            // print("Boss is taking Damage");
            _canAttack = Time.time + _attackSpeed;
            boss.GetComponent<Boss_Health>().TakeDamage(100);
            deadState = boss.GetComponent<Boss_Health>().isDead;

            if(deadState)
            {
                m_body2d.velocity = Vector2.zero;
                // print("da boi is dead");
                m_animator.SetTrigger("Death");
                Invoke("DeleteEnemy", 1.5f);
                GameObject.Find("Main Camera").GetComponent<EnemySpawner>().UpdatePoints(500);
                HealPlayer(50);
            } else if (!deadState) {
                m_body2d.velocity = new Vector2( (isFlipped == true ? 1 : -1) * 2f, 1.5f); // bounceback vector
            }
        }        
    }

    //function that deals damage to player
    public void DealDamageToPlayer(int amount) 
    {
        if(!player.isBlocking && !player.deadState)
        {
            playerObject.GetComponent<HeroHealth>().ObjectTakeDamage(10);
        }
    }

    //function that heals player
    void HealPlayer(int amount) 
    {
        playerObject.GetComponent<HeroHealth>().Heal(amount);
    }

    //Function to make sure boss faces player
    public void LookAtPlayer()
    {
        Vector3 flipped = transform.localScale;
        flipped.z *= -1f;
        if(transform.position.x < playerObject.transform.position.x && isFlipped){
            transform.localScale = flipped;
            transform.Rotate(0f,180f,0f);
            isFlipped = false;
        }

        else if( transform.position.x > playerObject.transform.position.x && !isFlipped){
            transform.localScale = flipped;
            transform.Rotate(0f,180f,0f);
            isFlipped = true;
        }
    }

    //function to delete boss object
    private void DeleteEnemy()
    {
        Destroy(this.gameObject);
    }
}
