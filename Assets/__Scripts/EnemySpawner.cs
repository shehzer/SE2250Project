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
    private int sceneNum;

    public HeroKnight player;
    public GameObject playerObject;
    public bool isBossDead = false;

    // spawn one enemy initially at the start 
    // Start is called before the first frame update
    void Start()
    {
        sceneNum = int.Parse(SceneManager.GetActiveScene().name.Substring(5, 1));
        lastXCoord = this.transform.position.x;
        print("Start xCoord: " + lastXCoord);

        if (sceneNum == 1) {
            playerObject = GameObject.Find("HeroKnight");
            spawnedEnemy = Instantiate(enemyPrefab, new Vector2(6f, 0f), new Quaternion(0f, 0f, 0f, 0f));
        } else if (sceneNum == 2) {
            playerObject = GameObject.Find("Monk");
            spawnedEnemy = Instantiate(enemyPrefab, new Vector2(-19.52f, 0f), new Quaternion(0, 0, 0f, 0f));
        }

        player = playerObject.GetComponent<HeroKnight>();
        currentEnemies++;
    }


    /* spawn enemy when spawn condition is met (current cam x coord is 2 units greater than it was at the last spawn event) 
     * with a boolean value that once its defeated the script captures the x value of the camera and repeats the same cycle.    
    */

    // Update is called once per frame
    void Update()
    {
        if (sceneNum == 1 && !bossMan && playerObject.transform.position.x >= 30 && spawnedEnemy == null) {
            // spawn boss1
            bossMan = true;
            spawnedEnemy = Instantiate(bossPrefab, new Vector2(52f,-3.56f), new Quaternion(0,0,0,0));
        }


        if (sceneNum == 2 && !bossMan && playerObject.transform.position.x >= 26 && spawnedEnemy == null) {
            // spawn boss2
            bossMan = true;
            spawnedEnemy = Instantiate(bossPrefab, new Vector2(43.2f,-2.87f), new Quaternion(0,0,0,0));
        }
        
        // indicates the death of the boss
        if (bossMan && !isBossDead && spawnedEnemy == null) {
            isBossDead = true;
            print("boss is dead");
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
            text.text = "Block Status: Attack";
        } else {
            text.text = "Block Status: Blocking";
        }
        
    }
}
