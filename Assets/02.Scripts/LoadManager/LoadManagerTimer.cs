using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadManagerTimer : MonoBehaviour
{
    public int timer;
    public string sceneName;

    void Start()
    {
        StartCoroutine(NextScene());
    }
    IEnumerator NextScene()
    {
        yield return new WaitForSeconds(timer);
        SceneManager.LoadScene(sceneName);
    }
}
