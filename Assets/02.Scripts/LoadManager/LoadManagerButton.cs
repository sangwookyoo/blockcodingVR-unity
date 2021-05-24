using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadManagerButton : MonoBehaviour
{
    public string sceneName;

    public void OnButtonClick()
    {
        SceneManager.LoadSceneAsync(sceneName);
        Debug.Log(sceneName + "로 이동합니다.");
    }
}
