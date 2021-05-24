using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollView : MonoBehaviour
{
    ScrollRect scrollRect;
    // Start is called before the first frame update
    void Start()
    {
        scrollRect = GetComponent<ScrollRect>();

    }

    // Update is called once per frame
    void Update()
    {
        float width = 100.0f;
        float height = 200.0f;
        // scrollRect.content를 통해서 Hierachy 뷰에서 봤던 Viewport 밑의 Content 게임 오브젝트에 접근할 수 있다.
        // 그리고 sizeDelta 값을 통해서 Content의 높이와 넓이를 수정할 수 있다.
        scrollRect.content.sizeDelta = new Vector2(width, height);

   
    }
}
