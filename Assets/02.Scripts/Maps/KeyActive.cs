using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class KeyActive : MonoBehaviour
{
    public GameObject key;
    public GameObject key_particle;
    public string sceneName;

    private float num;

    void Update()
    {
        if (key.activeSelf == true)
        {
            key_particle.SetActive(true);
            Destroy(key, 5);
            Destroy(key_particle, 5);
            num += Time.deltaTime;

            if (num > 5.0f)
            {
                num = 0.0f;
                SceneManager.LoadSceneAsync(sceneName);
                Debug.Log(sceneName + "로 이동합니다.");
            }
        }
    }
}