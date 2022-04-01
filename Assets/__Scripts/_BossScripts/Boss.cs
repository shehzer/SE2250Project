using UnityEngine;
using System.Collections;

public class Boss : MonoBehaviour
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
    public GameObject projectilePrefab;
    public float projectileSpeed = 40.0f;
    
    private int m_facingDirection = 1;
    private float _canAttack = -1f;
    private float _attackSpeed = 1f;
    private float attackRange = 2.5f;
    public bool deadState = false;
    public bool isBoss = false;
    public bool isFlipped = false;

    // Use this for initialization
    void Start()
    {
        m_animator = this.gameObject.GetComponent<Animator>();
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
        
        if((Vector2.Distance(playerObject.transform.position, m_body2d.position) <= attackRange)&&player.isAttacking && Time.time > _canAttack && !deadState)
        {
            // print("Boss is taking Damage");
            _canAttack = Time.time + _attackSpeed;
            boss.GetComponent<Boss_Health>().TakeDamage(200);
            // print(boss.GetComponent<Boss_Health>().currentHealth());
            deadState = boss.GetComponent<Boss_Health>().isDead;
            if(deadState)
            {
                m_animator.SetTrigger("die");
                // print("da boi is dead");
                Invoke("DeleteEnemy", 1f);
                GameObject.Find("Main Camera").GetComponent<EnemySpawner>().UpdatePoints(500);
                HealPlayer(50);
            }

        }
        

    }

    //function that deals damage to player
    public void DealDamageToPlayer(int amount) 
    {
        if(!player.isBlocking && !player.deadState)
        {
            playerObject.GetComponent<HeroHealth>().ObjectTakeDamage(amount);
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
        if(transform.position.x > playerObject.transform.position.x && isFlipped){
            transform.localScale = flipped;
            transform.Rotate(0f,180f,0f);
            isFlipped = false;
        }

        else if( transform.position.x < playerObject.transform.position.x && !isFlipped){
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

      //Firing method
    public void TempFire()
    {
        GameObject projGameObj = Instantiate(projectilePrefab);
        projGameObj.transform.position = transform.position;
       // projGameObj.transform.position.x = transform.position.x - 2.253647;
        Rigidbody2D rigidBody = projGameObj.GetComponent<Rigidbody2D>();
        rigidBody.velocity = Vector3.left * projectileSpeed;
        Destroy(projGameObj, 2f);
    }
}
