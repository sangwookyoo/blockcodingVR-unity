using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class AsyncOperationManager : MonoBehaviour
{
    public Slider progressBar;
    public Text loadtext;
    public string sceneName;

    private void Start()
    {
        StartCoroutine(LoadScene());
    }

    IEnumerator LoadScene()
    {
        yield return null;
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        Debug.Log("비동기로드를 시작합니다."); // 비동기로드 시작

        operation.allowSceneActivation = false;
        // 로드 전까지 씬을 실행하지 않음
        loadtext.text = (progressBar.value * 100.0f).ToString() + "%";

        while (!operation.isDone)
        {
            yield return null;

            if (progressBar.value < 0.9f)
            {
                progressBar.value = Mathf.MoveTowards(progressBar.value, 0.9f, Time.deltaTime);
            }
            else if (progressBar.value >= 0.9f)
            {
                progressBar.value = Mathf.MoveTowards(progressBar.value, 1f, Time.deltaTime);
            }
            if (progressBar.value >= 1f)
            {
                loadtext.text = "100%";
            }
            if (progressBar.value >= 1f && operation.progress >= 0.9f)
            {
                operation.allowSceneActivation = true;
                // 90%가 넘어가면 씬 로드
            }
        }
    }
}
