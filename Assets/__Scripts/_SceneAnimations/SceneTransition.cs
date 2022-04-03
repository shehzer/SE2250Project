using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    public Animator transitionScene;
    public float transitionDelay = 1f;
  
    void Update()
    {
        if (Input.GetKeyDown("t")){

            LoadNewLevel();
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
