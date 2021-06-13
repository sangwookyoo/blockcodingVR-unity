using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadPortal : MonoBehaviour
{
    public string sceneName;

    void OnTriggerEnter(Collider c)
    {
        if (c.name == "PlayerFBX")
        {
            Debug.Log("포탈감지");
            SceneManager.LoadSceneAsync(sceneName);
            Debug.Log(sceneName + "로 이동합니다.");
        }
    }
}