using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] GameObject enemyPrefab;
    [SerializeField] GameObject bossPrefab;
    [SerializeField] int spawnXOffset = 7;
    [SerializeField] int maxEnemies = 3;

    private GameObject spawnedEnemy;
    private int currentEnemies = 0;
    private float lastXCoord;
    private bool bossMan = false;
    private int currentPoints = 0;
    private int sceneNum; //.Equals("Level2Arena") ? 2 : 1;

    public HeroKnight player;
    public GameObject playerObject;

    // spawn one enemy initially at the start 
    // Start is called before the first frame update
    void Start()
    {
        sceneNum = int.Parse(SceneManager.GetActiveScene().name.Substring(5, 1));
        lastXCoord = this.transform.position.x;
        print("Start xCoord: " + lastXCoord);

        playerObject = GameObject.Find("HeroKnight");
        player = playerObject.GetComponent<HeroKnight>();

        
        if (sceneNum == 1) {
            spawnedEnemy = Instantiate(enemyPrefab, new Vector2(6f, 0f), new Quaternion(0f, 0f, 0f, 0f));
        } else if (sceneNum == 2) {
            spawnedEnemy = Instantiate(enemyPrefab, new Vector2(-19.52f, 0f), new Quaternion(0, 0, 0f, 0f));
        }
        currentEnemies++;
    }


    /* spawn enemy when spawn condition is met (current cam x coord is 2 units greater than it was at the last spawn event) 
     * with a boolean value that once its defeated the script captures the x value of the camera and repeats the same cycle.    
    */

    // Update is called once per frame
    void Update()
    {
        if (!bossMan && playerObject.transform.position.x >= 30 && spawnedEnemy == null && sceneNum == 1) {
            // spawn boss1
            bossMan = true;
            spawnedEnemy = Instantiate(bossPrefab, new Vector2(52f,-3.56f), new Quaternion(0,0,0,0));
        }


        if (!bossMan && playerObject.transform.position.x >= 26 && spawnedEnemy == null && sceneNum == 2) {
            // spawn boss2
            bossMan = true;
            spawnedEnemy = Instantiate(bossPrefab, new Vector2(43.2f,-2.87f), new Quaternion(0,0,0,0));
        }
        

        // this is a spawn condition.
        // When spawnedEnemy = null, then it is despawned, indicating that it is dead.
        if (this.transform.position.x - lastXCoord > 2 && (spawnedEnemy == null) && currentEnemies != maxEnemies) {
            lastXCoord = this.transform.position.x;
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
