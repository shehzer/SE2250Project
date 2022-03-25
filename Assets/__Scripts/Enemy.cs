using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{

    // speed is 1.4 units. I.e. it takes 1.4/10 frames to move 1 unit 

    [SerializeField] float m_speed = 1.96f;
    [SerializeField] float m_jumpForce = 7.5f;
    [SerializeField] float m_rollForce = 6.0f;
    [SerializeField] bool m_noBlood = false;
    [SerializeField] GameObject m_slideDust;

    public int enemyCurrentHealth = 100;

    private Animator m_animator;
    private SpriteRenderer m_SpriteRenderer;
    private Rigidbody2D m_body2d;
    private Sensor_HeroKnight m_groundSensor;
    private Sensor_HeroKnight m_wallSensorR1;
    private Sensor_HeroKnight m_wallSensorR2;
    private Sensor_HeroKnight m_wallSensorL1;
    private Sensor_HeroKnight m_wallSensorL2;
    private bool m_isWallSliding = false;
    private bool m_grounded = false;
    private bool m_rolling = false;
    private int m_facingDirection = 1;
    private int m_currentAttack = 0;
    private float m_timeSinceAttack = 0.0f;
    private float m_delayToIdle = 0.0f;
    private float m_rollDuration = 8.0f / 14.0f;
    private float m_rollCurrentTime;

    private float xOffsetHitbox = 1.5f;
    private float yOffsetHitbox = 0.662f;
    private float xOffsetHitboxBoss = 5f;
    private float xOffsetHitboxShieldAttack = 1f;
    private float yOffsetHitboxShieldAttack = 0.662f;

    private float inputX = -1;

    public HeroKnight player;
    public GameObject playerObject;

    public bool deadState = false;
    public bool isBoss = false;

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
        player = playerObject.GetComponent<HeroKnight>();
        
        if (this.gameObject.name == "Boss(Clone)") 
            isBoss = true;

        GetComponent<SpriteRenderer>().flipX = true;
        m_facingDirection = 1;
    }

    // Update is called once per frame
    void Update()
    {
        // Increase timer that controls attack combo
        m_timeSinceAttack += Time.deltaTime;

        // Increase timer that checks roll duration
        if (m_rolling)
            m_rollCurrentTime += Time.deltaTime;

        // Disable rolling if timer extends duration
        if (m_rollCurrentTime > m_rollDuration)
            m_rolling = false;

        //Check if character just landed on the ground
        if (!m_grounded && m_groundSensor.State())
        {
            m_grounded = true;
            m_animator.SetBool("Grounded", m_grounded);
        }

        //Check if character just started falling
        if (m_grounded && !m_groundSensor.State())
        {
            m_grounded = false;
            m_animator.SetBool("Grounded", m_grounded);
        }

        // -- Handle input and movement --
        //float inputX = Input.GetAxis("Horizontal"); // always negative for left movement 

        // Swap direction of sprite depending on walk direction
        if (!deadState && m_grounded)
        {
            // enemy move left
            if (playerObject.transform.position.x < this.transform.position.x || playerObject.transform.position.x == this.transform.position.x)
            {
                inputX = -1;
                GetComponent<SpriteRenderer>().flipX = true;
                m_facingDirection = 1;
                // Move left
                if (!m_rolling && !deadState && m_timeSinceAttack > 0.4f)
                {
                    m_body2d.velocity = new Vector2(inputX * m_speed, m_body2d.velocity.y);
                }

            }

            // enemy move right 
            else if (playerObject.transform.position.x > this.transform.position.x)
            {
                inputX = 1;
                GetComponent<SpriteRenderer>().flipX = false;
                m_facingDirection = -1;
                // Move
                if (!m_rolling && !deadState && m_timeSinceAttack > 0.4f)
                    m_body2d.velocity = new Vector2(inputX * m_speed, m_body2d.velocity.y);

                //Set AirSpeed in animator
                m_animator.SetFloat("AirSpeedY", m_body2d.velocity.y);
            }

        }

        // -- Handle Animations --
        //Wall Slide
        m_isWallSliding = (m_wallSensorR1.State() && m_wallSensorR2.State()) || (m_wallSensorL1.State() && m_wallSensorL2.State());
        m_animator.SetBool("WallSlide", m_isWallSliding);

        // Run
        if (Mathf.Abs(inputX) > Mathf.Epsilon)
        {
            // Reset timer
            m_delayToIdle = 0.05f;
            m_animator.SetInteger("AnimState", 1);
        }

        //Idle
        else
        {
            // Prevents flickering transitions to idle
            m_delayToIdle -= Time.deltaTime;
            if (m_delayToIdle < 0)
                m_animator.SetInteger("AnimState", 0);
        }

        // handle being attacked. Bilateral hitbox (works when enemy is attacking from left or right)
        if (player.isAttacking && !deadState && m_timeSinceAttack > 0.4f && Mathf.Abs(this.transform.position.x - playerObject.transform.position.x) < xOffsetHitbox && Mathf.Abs(this.transform.position.y - playerObject.transform.position.y) < yOffsetHitbox)
        {
            ObjectTakeDamage(25, false);
            m_body2d.velocity = new Vector2(m_facingDirection * 3f, 2f);
        } else if (player.isBlocking && player.shieldCombatMode && !deadState && m_timeSinceAttack > 0.4f && Mathf.Abs(this.transform.position.x - playerObject.transform.position.x) < xOffsetHitboxShieldAttack && Mathf.Abs(this.transform.position.y - playerObject.transform.position.y) < yOffsetHitboxShieldAttack) {
            ObjectTakeDamage(15, true);
            DealDamageToPlayer(10);
            m_body2d.velocity = new Vector2(m_facingDirection * 2f, 1.5f);
        }

        // handle being attacked. Bilateral hitbox (works when enemy is attacking from left or right)
        if (player.isAttacking && !deadState && m_timeSinceAttack > 0.4f && Mathf.Abs(this.transform.position.x - playerObject.transform.position.x) < xOffsetHitbox && Mathf.Abs(this.transform.position.y - playerObject.transform.position.y) < yOffsetHitbox)
        {
            ObjectTakeDamage(25, false);
            m_body2d.velocity = new Vector2(m_facingDirection * 3f, 2f);
        } else if (player.isBlocking && player.shieldCombatMode && !deadState && m_timeSinceAttack > 0.4f && Mathf.Abs(this.transform.position.x - playerObject.transform.position.x) < xOffsetHitboxShieldAttack && Mathf.Abs(this.transform.position.y - playerObject.transform.position.y) < yOffsetHitboxShieldAttack) {
            ObjectTakeDamage(15, true);
            DealDamageToPlayer(10);
            m_body2d.velocity = new Vector2(m_facingDirection * 2f, 1.5f);
        }

        // handle being attacked. Bilateral hitbox (works when enemy is attacking from left or right)
        if (isBoss && player.isAttacking && !deadState && m_timeSinceAttack > 0.4f && Mathf.Abs(this.transform.position.x - playerObject.transform.position.x) < xOffsetHitboxBoss)
        {
            ObjectTakeDamage(25, false);
            m_body2d.velocity = new Vector2(m_facingDirection * 3f, 2f);
        } else if (isBoss && player.isBlocking && player.shieldCombatMode && !deadState && m_timeSinceAttack > 0.4f && Mathf.Abs(this.transform.position.x - playerObject.transform.position.x) < xOffsetHitboxBoss && Mathf.Abs(this.transform.position.y - playerObject.transform.position.y) < yOffsetHitboxShieldAttack) {
            ObjectTakeDamage(15, true);
            DealDamageToPlayer(10);
            m_body2d.velocity = new Vector2(m_facingDirection * 2f, 1.5f);
        }
    }

    void DeleteEnemy()
    {
        Destroy(this.gameObject);
    }

    // Animation Events
    // Called in slide animation.
    void AE_SlideDust()
    {
        Vector3 spawnPosition;

        if (m_facingDirection == 1)
            spawnPosition = m_wallSensorR2.transform.position;
        else
            spawnPosition = m_wallSensorL2.transform.position;

        if (m_slideDust != null)
        {
            // Set correct arrow spawn position
            GameObject dust = Instantiate(m_slideDust, spawnPosition, gameObject.transform.localRotation) as GameObject;
            // Turn arrow in correct direction
            dust.transform.localScale = new Vector3(m_facingDirection, 1, 1);
        }
    }

    void DealDamageToPlayer(int amount) {
        playerObject.GetComponent<HeroHealth>().ObjectTakeDamage(10);
    }

    void HealPlayer(int amount) {
        playerObject.GetComponent<HeroHealth>().Heal(amount);
    }

    void ObjectTakeDamage(int amount, bool shieldKill)
    {
        enemyCurrentHealth -= amount; // subtract health
        print("damage");

        // if (!isBoss) 
        //     m_animator.SetTrigger("Hurt");

        if (enemyCurrentHealth <= 0)
        {
            deadState = true;
            
            if (isBoss) {
                GameObject.Find("Main Camera").GetComponent<EnemySpawner>().UpdatePoints(500);
            } else if (!isBoss) {
                GameObject.Find("Main Camera").GetComponent<EnemySpawner>().UpdatePoints(150);
            }

            if (shieldKill) {
                HealPlayer(20);
            } else {
                HealPlayer(35);
            }
            // if(!isBoss)
            //     m_animator.SetTrigger("die");
            
            Invoke("DeleteEnemy", 1f);
        }
        m_timeSinceAttack = 0;
    }
}
