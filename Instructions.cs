using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Instructions : MonoBehaviour
{
    
    [SerializeField] GameObject InstructionObj;
    [SerializeField] GameObject Loading;

    private bool canReloadScene;
    private Scene scene;


    void Start()
    {
        scene = SceneManager.GetActiveScene();
        canReloadScene = false;
        InstructionObj.SetActive(true);
        Loading.SetActive(false);
        StartCoroutine(InstructionsDisappear());
    }



    private IEnumerator InstructionsDisappear()
    {
        yield return new WaitForSeconds(10f);
        InstructionObj.SetActive(false);
        canReloadScene = true;
    }




    void Update()
    {
        if (Input.GetKey(KeyCode.M) && canReloadScene)
        {
            Loading.SetActive(true);
            SceneManager.LoadScene(scene.name);
        }
    }
}
