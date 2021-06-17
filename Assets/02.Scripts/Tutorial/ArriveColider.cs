using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ArriveColider : MonoBehaviour
{
    public string sceneName;

    void OnTriggerEnter(Collider c)
    {
        if (c.name == "Player")
        {
            SceneManager.LoadSceneAsync(sceneName);
            Debug.Log(sceneName + "로 이동합니다.");
        }
    }
}
