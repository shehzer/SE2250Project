using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    [SerializeField] Camera mainSceneCamera;
    private EnemySpawner sceneEnemySpawner;
    private int sceneNum;
    
    public Animator transitionScene;
    public float transitionDelay = 1f;

    void Start() {
        sceneEnemySpawner = mainSceneCamera.GetComponent<EnemySpawner>();

        // indicates we are in a transition scene, which follows the naming convention "BetweenLxLy" where x = scene we are transitioning from and y = scene we are transitioning to
        if (SceneManager.GetActiveScene().name.Substring(0,3).Equals("Bet")){
            sceneNum = -1; // indicates a transition scene 
        } else {
            sceneNum = int.Parse(SceneManager.GetActiveScene().name.Substring(5, 1));
        }
    }

    void Update()
    {
        // manual level change 
        if (Input.GetKeyDown("t")){
            LoadNewLevel();
        }

        // automatic level change
        if (sceneNum == -1) { 
            Invoke("LoadNewLevel", 3);
        } else if (sceneNum > 0 && sceneNum != 3 && sceneEnemySpawner.isBossDead) {
            Invoke("LoadNewLevel", 3);
        }
    }

    public void LoadNewLevel()
    {
        StartCoroutine(LoadLevel(SceneManager.GetActiveScene().buildIndex + 1));
    }
    
    IEnumerator LoadLevel (int indexLevel)
    {
        transitionScene.SetTrigger("Start");

        yield return new WaitForSeconds(transitionDelay);

        SceneManager.LoadScene(indexLevel);
    }
}
