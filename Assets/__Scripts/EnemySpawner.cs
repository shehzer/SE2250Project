using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemySpawner : MonoBehaviour
{
    public GameObject camera; 
    
    [SerializeField] GameObject enemyPrefab;
    [SerializeField] GameObject bossPrefab;
    [SerializeField] int spawnXOffset = 7;
    [SerializeField] int maxEnemies = 3;

    private GameObject spawnedEnemy;
    private int currentEnemies = 0;
    private float lastXCoord;
    private bool bossMan = false;
    private int currentPoints = 0;

    public HeroKnight player;
    public GameObject playerObject;

    // spawn one enemy initially at the start 
    // Start is called before the first frame update
    void Start()
    {
        lastXCoord = camera.transform.position.x;
        print("Start xCoord: " + lastXCoord);

        playerObject = GameObject.Find("HeroKnight");
        player = playerObject.GetComponent<HeroKnight>();

        spawnedEnemy = Instantiate(enemyPrefab, new Vector2(6, 0), new Quaternion(0, 0, 0, 0));
        currentEnemies++;
    }


    /* spawn enemy when spawn condition is met (current cam x coord is 2 units greater than it was at the last spawn event) 
     * with a boolean value that once its defeated the script captures the x value of the camera and repeats the same cycle.    
    */

    // Update is called once per frame
    void Update()
    {
        if (!bossMan && playerObject.transform.position.x >= 30 && spawnedEnemy == null) {
            // spawn boss0
            bossMan = true;
            spawnedEnemy = Instantiate(bossPrefab, new Vector2(52f,-3.56f), new Quaternion(0,0,0,0));
        }

        // this is a spawn condition.
        // When spawnedEnemy = null, then it is despawned, indicating that it is dead.
        if (camera.transform.position.x - lastXCoord > 2 && (spawnedEnemy == null) && currentEnemies != maxEnemies) {
            lastXCoord = camera.transform.position.x;
            spawnedEnemy = Instantiate(enemyPrefab, new Vector2(playerObject.transform.position.x + spawnXOffset, 0), new Quaternion(0, 0, 0, 0));
            currentEnemies++;
        }
    
    }
    
    public void UpdatePoints(int pointVal) {
        currentPoints += pointVal;
        Text text = GameObject.Find("PointText").GetComponent<Text>();
        text.text = "Points: " + currentPoints;
        
    }

    public void UpdateShieldStatus(bool status) {
        Text text = GameObject.Find("ShieldText").GetComponent<Text>();
        
        if(status) {
            text.text = "Shield Status: Attack";
        } else {
            text.text = "Shield Status: Blocking";
        }
        
    }
}
