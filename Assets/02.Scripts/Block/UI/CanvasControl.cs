using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasControl : MonoBehaviour
{
    public bool CanvasCheck;
    // Start is called before the first frame update
    private void Start()
    {
        CanvasCheck = false;
    }
    /// <summary>
    /// 캔버스 온오프 확인용
    /// </summary>

    public void check()
    {
        Debug.Log(CanvasCheck);
    }
   
}
